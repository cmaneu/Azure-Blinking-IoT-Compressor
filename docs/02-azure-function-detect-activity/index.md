# Détectez l'activité du compresseur avec Azure Functions

> Si vous réalisez ce workshop seul, sans quelqu'un de l'équipe relations
> développeurs, alors il vous faudra réaliser le module 3 "Connectez vos devices
> IoT au Cloud" avant de réaliser ce module.

## Ce que l'on cherche à réaliser

Nous avons déjà connecté les boards au cloud pour vous (vous pourrez le faire 
vous-même dans le prochain module). Nous avons besoin de vous afin de détecter
d'après les données des capteurs si le compresseur est en fonctionnement ou non.

Vous allez donc développer une API REST à l'aide d'Azure Functions qui sera appelée
dès que la carte IoT enverra une nouvelle donnée. A vous de créer un super algo
qui retournera l'état du compresseur :grinning:.

## Qu'est ce qu'Azure Function ?

Azure Functions est un des services _serverless_ d'Azure. Il vous permet de développer
des "fonctions" de code qui seront exécutées à la demande, avec un paiement à l'usage
ainsi qu'un auto-scaling. 

Il est possible de développer des Azure Functions avec un ensemble de langages: Java, Python, C#, Node.JS, etc...
Une fois développé, les Azure Functions peuvent être hébergées sur Azure, mais également sur d'autres clouds, sur 
vos serveurs - via une image Docker - et même sur des devices IoT !

## Comment développer une Azure Function ?

Vous avez deux possibilités pour développer une Azure Function : 

- **Via le portail Azure**: avec une expérience entièrement dans votre navigateur. Cela est pratique 
pour faire quelques tests, ou un petit PoC,
- **Sur votre poste de développement (Windows, Mac, Linux)**: avec Visual Studio ou Visual Studio Code. C'est
la solution recommandée pour des projets de production.