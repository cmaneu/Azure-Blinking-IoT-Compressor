#include "HTS221Sensor.h"
#include "AzureIotHub.h"
#include "Arduino.h"
#include "parson.h"
#include <assert.h>
#include "projectconfig.h"
#include "RGB_LED.h"
#include "Sensor.h"
#include "LIS2MDLSensor.h"

DevI2C *i2c;
HTS221Sensor *ht_sensor;
LPS22HBSensor *pressureSensor;

void setupSensors()
{
    i2c = new DevI2C(D14, D15);

    ht_sensor = new HTS221Sensor(*i2c);
    ht_sensor->init(NULL);

    pressureSensor = new LPS22HBSensor(*i2c);
    pressureSensor->init(NULL);
}

float readTemperature()
{
    ht_sensor->reset();
    float temperature = 0;
    ht_sensor->getTemperature(&temperature);
    return temperature;
}

float readPressure()
{
    float pressure = 0;
    pressureSensor->getPressure(&pressure);
    return pressure;
}

float readHumidity()
{
    ht_sensor->reset();
    float humidity = 0;
    ht_sensor->getHumidity(&humidity);
    return humidity;
}
