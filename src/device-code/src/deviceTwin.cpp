#include "parson.h"
#include "led.h"
#include "DevKitMQTTClient.h"

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