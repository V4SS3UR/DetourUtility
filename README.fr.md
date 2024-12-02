# DetourUtility : Un outil léger de redirection de méthode en C#

**DetourUtility** est une bibliothèque légère en C# conçue pour rediriger dynamiquement les appels de méthode à l'exécution en utilisant du code non sécurisé pour manipuler la mémoire à bas niveau. Cette technique, connue sous le nom de "détournement", est particulièrement utile pour modifier le comportement des méthodes lorsque le code source n'est pas accessible, comme dans Unity ou d'autres assemblages compilés.

## Fonctionnalités clés
- Extraction de `MethodInfo` à partir d'expressions d'appel de méthode, d'expressions getter et setter.  
- Compatible avec les systèmes `32 bits` et `64 bits`.  
- Permet le détournement d'appels de méthode via du code non sécurisé en modifiant les pointeurs de fonction à l'exécution.

## Cas d'utilisation
Le détournement est généralement utilisé dans les scénarios suivants :  
1. **Modding** : Particulièrement utile dans le modding de jeux pour remplacer ou étendre les fonctions du jeu.  
2. **Solutions pour l'éditeur Unity** : Bien que Unity permette désormais davantage de modifications, certains comportements du moteur nécessitent encore des ajustements. Le détournement permet de corriger des bugs dans l'éditeur, d'ajouter des outils ou de contourner des limitations.  
3. **Modifications du moteur pendant l'exécution** : Bien que cela ne soit pas idéal pour la production, le détournement peut résoudre des problèmes pendant le développement en l'absence d'autres solutions.  
4. **Patching d'assemblages** : Utile pour modifier du code dans des assemblages où le code source est indisponible.

## Limitations et risques
L'utilisation du détournement comporte des risques importants, notamment :  
- **Plantages et corruption des données** : La manipulation de pointeurs de fonction peut entraîner des plantages, des blocages ou des corruptions de données.  
- **Limitations de la plateforme** : Le détournement fonctionne bien avec Mono et .NET, mais présente des limites avec IL2CPP (par exemple, dans Unity pour les consoles).  
- **Récursion** : Un détournement incorrect peut entraîner des appels récursifs, provoquant des débordements de pile.  
- **Modifications permanentes** : Une fois appliqués, les détournements sont permanents pour l'assemblage chargé et ne permettent pas d'appeler la méthode originale à moins qu'elle ne soit réimplémentée manuellement.  

Assurez-vous de bien comprendre les impacts avant d'utiliser le détournement.

## Exemple d'utilisation
```csharp
using System.Reflection;
using DetourUtility;

// Exemple de redirection (détournement) d'appels de méthode
MethodInfo originalMethod = typeof(SomeClass).GetMethod("OriginalMethod");
MethodInfo newMethod = typeof(SomeClass).GetMethod("NewMethod");

DetourUtility.TryDetourFromTo(originalMethod, newMethod);
```

## Comment ça fonctionne
`DetourUtility` réécrit les pointeurs de fonction en mémoire pour rediriger les appels de méthode. En fonction de l'architecture du système (32 bits ou 64 bits), la classe adapte sa logique pour gérer différentes instructions de saut au niveau de l'assembleur.  

Cette technique s'inspire de méthodes éprouvées utilisées dans les communautés de modding et de patching logiciel, particulièrement dans les environnements nécessitant des modifications de code à l'exécution.

## Crédits
[Detours : Redirecting C# Methods at Runtime](https://tryfinally.dev/detours-redirecting-csharp-methods-at-runtime?_sm_pdc=1&_sm_rid=FQ7SMMJPMvP2Pvkqt6MWlr21tP0sPSMMkRq4RHF)
