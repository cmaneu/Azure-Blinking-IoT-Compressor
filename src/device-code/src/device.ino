#include "projectconfig.h"
#include "sensors.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "led.h"
#include "parson.h"

static bool hasWifi = false;
static bool hasIoTHub = false;


void parseTwinMessage(DEVICE_TWIN_UPDATE_STATE updateState, const char *message)
{
    int ledR, ledG, ledB = 0;
    JSON_Value *root_value;
    root_value = json_parse_string(message);
    if (json_value_get_type(root_value) != JSONObject)
    {
        if (root_value != NULL)
        {
            json_value_free(root_value);
        }
        LogError("parse %s failed", message);
        return;
    }
    JSON_Object *root_object = json_value_get_object(root_value);

    if (updateState == DEVICE_TWIN_UPDATE_COMPLETE)
    {
        JSON_Object *desired_object = json_object_get_object(root_object, "desired");
        if (desired_object != NULL)
        {
          JSON_Object *led_object = json_object_get_object(desired_object, "led");
          if (led_object != NULL)
          {
            if (json_object_has_value(led_object, "r"))
            {
              ledR = json_object_get_number(led_object, "r");
            }
            if (json_object_has_value(led_object, "g"))
            {
              ledG = json_object_get_number(led_object, "g");
            }
            if (json_object_has_value(led_object, "b"))
            {
              ledB = json_object_get_number(led_object, "b");
            }
          } 
        }
    }
    else
    {
      JSON_Object *led_object = json_object_get_object(root_object, "led");
      if (led_object != NULL)
      {
        if (json_object_has_value(led_object, "r"))
        {
          ledR = json_object_get_number(led_object, "r");
        }
        if (json_object_has_value(led_object, "g"))
        {
          ledG = json_object_get_number(led_object, "g");
        }
        if (json_object_has_value(led_object, "b"))
        {
          ledB = json_object_get_number(led_object, "b");
        }
      } 
    }

    #ifdef DEBUG_SERIAL
      Serial.printf("Device twin update received!\r\n");
    #endif 
    setLed(ledR, ledG, ledB);

    json_value_free(root_value);
}

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
  parseTwinMessage(updateState, temp);
  free(temp);
}


void setup() {
  Screen.print(0, "IoT Compressor");
  Screen.print(1, "By @cmaneu & co");
  Screen.print(2, "github.com/");
  Screen.print(3, "cmaneu");
  setupLed();

  delay(1500);
  Screen.clean();
  Screen.print(0, "IoT Compressor");
  
  Screen.print(1, "Wifi ...");
  if (WiFi.begin() == WL_CONNECTED)
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
    // DevKitMQTTClient_SetOption(OPTION_MINI_SOLUTION_NAME, "BlinkingCompressor");

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
    
    // Collecting Pressure
    float pressure = readPressure();
    
    // Collecting Temperature and humidity
    float temp = readTemperature();
    float humidity = readHumidity();

    // Collecting Sound level
    // TODO

    // Display data
    char buffDisplay[128];
    sprintf(buffDisplay, "IoT Compressor \r\nTemp:%.1f3 C   \r\nHumidity:%.2f%% \r\nPres:%.1fmb         \r\n" , temp, humidity, pressure);
    Screen.print(buffDisplay);
    delay (3000);

    // Prepare data  
    char buff[128];
    snprintf(buff, 128, "{\"topic\": \"iot\", \"t\": \"%.1f\", \"p\": \"%.1f\", \"h\": \"%.1f\"}", temp, pressure, humidity);
    
    // Send to IoT Hub
    Screen.print(1, "Sending...");
    if (DevKitMQTTClient_SendEvent(buff))
    {
      messageNumber = messageNumber+1;
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
