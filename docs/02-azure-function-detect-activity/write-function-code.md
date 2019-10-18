# Ecrire du code pour détecter l'activité du compresseur



## Quelques exemples

```
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysGreen
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysGreen
https://iotcompressor-evaluation.azurewebsites.net/api/AlwaysRed
https://iotcompressor-evaluation.azurewebsites.net/api/Prediction?code=7j3bnVqlu2/KDArv8uTXadNfLqcNaTQEsIWeDTMvsD/8NL2sNQJoag==
```


```javascript
module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');
    context.res = {
            body: {
                "state": "running",
                "led": 
                {
                    "r": 0,
                    "g": 255,
                    "b": 0
                }
            }
        };

};
```