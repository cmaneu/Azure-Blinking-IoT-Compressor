# FAQ - for proctors


## Setup the environment

The first modules of this workshop are based on a pre-build environment. You'll
need an Azure Subscription, with the following resources: 

- an IoT Hub,
- a Storage Account,
- an Azure App Service (free tier should work),
- an Event Hub
- An Azure Function

### Steps

- Create an Azure Storage Account (LRS),
- Create an Azure IoT Hub - free tier is OK,
- Create an event hub with name `iot-events`,
- Deploy the Azure Function from `function-dispatcher`
    - Setup the `iotHubConnectionString` environment variable with the service connection string from IoT Hub
    - Set `RegistryStorage` env. variable with the connection string of the Storage Account,
    - Set `iotworkshop-dev_iothubroutes_iotworkshopfr_EVENTHUB` with the connection string of the event hub
    - Get the URL with the function key for the `CreateDevice` function
- Deploy the Azure App Service with code from `backend-registrationwebsite`
    - Set `REGISTRATIONWEBSITE_IoTHubConnectionString` with IoT Hub service Connection
    - Set `REGISTRATIONWEBSITE_RegistryStorage` with Storage Account connection string,
- With Azure Explorer, create an empty table `webhooks`


## Understand board states

| Board picture | State         |
|---------------|---------------|
| ![](img/boot-codeversion.jpg) | Second screen of the boot screen. Displays code version. |
| ![](img/iothub-.jpg) | The board is booting up. The LED will go from Red, to green to blue to off while booting |
| ![](img/no-wifi.jpg) | This means that neither the preconfigured Wifi, nor the selected wifi in WiFi mode is available. Restart in WiFi configuration mode. |
| ![](img/wifi-noiothub.jpg) | There is some Wifi, but no connection to IoT Hub. Either the IoT Hub Connection string is incorrect, or there is no Internet behind that wifi. |
| ![](img/iothub-failure.jpg) | The board is displaying `Failure...` message. The IoT Hub was not reachable, while WiFi was enabled. Check if Wifi has still access to Internet, and reboot the board. |
| ![](img/dispatcher-nowebhook.jpg) | The LED is on a green-yellow state after the first update sent. There is probably no webhook configured for that board. Check the webhook table. |
