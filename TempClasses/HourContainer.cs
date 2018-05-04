using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class HourContainer
    {
        private float temperature;
        private DateTime timeStamp;

        public float Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        public HourContainer(float temp, DateTime tim)
        {
            Temperature = temp;
            TimeStamp = tim;
        }
    }

    




}
