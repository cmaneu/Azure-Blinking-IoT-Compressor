# Connectez votre board à votre IoT Hub

Jusqu'à présent, vous avez travaillé sur une solution IoT qui a déjà été créée pour 
vous à l'avance. Dans ce module, nous allons voir comment créer une solution IoT
complète à l'aide d'[Azure IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/about-iot-hub?wt.mc_id=blinkingcompressor-github-chmaneu). 
Ce service vous permet de créer de solutions IoT pouvant gérer des millions de devices, tout en vous permettant d'abstraire la complexité de communication avec
les devices IoT (Internet public, réseaux sigfox ou LoRA, etc...), ainsi que la
diversité des types de devices (Rasperry Pi, Arduino, microcontrôleurs, PC, Mobiles, ...).

Avant de poursuivre, pensez à installer l'ensemble [des prérequis](01-prepare-environment/configure-dev-env.md#module-connectez-vos-devices-iot-au-cloud) nécessaires à cette partie. Ils seront nécessaires afin de reconfigurer votre board pour qu'elle communique avec l'IoT Hub que vous allez créer.

- Créer les ressources dans Azure
- Reconfigurer votre board
- Connectez votre fonction à votre IoT Hub
- Accédez a vos devices depuis Visual Studio Code