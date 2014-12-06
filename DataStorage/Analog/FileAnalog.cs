using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataStorage.Analog
{
    class FileAnalog
    {

        const int BlockNum = 2;

        AnalogDataManager dataManager;

        FileData fileData;
        FileIndex fileIndex;

        DataStoreBlock[] blocks = new DataStoreBlock[BlockNum];

        DataStoreBlock currentBlock = null;

        int reserveCount = 0;

        public FileAnalog(AnalogDataManager manager, int type,int index)
        {
            this.dataManager = manager;
            this.Type = type;
            this.Index = index;
            this.MaxFileSize = 100 * 1024 * 1024; //每个模拟量最大保存为100MB个文件
            
            fileIndex = new FileIndex(this);
            string datName = Path.Combine(manager.StoreDir, type.ToString("X2") + "H-" + index.ToString("000") + ".dat");
            fileData = new FileData(this);

            for (int i = 0; i < BlockNum; i++)
            {
                blocks[i] = new DataStoreBlock(i,this);
            }
          
            Random rnd = new Random();

           // this.reserveCount = rnd.Next(DataStoreBlock.MaxPoint);
            currentBlock = NewBlock(null, false);

        }

        public int Type
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public int MaxFileSize
        {
            get;
            private set;
        }

        public AnalogDataManager DataManager
        {
            get { return this.dataManager; }
        }


        public void AddAnalog(DateTime time, float value, byte digit)
        {
            if (currentBlock == null)
            {
                currentBlock = NewBlock(null,false);
            }
            int pointNum = currentBlock.AddAnalog(time, value, digit);
            if (pointNum >= 0)
            {
                if (pointNum + reserveCount >= DataStoreBlock.MaxPoint)
                {
                    this.dataManager.PutDataBlock(currentBlock);
                    currentBlock = NewBlock(currentBlock, false);
                    reserveCount = 0;
                }
            }
            else
            {
                this.dataManager.PutDataBlock(currentBlock);
                currentBlock = NewBlock(currentBlock, true);
                currentBlock.AddAnalog(time, value, digit);
            }

            
        }

        public FileData DataFile
        {
            get { return fileData; }
        }
        public FileIndex IndexFile
        {
            get { return fileIndex; }
        }

      

        private DataStoreBlock NewBlock(DataStoreBlock oldBlock,bool newIndex)
        {
            DataStoreBlock newBlock = null;
            if (oldBlock == null)
            {
                newBlock = blocks[0];
            }
            else
            {
                int blkIndex=(oldBlock.Index + 1) % BlockNum;
                newBlock = blocks[blkIndex];
            }
            if ((oldBlock == null) && (!newIndex))
            {
                newBlock.IndexRecord = fileIndex.LastIndex;
            }
            else if (newIndex)
            {
                newBlock.IndexRecord = fileIndex.NewRecord();
            }
            else
            {
                newBlock.IndexRecord = oldBlock.IndexRecord;
            }
            newBlock.Wait(-1);
            newBlock.Clear();
            return newBlock;
            
        }

        public void Flush()
        {
            this.dataManager.PutDataBlock(currentBlock);
            currentBlock = NewBlock(currentBlock, false);
            reserveCount = 0;
        }

        public List<List<AnalogPoint>> GetAnalogPoint(DateTime timeBegin, DateTime timeEnd)
        {
       
            return null;
        }

    }
}
