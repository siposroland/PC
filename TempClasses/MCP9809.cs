
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace HelloWorld
{
    // MCP9808 temperature sensor class
    public sealed class MCP9808Device : IDisposable
    {
        private  I2cDevice Sensor { get; set; }
        private double Temperature { get; set; }


        #region MCP9808 addresses

        //  Adress Code: 0011
        //  Slave Adress: A2 A1 A0 -> 0 0 0
        //  Read bit: 1
        //  Write bit: 0

        //  Main adress: 0 + Adress Code + Slave Adress
        //  0001 1000 -> 0x18
        private const int ADDRESS_MAIN = 0x18;
       
        //  Read Adress: Adress Code + Slave Adress + Read bit
        // 0011 0001 -> 0x31
        private const int ADDRESS_READ = 0x31;

        //  Write Adress: Adress Code + Slave Adress + Write bit
        // 0011 0000 -> 0x30
        private const int ADDRESS_WRITE = 0x30;

        #endregion

        #region Register List
        // 16 bits registers (MSB, LSB)

        // RFU, Reserved for Future Use (Read-Only register)
        private const byte REGISTER_RFU = 0x00;

        // Configuration register(CONFIG)
        private const byte REGISTER_CONFIG = 0x01;

        // Alert Temperature Upper Boundary Trip register (TUPPER)
        private const byte REGISTER_TUPPER = 0x02;

        // Alert Temperature Lower Boundary Trip register (TLOWER)
        private const byte REGISTER_TLOWER = 0x03;

        // Critical Temperature Trip register (TCRIT)
        private const byte REGISTER_TCRIT = 0x04;

        // Temperature register (TA)
        private const byte REGISTER_TEMPERATURE = 0x05;

        // Manufacturer ID register
        private const byte REGISTER_MANUFACTURE_ID = 0x06;

        // Device ID/Revision register
        private const byte REGISTER_DEVICE_ID = 0x07;

        // Resolution register
        private const byte REGISTER_RESOLUTION = 0x08;

        #endregion

        #region Parameters of the UPPER Configuration Register 

        // THYST: TUPPER and TLOWER Limit Hysteresis bits (2. and 1. bit)
        // This bit can not be altered when either of the Lock bits are set (bit 6 and bit 7).
        // This bit can be programmed in Shutdown mode.

        // Set Limit of Hysteresis 0 celsius degree (default)
        private const byte CONFIG_UPPER_HYSTERESIS_00 = 0x00;

        // Set Limit of Hysteresis 1,5 celsius degree
        private const byte CONFIG_UPPER_HYSTERESIS_15 = 0x02;

        // Set Limit of Hysteresis 3 celsius degree
        private const byte CONFIG_UPPER_HYSTERESIS_3 = 0x04;

        // Set Limit of Hysteresis 6 celsius degree
        private const byte CONFIG_UPPER_HYSTERESIS_6 = 0x06;

        // SHDN: Shutdown Mode (0. bit)
        // In shutdown, all power-consuming activities are disabled, 
        // though all registers can be written to or read.
        // This bit cannot be set to ‘1’ when either of the Lock bits is set(bit 6 and bit 7). 
        // However, it can be cleared to ‘0’ for continuous conversion while locked

        // Shutdown (Low-Power mode)
        private const byte CONFIG_UPPER_SHDN_SHUTDOWN = 0x01;

        // Continuous conversion (power-up default)
        private const byte CONFIG_UPPER_SHDN_CONTINUOUS = 0x00;

        #endregion

        #region Parameters of the LOWER Configuration Register 

        // Crit. Lock: TCRIT Lock bit (7. bit)
        // When enabled, this bit remains set to ‘1’ or locked until cleared by an internal Reset 
        // This bit can be programmed in Shutdown mode.

        //  Unlocked. TCRIT register can be written (power-up default)
        private const byte CONFIG_LOWER_CRITLOCK_UNLOCKED = 0x00;

        //  Locked. TCRIT register can not be written
        private const byte CONFIG_LOWER_CRITLOCK_LOCKED = 0x80;

        // Win. Lock: TUPPER and TLOWER Window Lock bit (6. bit)
        // When enabled, this bit remains set to ‘1’ or locked until cleared by a Power-on Reset 
        // This bit can be programmed in Shutdown mode

        // Unlocked. TUPPER and TLOWER registers can be written (power-up default)
        private const byte CONFIG_LOWER_WINLOCK_UNLOCKED = 0x00;

        // Locked. TUPPER and TLOWER registers can not be written
        private const byte CONFIG_LOWER_WINLOCK_LOCKED = 0x40;

        // Int. Clear: Interrupt Clear bit (5. bit)
        // This bit can not be set to ‘1’ in Shutdown mode, but it 
        // can be cleared after the device enters Shutdown mode.

        // No effect (power-up default)
        private const byte CONFIG_LOWER_INTERRUPT_NOT_CLEAR = 0x00;

        // Clear interrupt output; when read, this bit returns to ‘0’
        private const byte CONFIG_LOWER_INTERRUPT_CLEAR = 0x20;

        // Alert Stat.: Alert Output Status bit (4. bit)
        // This bit can not be set to ‘1’ or cleared to ‘0’ in Shutdown mode. 
        // However, if the Alert output is configured as Interrupt mode, and 
        // if the host controller clears to ‘0’, the interrupt, using bit 5 while the device
        // is in Shutdown mode, then this bit will also be cleared ‘0’.

        // Alert output is not asserted by the device (power-up default)
        private const byte CONFIG_LOWER_ALERT_NOT_ASSERTED = 0x00;

        // Alert output is asserted as a comparator/Interrupt or critical temperature output
        private const byte CONFIG_LOWER_ALERT_ASSERTED = 0x10;

        // Alert Cnt.: Alert Output Control bit (3. bit)
        // This bit can not be altered when either of the Lock bits are set (bit 6 and bit 7).
        // This bit can be programmed in Shutdown mode, but the Alert output will not assert or deassert.

        // Alert Output Control Disabled (Default)
        private const byte CONFIG_LOWER_ALERT_OUTPUT_DISABLED = 0x00;

        // Alert Output Control Enabled
        private const byte CONFIG_LOWER_ALERT_OUTPUT_ENABLED = 0x08;

        // Alert Sel.: Alert Output Select bit (2. bit)
        // When the Alarm Window Lock bit is set, this bit cannot be altered until unlocked(bit 6).
        // This bit can be programmed in Shutdown mode, but the Alert output will not assert or deassert.

        // Alert output for TUPPER, TLOWER and TCRIT (power-up default)
        private const byte CONFIG_LOWER_ALERT_OUTPUT_TUPPER_TLOWER_TCRIT = 0x00;

        // TA > TCRIT only (TUPPER and TLOWER temperature boundaries are disabled)
        private const byte CONFIG_LOWER_ALERT_OUTPUT_TCRIT_ONLY = 0x04;

        // Alert Pol.: Alert Output Polarity bit (1. bit)
        // This bit can not be altered when either of the Lock bits are set (bit 6 and bit 7).
        // This bit can be programmed in Shutdown mode, but the Alert output will not assert or deassert.

        // Active-low (power-up default; pull-up resistor required) 
        private const byte CONFIG_LOWER_ALERT_POLARITY_LOW = 0x00;

        // Active-high
        private const byte CONFIG_LOWER_ALERT_POLARITY_HIGH = 0x02;

        // Alert Mod.: Alert Output Mode bit (0. bit)
        // This bit can not be altered when either of the Lock bits are set (bit 6 and bit 7).
        // This bit can be programmed in Shutdown mode, but the Alert output will not assert or deassert.

        // Comparator output (power-up default)
        private const byte CONFIG_LOWER_ALERT_MODE_COMPARATOR = 0x00;

        // Interrupt output
        private const byte CONFIG_LOWER_ALERT_MODE_INTERRUPT = 0x01;
        #endregion



        //Default constructor with slave address
        public MCP9808Device()
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

        // Read the ambient temperature register
        public double GetTemperatureValue()
        {
            byte[] tempBuffer = new byte[2];
            byte[] writeBuffer = new byte[1];
            double temperature = 0;

            // Send the adress of the readable Temperature Register 
            // then read 2 bytes to tempBuffer, this contains actual temperature value
            writeBuffer = new byte[] { REGISTER_TEMPERATURE };
            Sensor.WriteRead(writeBuffer, tempBuffer);
            SpinWait.SpinUntil(() => false, 30);

            // Byte array to temperature value
            temperature = this.calculateTemperature(tempBuffer);

            return temperature;
        }

        // Read device ID
        public int GetDeviceID()
        {
            byte[] devIdBuffer = new byte[2];
            byte[] writeBuffer = new byte[1];
            int deviceID = 0;

            // Send the adress of the readable Device ID register
            // then read 2 bytes to devIDBuffer, this contains ID value
            writeBuffer = new byte[] { REGISTER_DEVICE_ID };
            Sensor.WriteRead(writeBuffer, devIdBuffer);


            // Byte array to ID value (decimal)
            deviceID = (int)this.calculateDeviceID(devIdBuffer);

            return deviceID;
        }

        private double calculateTemperature(byte[] tempBuffer)
        {
            double temperature = 0;

            // convert binary to temeperature (celsius)
            // interval of bit0 -> bit11 contains values
            // the Upper and Lower registers is exchanged under the read procedure

            // byte array to "bool" array
            BitArray bits = new BitArray(tempBuffer);

            // handle the "Lower register" (bit8-15 interval)
            // datasheet: bit8 multiplier is 2^-4 ... bit15 multiplier is 2^3
            for (int i=8; i<=15; i++)
            {
                int value = 0;
                value = bits[i] ? 1 : 0;
                temperature += value * Math.Pow(2, (i-12));
            }

            // handle the "Upper register" (bit0-7 interval)
            // contains only 4 bits of temperature value
            // datasheet: bit0 multiplier is 2^4 ... bit3 multiplier is 2^8
            for (int i = 0; i <= 3; i++)
            {
                int value = 0;
                value = bits[i] ? 1 : 0;
                temperature += value * Math.Pow(2, (i+4));
            }

            // add sign, it defined by the bit4 (Upper register bit12)
            temperature *= bits[4] ? (-1) : 1;
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
            for (int i = 0; i <= 7; i++)
            {
                int value = 0;
                value = bits[i] ? 1 : 0;
                deviceID += value * Math.Pow(2, (i));
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
