using System;
using System.Linq.Expressions;
using System.Reflection;

//https://tryfinally.dev/detours-redirecting-csharp-methods-at-runtime?_sm_pdc=1&_sm_rid=FQ7SMMJPMvP2Pvkqt6MWlr21tP0sPSMMkRq4RHF
internal static class DetourUtility
{
	/// <summary> Returns the get accessor MethodInfo obtained from a method call expression. </summary>
	public static MethodInfo MethodInfoForMethodCall(Expression<Action> methodCallExpression)
	{
		if (methodCallExpression == null)
			throw new ArgumentNullException(nameof(methodCallExpression));

		return methodCallExpression.Body is MethodCallExpression methodCall ? methodCall.Method
			: throw new ArgumentException($"Couldn't obtain MethodInfo for the method call expression: {methodCallExpression}");
	}

	/// <summary> Returns the get accessor MethodInfo obtained from a property expression. </summary>
	public static MethodInfo MethodInfoForGetter<T>(Expression<Func<T>> propertyExpression)
	{
		if (propertyExpression == null)
			throw new ArgumentNullException(nameof(propertyExpression));

		return propertyExpression.Body is MemberExpression member && member.Member is PropertyInfo propInfo ? propInfo.GetMethod
			: throw new ArgumentException($"Couldn't obtain MethodInfo for the property get accessor expression: {propertyExpression}");
	}

	/// <summary> Returns the set accessor MethodInfo obtained from a property expression. </summary>
	public static MethodInfo MethodInfoForSetter<T>(Expression<Func<T>> propertyExpression)
	{
		if (propertyExpression == null)
			throw new ArgumentNullException(nameof(propertyExpression));

		return propertyExpression.Body is MemberExpression member && member.Member is PropertyInfo propInfo ? propInfo.SetMethod
			: throw new ArgumentException($"Couldn't obtain MethodInfo for the property set accessor expression: {propertyExpression}");
	}



	// this is based on an interesting technique from the RimWorld ComunityCoreLibrary project, originally credited to RawCode:
	// https://github.com/RimWorldCCLTeam/CommunityCoreLibrary/blob/master/DLL_Project/Classes/Static/Detours.cs
	// licensed under The Unlicense:
	// https://github.com/RimWorldCCLTeam/CommunityCoreLibrary/blob/master/LICENSE
	public static unsafe void TryDetourFromTo(MethodInfo src, MethodInfo dst)
	{
		try
		{
			if (IntPtr.Size == sizeof(Int64))
			{
				// 64-bit systems use 64-bit absolute address and jumps
				// 12 byte destructive

				// Get function pointers
				long srcBase = src.MethodHandle.GetFunctionPointer().ToInt64();
				long dstBase = dst.MethodHandle.GetFunctionPointer().ToInt64();

				// Native source address
				byte* pointerRawSource = (byte*)srcBase;

				// Pointer to insert jump address into native code
				long* pointerRawAddress = (long*)(pointerRawSource + 0x02);

				// Insert 64-bit absolute jump into native code (address in rax)
				// mov rax, immediate64
				// jmp [rax]
				*(pointerRawSource + 0x00) = 0x48;
				*(pointerRawSource + 0x01) = 0xB8;
				*pointerRawAddress = dstBase; // ( pointerRawSource + 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 )
				*(pointerRawSource + 0x0A) = 0xFF;
				*(pointerRawSource + 0x0B) = 0xE0;
			}
			else
			{
				// 32-bit systems use 32-bit relative offset and jump
				// 5 byte destructive

				// Get function pointers
				int srcBase = src.MethodHandle.GetFunctionPointer().ToInt32();
				int dstBase = dst.MethodHandle.GetFunctionPointer().ToInt32();

				// Native source address
				byte* pointerRawSource = (byte*)srcBase;

				// Pointer to insert jump address into native code
				int* pointerRawAddress = (int*)(pointerRawSource + 1);

				// Jump offset (less instruction size)
				int offset = dstBase - srcBase - 5;

				// Insert 32-bit relative jump into native code
				*pointerRawSource = 0xE9;
				*pointerRawAddress = offset;
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Unable to detour: {src?.Name ?? "null src"} -> {dst?.Name ?? "null dst"}\n{ex}");
			throw;
		}
	}
}

