# Créer un projet IoT Workbench

!> **Attention**: Ce module est en cours de rédaction.

## Créer le projet

## Ecrire du code pour le MxChip

Après la création du projet, vous aurez un fichier `device.ino`. Il contiendra seulement 
deux méthodes : 

- Une méthode `setup()` : 
- Une méthode `loop()` : 

```c
void loop() {
  // put your main code here, to run repeatedly:
  if (hasIoTHub && hasWifi)
  {
    char buff[128];

    // replace the following line with your data sent to Azure IoTHub
    snprintf(buff, 128, "{\"topic\":\"iot\"}");
    
    if (DevKitMQTTClient_SendEvent(buff))
    {
      Screen.print(1, "Sending...");
    }
    else
    {
      Screen.print(1, "Failure...");
    }
    delay(2000);
  }
}
```