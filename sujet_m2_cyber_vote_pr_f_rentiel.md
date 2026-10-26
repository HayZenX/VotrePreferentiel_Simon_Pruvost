# Évaluation M2 Cyber
**Développement d'un Worker Service C# — Vote préférentiel**

## 1. Modalités de rendu
- Créer un dépôt GitHub contenant les sources.
- Envoyer un e-mail à l'adresse `jean-lou.pierme@lacatholille.fr` en indiquant le SHA-1 du dernier commit et le lien vers le dépôt.

---

## 2. Le vote préférentiel
Une association étudiante organise chaque année l'élection de son président ou de sa présidente. Face au nombre croissant de participants et aux contestations liées au dépouillement manuel, elle souhaite automatiser le traitement des bulletins. Votre équipe est donc chargée de développer un outil fiable capable de trier les préférences exprimées et de désigner la personne gagnante.

Le mode de scrutin utilisé est le vote préférentiel :
- Chaque votant classe les candidats selon son ordre de préférence.
- Comptabiliser les premiers choix de l'ensemble des votants.
  - Éliminer le candidat ayant obtenu le moins de voix. Si plusieurs candidats ont obtenu le même nombre minimal de voix, les éliminer simultanément.
  - Reporter les voix des candidats éliminés sur le choix suivant encore en lice dans chaque bulletin.
- Répéter le processus d'élimination et de report des voix jusqu'à ce qu'il ne reste plus qu'un seul candidat en lice. Ce dernier est déclaré gagnant.

### Exemple
Trois candidats se présentent : Alice, Bilal et Chloé. Cinq votants indiquent leur classement :

| Votant | 1er choix | 2e choix | 3e choix |
| :--- | :--- | :--- | :--- |
| Votant 1 | Alice | Bilal | Chloé |
| Votant 2 | Alice | Chloé | Bilal |
| Votant 3 | Bilal | Chloé | Alice |
| Votant 4 | Chloé | Bilal | Alice |
| Votant 5 | Chloé | Alice | Bilal |

**Premier tour :** Alice obtient 2 voix, Bilal 1 voix et Chloé 2 voix. Bilal a le moins de voix : il est éliminé. Le votant 3 avait choisi Bilal en premier et Chloé en second ; sa voix est donc transférée à Chloé.

**Tour suivant :** Alice conserve 2 voix et Chloé obtient 3 voix. Alice ayant le moins de voix, elle est éliminée. **Chloé, désormais seule candidate encore en lice, remporte l'élection.**

---

## 3. Consignes
Développer un Worker Service chargé de calculer le résultat de l'élection. Celui-ci devra télécharger le fichier de votes au format JSON hébergé sur GitHub, le désérialiser, puis déterminer la personne gagnante.

Réaliser l'exercice en suivant les étapes ci-dessous :

### 3.1 Bibliothèque de classes
Créer une solution nommée `VotePreferentiel`, puis ajouter une bibliothèque de classes nommée `VotePreferentiel`, dont le fichier projet sera `VotePreferentiel.csproj`.

### 3.2 Modèle de données
Examiner le fichier JSON d'entrée et modéliser son contenu dans une classe `VoteModel`.

### 3.3 Récupération des votes
Créer une classe `VoteProvider` chargée d'effectuer l'appel HTTP, de récupérer le fichier JSON et de le désérialiser. Utiliser l'injection de dépendances pour obtenir une instance de `HttpClient`.

### 3.4 Worker Service
Créer un projet de type **Worker Service** nommé `VotePreferentiel.Worker`, dont le fichier projet sera `VotePreferentiel.Worker.csproj`. Utiliser la classe `VoteProvider` dans ce projet afin de vérifier qu'elle retourne correctement les données.

### 3.5 Calcul du gagnant
Implémenter une classe `VoteCounter` chargée de calculer le résultat de l'élection à partir des données fournies par `VoteProvider`.

Cette classe devra contenir les méthodes suivantes :
```csharp
// Récupère la liste des candidats présents dans les votes.
HashSet<string> GetCandidates(VoteModel voteModel);

// Détermine le gagnant à partir des choix des votants.
string GetWinner(VoteModel voteModel);

// Récupère le premier choix de chaque votant.
List<string> GetAllFirstChoices(VoteModel voteModel);

// Compte les premières préférences obtenues par chaque candidat.
IDictionary<string, int> CountFirstChoices(List<string> firstChoices);

// Trouve le ou les candidats ayant obtenu le moins de premières préférences.
string[] GetCandidatesWithLeastVotes(
    IDictionary<string, int> firstChoiceCounts);
```

Créer une interface pour cette classe. Utiliser ensuite l'injection de dépendances afin d'appeler, depuis le Worker Service, le service de comptage des votes par l'intermédiaire de cette interface. Afficher le nom du gagnant dans la console.

### 3.6 Tester la solution
Créer un projet de tests unitaires avec **TUnit**, nommé `VotePreferentiel.Tests`, dont le fichier projet sera `VotePreferentiel.Tests.csproj`, puis implémenter les tests suivants pour la méthode `GetWinner` :

1. **Cas nominal :** reprendre les votes de l'exemple et vérifier que la gagnante est « Chloé ».
2. **Un seul candidat :** utiliser un bulletin contenant `[« Alice »]` et vérifier que la gagnante est « Alice ».
3. **Report des préférences :** quatre votes `[« Alice », « Bilal », « Chloé », « Diane »]`, trois votes `[« Bilal », « Chloé », « Alice », « Diane »]`, trois votes `[« Chloé », « Alice », « Bilal », « Diane »]` et un vote `[« Diane », « Chloé », « Bilal », « Alice »]`. Au premier tour, Diane est éliminée et sa voix est transférée à Chloé. Au deuxième tour, Bilal est éliminé et ses trois voix sont transférées à Chloé. Vérifie que « Chloé » gagne.
4. **Égalité en dernière position :** utiliser les votes `[« Alice », « Chloé », « Bilal »]`, `[« Bilal », « Chloé », « Alice »]`, `[« Chloé », « Alice », « Bilal »]` et `[« Chloé », « Bilal », « Alice »]` ; vérifier que « Chloé » gagne après l'élimination simultanée d'Alice et de Bilal, à égalité en dernière position.
5. **Bulletin vide :** utiliser un bulletin ne contenant aucun choix et vérifier qu'une exception est levée.