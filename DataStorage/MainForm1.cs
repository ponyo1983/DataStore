using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using DataStorage.Analog;

namespace DataStorage
{
    public partial class MainForm1 : Form
    {
        Stopwatch sw = new Stopwatch();
        long prevTick = 0;
        public MainForm1()
        {
            InitializeComponent();

            sw.Start();
    
           
        }


        private void TimerCallback(object state)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));

            long ticks = sw.ElapsedMilliseconds;

            Console.WriteLine(ticks-prevTick);
            prevTick = ticks;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            AnalogDataManager manager = AnalogDataManager.GetInstance();
            manager.StoreDir = @"d:\Data";
            manager.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            AnalogDataManager manager = AnalogDataManager.GetInstance();
            DateTime timeBase = DateTime.Now.Date;
            for (int i = 0; i < 200*24*3600; i++)
            {
                manager.AddData(0x11, 1, (byte)(i & 0xff), i, timeBase.AddSeconds(i));
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AnalogDataManager.GetInstance().FlushAll();

            
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] hexstr = textBox1.Text.Split(new char[] {' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte data = 0;
            UInt32 unix = 0;
            for (int i = 0; i < 4; i++)
            {
                data = 0;
                if (i < hexstr.Length)
                {
                    data = byte.Parse(hexstr[i], System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                unix |= (UInt32)(((data) << (8 * i)));
            }

            DateTime time = Utility.Unix2DateTime(unix);

            textBox2.Text = time.ToString("yyyy-MM-dd HH:mm:ss");

        }
    }
}
