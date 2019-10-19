# Ecrire du code pour détecter l'activité du compresseur

Nous voulons maintenant écrire un "algorithme" qui permettra de détecter d'après les données de télémétrie si 
le compresseur est en fonctionnement ou pas. Nous vous laissons le soin de concocter l'algorithme !
Il sera exposé sous la forme d'une API REST, et sera appelée automatiquement par le _dispatcher_ que 
nous avons créés.

## Input / Output

Votre API REST sera appelée par le dispatcher via une requête POST, avec le body suivant: 

```json
{
    "deviceId" = "device-42",
    "temperature" = 27.13,
    "humidity" = 50.2,
    "pressure" = 1004.8
}
```

Votre API doit retourner un JSON indiquant l'état du compresseur, ainsi que la couleur de la LED à afficher. L'idée est
d'avoir une LED verte quand le compresseur est éteint (aucun risque), puis rouge quand il est en fonctionnement 
(_attention, compresseur sous tension_).

Le format de sortie attendu est le suivant: 

```json
{
    "state": "running",
    "led": 
    {
        "r": 0,
        "g": 255,
        "b": 0
    }
}
```

La propriété `state` peut avoir comme valeurs `running` ou `idle`. Les valeurs de la led doivent être entre 0 et 255.

## Quelques exemples d'API

Si vous voulez tester une API déjà réalisée, voici quelques endpoints.

```
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysGreen
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysGreen
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysRed
https://iotcompressor-evaluation.azurewebsites.net/api/Prediction?code=7j3bnVqlu2/KDArv8uTXadNfLqcNaTQEsIWeDTMvsD/8NL2sNQJoag==
```

## Quelques exemples de code

### JavaScript

Cet exemple de JavaScript va simplement retourner que le compresseur est tout le temps
en fonctionnement, et affiche une couleur de led rouge si la température dépasse les 30°.

```javascript
module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');
    
    var temperatureRed = 0;
    if (req.body && req.body.temperature > 30)
    {
        temperatureRed = 255;
    }

    context.res = {
            body: {
                "state": "running",
                "led": 
                {
                    "r": temperatureRed,
                    "g": 0,
                    "b": 0
                }
            }
        };

};
```

