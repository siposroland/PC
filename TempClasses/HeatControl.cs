using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class HeatControl
    {
        private DateTime heatIsOnTime;
        private DateTime requiredStartTime;
        private DateTime requiredEndTime;
        private int requiredTemperature;


        public HeatControl()
        {
            requiredStartTime = new DateTime(2018, 05, 04, 10, 30, 0);
            requiredEndTime = new DateTime(2018, 05, 04, 11, 30, 0);
            requiredTemperature = 30;
            heatIsOnTime = requiredEndTime;
        }

        public void ControlHeat(TimeSpan preTimerSettings,
                            TimeSpan pendingTimerSettings,
                            TimeSpan afterTimerSettings,
                            double preValueSettings,
                            double afterValueSettings)
        {
            if (!TempDocument.Instance.Heat_State)// if heat is off
            {
                if ((requiredStartTime - preTimerSettings) < DateTime.UtcNow)
                {
                    if ((TempDocument.Instance.temperatureMCP9808 + preValueSettings) > requiredTemperature)
                    {
                        TempDocument.Instance.Heat_State = true;
                        heatIsOnTime = DateTime.UtcNow;
                        TempDocument.Instance.InvHeatState();
                    }
                }

            }
            else
            {
                if ((heatIsOnTime + pendingTimerSettings) > DateTime.UtcNow)
                {
                    if ((TempDocument.Instance.temperatureMCP9808 - afterValueSettings) > requiredTemperature)
                    {
                        TempDocument.Instance.Heat_State = false;
                        TempDocument.Instance.InvHeatState();
                    }
                    if ((requiredEndTime + afterTimerSettings) < DateTime.UtcNow)
                    {
                        TempDocument.Instance.Heat_State = false;
                        TempDocument.Instance.InvHeatState();
                    }
                }




            }

        }



    }

    




}
