using System;
using System.Collections.Generic;
using System.Text;

namespace DataStorage.Analog
{
    struct AnalogPoint
    {
        DateTime time;
        float analogValue;
        byte digitValue;

        public DateTime Time
        {
            get { return time; }
        }

        public float AnalogValue
        {
            get { return analogValue; }
        }

        public byte DigitValue
        {
            get { return digitValue; }
        }
    }
}
