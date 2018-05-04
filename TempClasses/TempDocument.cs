using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace HelloWorld
{
    class TempDocument
    {

        public List<TempContainer> TempValues = new List<TempContainer>();
        public List<DailyContainer> DailyValues = new List<DailyContainer>();

        private string savePath;
        private int pageNumber;
        private double tempAvg = 0;

        private TimeSpan preTimerSettings;
        private TimeSpan pendingTimerSettings;
        private TimeSpan afterTimerSettings;
        private Boolean isError;
        private int codeError;
        private double preValueSettings;
        private double afterValueSettings;

        public int PageNumber
        {
            set { pageNumber = value; }
            get { return pageNumber; }
        }

        public double TempAvg
        {
            set { tempAvg = value; }
            get { return tempAvg; }
        }

        private static TempDocument tempDocument = new TempDocument("2",0);
        private HeatControl heatController = new HeatControl();

        public static TempDocument Instance
        {
            get { return tempDocument; }
        }

        private DispatcherTimer timer_MCP9808;
        private DispatcherTimer timer_TMP007;
        private DispatcherTimer timer_HEAT;

        // LED Gpio init values
        GpioPin pin;
        GpioPinValue pinValue;
        public Boolean Heat_State = false;
        const int LED_PIN = 6;

        public double temperatureMCP9808 = 0;

        // Sensors class init
        public MCP9808Device SensorTemp = new MCP9808Device();

        public void testSensor() { 
            int devID = 0;
            devID = SensorTemp.GetDeviceID();
            System.Diagnostics.Debug.WriteLine("The temperature device id is: " + Convert.ToString(devID));
            tempDocument.SavePath = "3";
        }

        public void initTimers()
        {
            timer_MCP9808 = new DispatcherTimer();
            timer_MCP9808.Interval = TimeSpan.FromSeconds(5);
            timer_MCP9808.Tick += Timer_MCP9808;

            timer_TMP007 = new DispatcherTimer();
            timer_TMP007.Interval = TimeSpan.FromSeconds(5);
            timer_TMP007.Tick += Timer_TMP007;

            timer_HEAT = new DispatcherTimer();
            timer_HEAT.Interval = TimeSpan.FromSeconds(10);
            timer_HEAT.Tick += Timer_HEAT;

            InitGPIO();

            timer_MCP9808.Start();
            timer_TMP007.Start();
            timer_HEAT.Start();

        }


        public string SavePath
        {
            get { return savePath; }
            set { savePath = value; }
        }

        public TempDocument(string save, int pN)
        {
            this.SavePath = save;
            this.PageNumber = pN;
            TempValues.Add(new TempContainer(20, 21, 21, new DateTime(2018, 5, 1, 4, 10, 0), false, 0, false));
            TempValues.Add(new TempContainer(22, 23, 20, new DateTime(2018, 5, 1, 4, 10, 5), false, 0, false));
            TempValues.Add(new TempContainer(22, 21, 21, new DateTime(2018, 5, 1, 4, 10, 10), false, 0, false));
            TempValues.Add(new TempContainer(22, 23, 22, new DateTime(2018, 5, 1, 4, 10, 15), false, 0, false));
            TempValues.Add(new TempContainer(20, 22, 23, new DateTime(2018, 5, 1, 4, 10, 20), false, 0, false));
        }

        // blink led to check the program is running
        private void Timer_HEAT(object sender, object e)
        {
            heatController.ControlHeat(new TimeSpan(1, 30,0),
                new TimeSpan(1,30,0),
                new TimeSpan(0, 30,0),
                1,3);
        }

        public void InvHeatState()
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }


        // read ambient temperature values and write to the Debug Output
        private void Timer_MCP9808(object sender, object e)
        {
            temperatureMCP9808 = SensorTemp.GetTemperatureValue();
            System.Diagnostics.Debug.WriteLine("The TEMP temperature is: " + Convert.ToString(temperatureMCP9808));
            if(0 == PageNumber)
            {
                MainPage.Instance.UpdateMCP9809(temperatureMCP9808);
            }
            
        }

        // read object temperature values and write to the Debug Output
        private void Timer_TMP007(object sender, object e)
        {
            double temperature = 0;
            //temperature = sensorIR.GetObjectTemperatureValue();
            System.Diagnostics.Debug.WriteLine("The IR temperature is: " + Convert.ToString(temperature));
        }


        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }


    }
}
