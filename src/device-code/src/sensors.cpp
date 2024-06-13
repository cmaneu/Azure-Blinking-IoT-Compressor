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
LSM6DSLSensor *accelerometerGyroscopeSensor;

void setupSensors()
{
    i2c = new DevI2C(D14, D15);

    ht_sensor = new HTS221Sensor(*i2c);
    ht_sensor->init(NULL);

    pressureSensor = new LPS22HBSensor(*i2c);
    pressureSensor->init(NULL);

    accelerometerGyroscopeSensor = new LSM6DSLSensor(*i2c, D4, D5);
    accelerometerGyroscopeSensor->init(NULL);
    accelerometerGyroscopeSensor->enableAccelerator();
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

int * readAccelerometer()
{
    int accelerometerAxes[3]; // [0]=X [1]=Y [2]=XZ
    accelerometerGyroscopeSensor->resetStepCounter();
    accelerometerGyroscopeSensor->getXAxes(accelerometerAxes);
    return accelerometerAxes;
}
