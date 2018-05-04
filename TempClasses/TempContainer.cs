using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class TempContainer
    {
        private float tempIRObject;
        private float tempIRAmbient;
        private float tempMCPAmbient;
        private DateTime timeStamp;
        private Boolean isError;
        private int errorCode;
        private Boolean isHeatOn;

        public float TempIRObject
        {
            get { return tempIRObject; }
            set { tempIRObject = value; }
        }
        public float TempIRAmbient
        {
            get { return tempIRAmbient; }
            set { tempIRAmbient = value; }
        }
        public float TempMCPAmbient
        {
            get { return tempMCPAmbient; }
            set { tempMCPAmbient = value; }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
        public Boolean IsError
        {
            get { return isError; }
            set { isError = value; }
        }
        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
        public Boolean IsHeatOn
        {
            get { return isHeatOn; }
            set { isHeatOn = value; }
        }

        public TempContainer(float tiro, float tira, float tma, DateTime tim, Boolean ier, int erc, Boolean iho)
        {
            TempIRAmbient = tiro;
            TempIRObject = tira;
            TempMCPAmbient = tma;
            TimeStamp = tim;
            IsError = ier;
            ErrorCode = erc;
            IsHeatOn = iho;
        }
    }

    




}
