using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DataStorage.Analog
{
    class DataRequestBlock:IDataProcessBlock
    {

        FileAnalog fileAnalog;
        ManualResetEvent finishedEvent = new ManualResetEvent(false);

        List<List<AnalogPoint>> listPoit=new List<List<AnalogPoint>>();

         DateTime timeBegin;
         DateTime timeEnd;


         public DataRequestBlock(FileAnalog fileAnalog,DateTime timeBegin,DateTime timeEnd)
         {
             this.fileAnalog = fileAnalog;
             if (timeBegin < timeEnd)
             {
                 this.timeBegin = timeBegin;
                 this.timeEnd = timeEnd;
             }
             else
             {
                 this.timeBegin = timeEnd;
                 this.timeEnd = timeBegin;
             }
         }


        #region IDataProcessBlock 成员

        public void Process()
        {
            IList<IndexRecord> indexAll = fileAnalog.IndexFile.AllIndex;
            for (int i = indexAll.Count; i >= 0; i--)
            {
                IndexRecord recordIndex=indexAll[i];
                if (recordIndex.IsValid == false) continue;
                if (recordIndex.BeginTime > this.timeEnd) continue;
                if (recordIndex.EndTime > this.timeBegin) continue;

            }
        }

        #endregion
    }
}
