# Créer une Fonction dans VS Code

Visual Studio Code vous permet de créer et de tester votre code en local, souvent sans accès à Internet. Il est ensuite
possible de déployer directement votre code dans une Azure function, ou via un pipeline d'intégration/déploiement continu.

Si vous n'avez pas Visual Studio Code, vous pouvez également [développer votre fonction dans le portail Azure](02-azure-function-detect-activity/create-function-portal.md).

## Créer le projet

Pour créer un projet Azure Functions, lancez la barre de commandes (_Ctrl+Shift+P_), et recherchez la commande
`Azure Functions: Create New Project...`. Choisissez un répertoire vide et répondez aux options de création du projet: 

- **Le langage**: Vous pouvez choisir n'importe quel langage pour ce projet,
- **Le trigger**: C'est l'élément qui va venir déclencher l'exécution de votre fonction. Dans le cas d'une API REST, il 
faudra sélectionner _HTTP trigger_,
- **Un nom de fonction**: Ce nom doit être prédictif (j'ai choisi `prediction`). Par défaut, ce sera également une partie
de l'URL
- **Le type d'autorisation**: Est-ce que votre fonction doit être accessible sans authentification, ou avec une clé. Pour
ce workshop, vous êtes libres de choisir l'une des deux méthodes, mais je vous encourage à sélectionner _Function_. Prenons 
de bonnes habitudes :).
- **Comment souhaitez-vous ouvrir le projet**: dans une nouvelle fenêtre VS Code ou dans la même.

![](/img/function-vscode-01.png ':size=300%')
![](/img/function-vscode-02.png ':size=300%')
![](/img/function-vscode-03.png ':size=300%')
![](/img/function-vscode-04.png ':size=300%')
![](/img/function-vscode-05.png ':size=300%')

Une fois ces options choisies, VS Code va automatiquement vous créer un squelette de projet et de fonction. Il exécutera
ensuite des tâches nécessaires en fonction du langage: créer un environnement virtuel en python, faire un package restore
en NodeJS ou en .Net, etc...

![](/img/function-vscode-08.png)

Sachez que le projet que vous venez de créer peut contenir d'autres functions, quelque soit leur trigger mais avec 
le même langage de programmation. Si vous souhaitez ajouter une nouvelle fonction, vous pouvez le faire 
via la commande `Azure Functions: Create Function...`

## Tester et débugguer votre projet

Avant d'aller plus loin, nous allons tester que vous pouvez exécuter et débugger votre projet.
Si votre environnement de développement est correctement installé, vous devriez pouvoir lancer la commande 
`Debug: Start debugging` (_F5_) et l'hôte Azure Functions va alors se lancer. Vous verrez apparaître dans le terminal
l'adresse HTTP locale à laquelle vous pouvez tester votre API.

![](/img/function-vscode-09.png)