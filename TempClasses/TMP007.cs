
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace HelloWorld
{
    // TMP007 infrared temperature sensor class
    public sealed class TMP007Device : IDisposable
    {
        private I2cDevice Sensor { get; set; }
        private double Temperature { get; set; }


        #region TMP007 addresses

        //  Adress Code: 10000
        //  Slave Adress:  A1 A0 -> 0 0 
        //  Read bit: 1
        //  Write bit: 0

        //  Main address: default
        private const int ADDRESS_MAIN = 0x40;

        #endregion

        #region Register List
        // 16 bits registers (MSB, LSB)

        // Sensor voltage result register
        private const byte REGISTER_SENSOR_RESULT = 0x00;

        // Local temperature
        private const byte REGISTER_LOCAL_TEMPERATURE = 0x01;

        // Configuration register
        private const byte REGISTER_CONFIGURATION = 0x02;

        // Object temperature result register
        private const byte REGISTER_OBJECT_TEMPERATURE = 0x03;

        // Device ID/Revision register
        private const byte REGISTER_DEVICE_ID = 0x1F;

        #endregion 


        //Default constructor with slave address
        public TMP007Device()
        {
            Init(ADDRESS_MAIN);
        }

        // Init function
        private void Init(int slaveAddress)
        {
            Task_InitSensor(slaveAddress).Wait();
        }

        // Async Init Task
        private async Task Task_InitSensor(int slaveAddress)
        {
            var settings = new I2cConnectionSettings(slaveAddress) { BusSpeed = I2cBusSpeed.FastMode };
            string aqs = I2cDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);
            Sensor = await I2cDevice.FromIdAsync(dis[0].Id, settings);
        }

        public double GetLocalTemperatureValue()
        {
            byte[] tempBuffer = new byte[2];
            byte[] writeBuffer = new byte[1];
            double temperature = 0;


            // Send the adress of the readable Temperature Register 
            // then read 2 bytes to tempBuffer, this contains actual temperature value
            writeBuffer = new byte[] { REGISTER_LOCAL_TEMPERATURE};
            Sensor.WriteRead(writeBuffer, tempBuffer);
            SpinWait.SpinUntil(() => false, 30);

            // Bytes array to temperature value
            //temperature = this.calculateTemperature(tempBuffer);

            return temperature;
        }
        public double GetObjectTemperatureValue()
        {
            byte[] tempBuffer = new byte[2];
            byte[] writeBuffer = new byte[1];
            double temperature = 0;

            // Send the adress of the readable Temperature Register 
            // then read 2 bytes to tempBuffer, this contains actual temperature value
            writeBuffer = new byte[] { REGISTER_OBJECT_TEMPERATURE };
            Sensor.WriteRead(writeBuffer, tempBuffer);
            SpinWait.SpinUntil(() => false, 30);

            // Bytes array to temperature value
            temperature = this.calculateObjectTemperature(tempBuffer);

            return temperature;
        }

        public int GetDeviceID()
        {
            byte[] devIdBuffer = new byte[2];
            byte[] writeBuffer = new byte[1];
            int deviceID = 0;

            // Send the adress of the readable Device ID register
            // then read 2 bytes to devIDBuffer, this contains ID value
            writeBuffer = new byte[] { REGISTER_DEVICE_ID };
            Sensor.WriteRead(writeBuffer, devIdBuffer);


            // Bytes array to temperature value
            deviceID = (int)this.calculateDeviceID(devIdBuffer);

            return deviceID;
        }

        private double calculateObjectTemperature(byte[] tempBuffer)
        {
            double temperature = 0;

            // convert binary to temeperature (celsius)

            BitArray bits = new BitArray(tempBuffer);

            // twos complement numbers to decimal
            // the bit0 and bit1 doesn't contain temp values (masked)
            // bit0 is the validation bit (if setted -> error)
            if (!((tempBuffer[1] & 0x01) == 1))
            {
                temperature = ((tempBuffer[0] * 256 + (tempBuffer[1] & 0xFC)) / 4);
                // sign check
                if (temperature > 8191)
                {
                    temperature -= 16384;
                }
                // multiple with the lowest value
                temperature = temperature * 0.03125;
            }
            // error value
            else { temperature = -999; }
            
            return temperature;
        }

        private double calculateDeviceID(byte[] tempBuffer)
        {
            double deviceID = 0;

            // convert binary to deviceID (decimal)
            // interval of bit0 -> bit7 contains values

            // byte array to "bool" array
            BitArray bits = new BitArray(tempBuffer);

            // convert 8bit binary to decimal
            for (int i = 8; i <= 15; i++)
            {
                int value = 0;
                value = bits[i] ? 1 : 0;
                deviceID += value * Math.Pow(2, (i-8));
            }

            return deviceID;
        }

        // Dispose function
        public void Dispose()
        {
            if (Sensor != null)
            {
                Sensor.Dispose();
                Sensor = null;
            }
        }
    }
}

