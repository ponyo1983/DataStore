using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DataStorage.Analog
{
    class DataStoreBlock:IDataProcessBlock
    {
        public const int MaxPoint = 5 * 60; //5分钟的缓存

        ManualResetEvent finishedEvent = new ManualResetEvent(true);
        FileAnalog fileAanlog;
        
        byte[] dataBuffer=new byte[8*MaxPoint];

        DateTime prevTime;
        IndexRecord indexRecord;
        public DataStoreBlock(int index,FileAnalog analogFile)
        {
            this.Index = index;
            this.fileAanlog = analogFile;
        }

        public int Index
        {
            get;
            private set;
        }
        public int PointCount
        {
            get;
            private set;
        }

        public bool Wait(int milli)
        {
            if (milli < 0)
            {
                return finishedEvent.WaitOne();
            }
           return finishedEvent.WaitOne(milli);
        }
        public void Clear()
        {
            this.PointCount = 0;
            finishedEvent.Reset();
        }

        public void Store()
        {
            if (this.PointCount > 0)
            {
                fileAanlog.DataFile.Store(this.IndexRecord, dataBuffer, this.PointCount * 8);
                fileAanlog.IndexFile.Store(this.IndexRecord);
            }
            finishedEvent.Set();
            fileAanlog.PutBlock(this);
        }


        public IndexRecord IndexRecord
        {
            get
            {
                return indexRecord;
            }
            set
            {
                this.indexRecord = value;
                this.prevTime = value.EndTime;
            }
        }


        private void AddValue(int index, DateTime time, float value, byte digit)
        {
            int mills = (int)((time - time.Date).TotalMilliseconds);
            byte[] timeByte =BitConverter.GetBytes((int)((mills << 4) | (digit & 0x0f)));
            Array.Copy(timeByte, 0, dataBuffer, index * 8, 4);
            byte[] analogByte = BitConverter.GetBytes(value);
            Array.Copy(analogByte, 0, dataBuffer, index * 8+4, 4);

            this.indexRecord.RealLength = this.indexRecord.RealLength + 8;
        }

        /// <summary>
        /// 添加模拟量
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns>>=0为加入的模拟量的个数,-1:日期变化,-2:时间回溯</returns>
        public int AddAnalog(DateTime time, float value, byte digit)
        {
            if (indexRecord.BeginTime == DateTime.MinValue)
            {
                indexRecord.BeginTime = time;
                AddValue(0, time, value, digit);
            }
            else
            {
                if ((prevTime - time).TotalSeconds > 30) return -2; //先判断时间回溯
                if (prevTime.Date != time.Date) return -1;
                
                AddValue(this.PointCount, time, value, digit);
            }
            prevTime = time;
            PointCount++;
            indexRecord.EndTime = time;
            return this.PointCount;
        }




        #region IDataProcessBlock 成员

        public void Process()
        {
            Store();
        }

        #endregion
    }
}
