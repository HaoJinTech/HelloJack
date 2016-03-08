using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace switch_win
{
    public  class Global
    {
       
        public  Boolean shouldchange = true;
        public static bool saved = true;
        private static int ch_counter = 4;
        public static int Ch_counter
        {
            get { return ch_counter; }
            set { ch_counter = value; }
        }

        Form1 dad;
        public Global(Form1 _dad)
        {
            
            dad = _dad;
        }
       
         public String ipaddr = "192.168.1.18";
         public int port = 50000;
         public string[] localaccess = new string[64];
         public bool Adaccess = false;
         public int type = 0;

         private bool[] sw_now = new bool[64];

         public bool[] Sw_now
         {
             get { return sw_now; }
             set 
             { 
                 sw_now = value;
                 set_value();
             }
         }


         public void set_value()
         {
          //   this.Invoke((EventHandler)(delegate
            // {
                 //判断是否是显示为16禁止  

                 for (int i = 0; i < ch_counter; i++)
                 {
                     if (sw_now[i] == true)
                     {
                         dad.button_ch[i].Text = "Channel " + (i + 1).ToString() + "\r\n" + "ON";
                         dad.button_ch[i].BackColor = Color.LightGreen;
                     }
                     else
                     {
                         dad.button_ch[i].Text = "Channel " + (i + 1).ToString() + "\r\n" + "OFF";
                         dad.button_ch[i].BackColor = Color.Salmon;
                     }
                 }

            // }));   
         }

    }
}
