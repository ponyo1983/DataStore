using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataStorage.Analog
{
    class FileData
    {
        public const int IncreaseSize = 1024 * 1024;
        FileStream fileStream = null;
        FileAnalog fileAnalog;

        public FileData(FileAnalog analog)
        {
            this.fileAnalog = analog;
            try
            {
                string dataName = Path.Combine(analog.DataManager.StoreDir, analog.Type.ToString("X2") + "H-" + analog.Index.ToString("000") + ".dat");
                fileStream = new FileStream(dataName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch
            {
                fileStream = null;
            }
        }
        long prevOffset = 0;
        public void Store(IndexRecord index, byte[] data, int length)
        {
            long offset = index.BeginOffset;
            long fileSize=fileStream.Length;
            long needSize = offset + index.RecordLength + length;
            long wrPos = (index.RecordLength + offset)%fileAnalog.MaxFileSize;
            if (needSize <= fileAnalog.MaxFileSize)//直接写入
            {
                if (fileSize < needSize)
                {
                    fileStream.SetLength(fileSize + IncreaseSize);
                }
                fileStream.Position = wrPos;
                fileStream.Write(data, 0, length);
                fileStream.Flush();
            }
            else
            {
            
                long leftSize = fileSize - wrPos;

                int size1 = (int)(leftSize < length ? leftSize : length);
                int size2 = length - size1;

                if (size1 > 0)
                {
                    fileStream.Position = wrPos;
                    fileStream.Write(data, 0, size1);
                }
                if (size2 > 0)
                {
                    fileStream.Position = 0;
                    fileStream.Write(data, size1, size2);
                }
                fileStream.Flush();
            }

            index.RecordLength = index.RecordLength + length;

        }

    }
}
