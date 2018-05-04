using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class DailyContainer
    {
        private static int numberOfTemplates = 0;
        private string name;
        public List<HourContainer> HourValues = new List<HourContainer>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static int NumberOfTemplate
        {
            get {return numberOfTemplates; }
        }


        public DailyContainer(string name)
        {
            this.Name = name;
            numberOfTemplates++;
        }
    }

    




}
