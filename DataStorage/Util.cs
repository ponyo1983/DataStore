using System;
using System.Collections.Generic;
using System.Text;

namespace DataStorage
{
    class Utility
    {
        static readonly DateTime TimeBase = new DateTime(1970, 1, 1, 8, 0, 0);
        public static DateTime Unix2DateTime(uint unixTime)
        {
           return TimeBase.AddSeconds(unixTime);
        }
        public static UInt32 DateTime2Unix(DateTime time)
        {
            TimeSpan ts = time - TimeBase;
            return (uint)ts.TotalSeconds;
        }
    }
}
