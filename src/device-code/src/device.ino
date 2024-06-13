#include "projectconfig.h"
#include "sensors.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "led.h"
#include "deviceTwin.h"
// #include "queue.h"

static bool hasWifi = false;
static bool hasIoTHub = false;
static bool isRunning = false;

// QueueArray<int> _accelerationX = QueueArray<int>();
// QueueArray<int> _accelerationY = QueueArray<int>();
// QueueArray<int> _accelerationZ = QueueArray<int>();

static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
#ifdef DEBUG_SERIAL
  Serial.printf("Device twin update received!\r\n");
#endif 

  char *temp = (char *)malloc(size + 1);
  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';
  // parseTwinMessage(updateState, temp);
  free(temp);
}

// float computeJitter(int *array)
// {
//   int differences[sizeof(array) -1] = {0};

//   for (size_t i = 1; i < sizeof(array); i++)
//   {
//     differences[i-1] = abs(array[i-1] - array[i]);
//   }

//   int jitter = 0;

//   for (size_t i = 0; i < sizeof(differences); i++)
//   {
//     jitter = jitter + differences[i];
//   }
  
//   return jitter / sizeof(differences);
// }

void setup() {
  Screen.print(0, "IoT Compressor");
  Screen.print(1, "By @cmaneu & co");
  Screen.print(2, "github.com/");
  Screen.print(3, "cmaneu");
  setupLed();
  Screen.print(1, "Code version");
  Screen.print(2, FIRMWARE_VERSION);
  Screen.print(3, "");

  delay(1500);
  Screen.clean();
  Screen.print(0, "IoT Compressor");
  
  Screen.print(1, "Wifi ...");

  int wifiState = WiFi.begin("IoTWorkshop","AzureRocks");

  if(wifiState != WL_CONNECTED)
  {
    wifiState = WiFi.begin();
  }

  if (wifiState == WL_CONNECTED)
  {
    hasWifi = true;
    Screen.print(1, "Wifi ...OK");
    Screen.print(2, "IoT Hub...");

    if (!DevKitMQTTClient_Init(true))
    {
      hasIoTHub = false;
      Screen.print(0, "/!\\ Error");
      Screen.print(2, "No IoT Hub");
      return;
    }
    hasIoTHub = true;
    DevKitMQTTClient_SetDeviceTwinCallback(DeviceTwinCallback);

    Screen.print(2, "IoT Hub...OK");
    Screen.print(3, "Sensors...");
    setupSensors();
    Screen.print(3, "Sensors...OK");
  }
  else
  {
    hasWifi = false;
    Screen.print(1, "No Wi-Fi");
  }
}

static int messageNumber = 1;

void loop() {

  if (hasIoTHub && hasWifi)
  {
    messageNumber = messageNumber+1;

    // Collecting Temperature and humidity
    float temp = readTemperature();
    float humidity = readHumidity();

    // Collecting Accelerometer data

    // _accelerationX.~QueueArray();
    // _accelerationY.~QueueArray();
    // _accelerationZ.~QueueArray();

    // _accelerationX = QueueArray<float>();
    // _accelerationY = QueueArray<float>();
    // _accelerationZ = QueueArray<float>();

    // for (size_t i = 0; i < 20; i++)
    // {
    //   int *accelerometerdata = readAccelerometer();
    //   _accelerationX.enqueue(accelerometerdata[0]);
    //   _accelerationY.enqueue(accelerometerdata[1]);
    //   _accelerationZ.enqueue(accelerometerdata[2]);
    //   delay(100);
    // }
    
    // float jitterX, jitterY, jitterZ;

    // jitterX = computeJitter(_accelerationX.getArray());
    // jitterY = computeJitter(_accelerationY.getArray());
    // jitterZ = computeJitter(_accelerationZ.getArray());

    // if (jitterX > 20 || jitterY > 20 || jitterZ > 20)
    // {
    //   isRunning = true;
    //   setLed(255, 0, 0);
    // }
    // else
    // {
    //   isRunning = false;
    //   setLed(0, 0, 0);
    // }

    // Display data
    char buffDisplay[128];
    sprintf(buffDisplay, "IoT Compressor \r\nTemp:%.1f3 C   \r\nHumidity:%.2f%% \r\nPres:%.1fmb         \r\n" , temp, humidity, pressure);
    Screen.print(buffDisplay);
    delay (3000);

    // Prepare data  
    char buff[128];
    snprintf(buff, 128, "{\"topic\": \"iot\", \"updateId\":\"%i\", \"t\": \"%.1f\", \"p\": \"%.1f\", \"h\": \"%.1f\"}", messageNumber, temp, pressure, humidity);
    
    // Send to IoT Hub
    Screen.print(1, "Sending...");
    if (DevKitMQTTClient_SendEvent(buff))
    {
      char updateBuffer [17];
      snprintf(updateBuffer, 17, "Update #%d sent", messageNumber);
      Screen.print(1, updateBuffer);
    }
    else
    {
      Screen.print(1, "Failure...");
    }

    delay(LOOP_INTERVAL);
  }
  else 
  {
    Screen.print(4, "Reboot device?");
    delay(LOOP_INTERVAL);
  }
}
