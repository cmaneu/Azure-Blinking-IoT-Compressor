#include "Arduino.h"
#include "Sensor.h"
#include "projectconfig.h"

RGB_LED rgbLed;

void setupLed()
{
    rgbLed.turnOff();
    rgbLed.setColor(255, 0, 0);
    delay(500);
    rgbLed.setColor(0, 255, 0);
    delay(500);
    rgbLed.setColor(0, 0, 255);
    delay(500);
    rgbLed.turnOff();
}

void setLed(int r, int g, int b)
{
#ifdef DEBUG_SERIAL
    Serial.printf("LED changed to %i %i %i\r\n", r, g, b);
#endif    
    if (r == 0 && g == 0 && b == 0)
    {
        rgbLed.turnOff();
    }
    else
    {
        rgbLed.setColor(r, g, b);
    }   
}