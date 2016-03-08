using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using NetSocket;

using System.Windows.Forms;
using System.Drawing;
using switch_win;

namespace NetSocket
{
    

    public class Communication
    {

       
        /// <summary>
        /// ////////////////////
        /// 
        /// 
        
        /// ////////////////////////////////////////////
        public static Socket ClientSocket;
        public static IPEndPoint ServerInfo;
        public static ulong ulAnsy = 0;
        public static ulong uldefAnsy = 0;
        public static bool bConnected = false;
        public static int i = 0;
        public static int cc = 0;
        public static String packetvalue="";
        public static String typess="";
        public static int two_to_ten = 0;
        public static int size;
        public static SocketFlags socketFlags;
        public static Boolean canwrite=false;       
        public int[] values = new int[12];
        public static  Thread t;

  


        Boolean willbeover = false;

        public static Boolean flag = false;

        public static String SN_NO;
        public static String TYPE;
        
        private delegate void delegate_chcounter(int _chount);

        private delegate void delegate_current();
        delegate_current delegate_current_handler;
        private void disconnect()
        {
            dad.disconnect();
        }
        private delegate void delegate_log(string value);
        delegate_log delegate_log_handler;
        private void setlog(string s)
        {
            char[] cc = { '\r', '\n', '\0' };
            string[] sub_s = s.Split(cc, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sub_s.Length; i++)
                dad.txt_log.AppendText(DateTime.Now.ToString() + ":>Receive:" + sub_s[i] + "\r\n");
        }
        TimeOutSocket IOSocket = new TimeOutSocket();

        Form1 dad;
        public Communication(Form1 _dad)
        {
            dad = _dad; 
        }

        //===========================关闭连接==========================================
        public bool closeConnect()
        {
            
            try
            {
                willbeover = true;
                Thread.Sleep(100);
                sendvalue("end");
                Thread.Sleep(100);
                t.Join();
                t.Abort();
                IOSocket.tcpclient.Close();
                bConnected = false;
                willbeover = false;
                bconnected = false;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Closing the Connect Failed: "+e.Message);
                LogContext.theInst.LogPrint("Closing the Connect Failed: " + e.ToString(), 2);
                return false;
            }
        }
        private delegate void delegate_handler_showbutton();


        public bool Check_value(string senddata, string checkdata)
        {
            try
            {
                int times = 5;
                IOSocket.tcpclient.Client.Send((Encoding.ASCII.GetBytes(senddata + "\r\n")));
                string receivedata = "";
                byte[] receivebyte = new byte[1024];
                for (int i = 0; i < times; i++)
                {
                    IOSocket.tcpclient.Client.ReceiveTimeout = 1000;
                    IOSocket.tcpclient.Client.Receive(receivebyte);
                    receivedata = Encoding.Default.GetString(receivebyte);
                    IOSocket.tcpclient.Client.ReceiveTimeout = 0;
                    if (receivedata.StartsWith(checkdata))
                    {
                        receivedata=receivedata.Replace("\0","");
                        //
                        char[] cc = { ' ', 'S', 'W', '\r', '\n', ';'};
                        string[] value = receivedata.Split(cc, StringSplitOptions.RemoveEmptyEntries);
                        if (value.Length % 2 == 0)
                        {
                            Global.Ch_counter = value.Length / 2;
                            delegate_handler_showbutton dele_bt = new delegate_handler_showbutton(dad.show_button);
                            dad.BeginInvoke(dele_bt);
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Check Data Error:" + checkdata + "\r\n" + e.Message);
                IOSocket.tcpclient.Client.ReceiveTimeout = 0;
                LogContext.theInst.LogPrint("Check Data Error:" + checkdata + "\r\n" + e.ToString(), 2);
                return false;
            }
        }

        private string get_receive(string data, bool send)
        {
            try
            {

                string receivedata = "";
                byte[] receivebyte = new byte[1024];
                IOSocket.tcpclient.Client.ReceiveTimeout = 1000;
                if (send == true)
                {
                    IOSocket.tcpclient.Client.Send((Encoding.ASCII.GetBytes(data + "\r\n")));
                }
                IOSocket.tcpclient.Client.Receive(receivebyte);
                receivedata = Encoding.Default.GetString(receivebyte);
                IOSocket.tcpclient.Client.ReceiveTimeout = 0;
                return receivedata;
            }
            catch
            {
                //MessageBox.Show("Check Data Error:" + data + "\r\n" + e.Message);
                return "UNKNOWN ERROR";
            }
        }



        public Boolean bconnected = false;
        //=====================================================================
        //********连接到设备 参数 ： IP地址   端口号   延时时间
        public bool connecttoAtt(String IPaddr,int Port,int Timeout)
        {

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            if (true)
            {

                ServerInfo = null;
                try
                {
                    ServerInfo = new IPEndPoint(IPAddress.Parse(IPaddr), Port);
                }
                catch (System.Exception ex)
                {
                    
                    MessageBox.Show(ex.Message, ex.Source);
                    LogContext.theInst.LogPrint(ex.ToString(), 2);
                    ServerInfo = new IPEndPoint(IPAddress.Parse(IPaddr), Port);
                    return false;
                }


            }

            if (bConnected == false)
            {

                try
                {
                    
                    if (IOSocket.Connect(ServerInfo, Timeout))
                    {
                        bConnected = true;
                        //sc = System.Threading.SynchronizationContext.Current;
                    }
                    else
                    {
                        bConnected = false;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Connect Failed,Please Cheack The Network Config","Connect Error");
                    LogContext.theInst.LogPrint(ex.ToString(), 2);
                    return false;
                }
            }
            bconnected = true;
            return true;
        }
        //=====================================================================









        //=====================================================================
        //开始线程 执行监听解包
        public void startthread()
        {
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            t = new Thread(new ThreadStart(listenvalue));
            t.IsBackground = true;
            t.Priority = ThreadPriority.AboveNormal;
            t.Start();
        }
        //=====================================================================







        //=====================================================================
        //发送指令返回通道数目
        public bool getchcounter()
        {
            String str;
            str = "CHA\r\n";
            IOSocket.tcpclient.Client.Send(Encoding.ASCII.GetBytes(str));
            return true;
        }
         //======================================================================

  
            
        






         //=====================================================================
         //截获设备返回值，交由解包方法处理
        bool listen_already_failed = false;
        public void listenvalue()
        {
            IOSocket.tcpclient.Client.ReceiveTimeout = 1000;
            byte[] buffer = new byte[2000];
            while (willbeover == false)
            {
                try
                {
                    IOSocket.tcpclient.Client.Receive(buffer, 2000, 0);
                    readvalue(buffer); //解包方法
                    buffer = new byte[2000];
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.TimedOut)
                    {
                        listenvalue();
                    }
                    else
                    {
                        if (!listen_already_failed)
                        {
                            listen_already_failed = true;
                            MessageBox.Show("Failed to listen the connection:" + se.Message, se.Source);
                            LogContext.theInst.LogPrint("Failed to listen the connection:" + se.Message, 2);
                            delegate_current_handler = new delegate_current(disconnect);
                            dad.BeginInvoke(delegate_current_handler);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!listen_already_failed)
                    {
                        listen_already_failed = true;
                        MessageBox.Show("Failed to listen the connection:" + e.Message, e.Source);
                        LogContext.theInst.LogPrint("Failed to listen the connection:" + e.Message, 2);
                        delegate_current_handler = new delegate_current(disconnect);
                        dad.BeginInvoke(delegate_current_handler);
                    }
                }
            }
        }
         //=====================================================================
       
         int[] c_catch_value = new int[60000];
         String[] data_channel_name = new String[10];
         String str = "";
         public static String valuestr = "";


        

        

        
       

       
         //=====================================================================
        //解包函数 参数为buffer字符
        public void readvalue(byte[] buffer)
         {
            string receive_data = ASCIIEncoding.ASCII.GetString(buffer);
            LogContext.theInst.LogPrint(receive_data.Replace("\0", ""), 0);
            delegate_log_handler = new delegate_log(setlog);
            dad.BeginInvoke(delegate_log_handler, receive_data);
            int current_count = 4;
            int channel_no = 0;
            int[] c_catch_value = new int[60000];
            String[] data_channel_name = new String[10];

            current_count = 4;
            current_count = 4;
            //===============解包错误信息========================
            if (buffer[0] == 'S' && buffer[1] == 'Y' && buffer[2] == 'N')
            {
                return;
            }

            //===============================解包SAVE===================
            if (buffer[0] == 'S' && buffer[1] == 'a' && buffer[2] == 'v' && buffer[3]== 'e')
            {
                string value = Encoding.Default.GetString(buffer);
                string[] split1 = value.Split(' ');
                if (split1[1].StartsWith("e"))
                {
                    Global.saved = true;
                    dad.button5.BackColor = Color.LightGreen;
                }
                else if (split1[1].StartsWith("d"))
                {
                    Global.saved = false;
                    dad.button5.BackColor = Color.LightPink;
                }

                return;
            }



            //==================解包ST ============================
            if (buffer[0] == 'S' && buffer[1] == 'T')
            {
                channel_no = 0;
                current_count = 3;
                while (buffer[current_count] != 13)
                {

                    channel_no *= 10;
                    channel_no += ((buffer[current_count]) - 48);
                    current_count++;
                }

                i = 0;
                str = "";
                str = Convert.ToString(channel_no, 2);
                for (int m = 0; m < 8; m++)
                {
                    values[m] = 0;
                }


                for (int ac = 0; ac < str.Length; ac++)
                {
                    values[str.Length - ac - 1] = int.Parse(str[ac].ToString());
                }

                for (int p = 0; p < 8; p++)
                {
                    if (values[p] != 0)
                    {
                        dad.Global_value.Sw_now[p] = true;

                        //vvas.ppc[p].Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { vvas.ppc[p].Source = pon; }));
                    }
                    else
                    {
                       dad.Global_value.Sw_now[p] = false;
                        //vvas.ppc[p].Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { vvas.ppc[p].Source = poff; }));
                    }
                }
                dad.Global_value.set_value();
            }


            //==========================解包PHA=================================

            if (buffer[0] == 'S' && buffer[1] == 'W' && buffer[2] == ' ')
            {
                current_count = 3;
                while (buffer[current_count] != 13)
                {
                    channel_no = 0;
                    while (buffer[current_count] != 32)
                    {
                        channel_no *= 10;
                        channel_no += ((buffer[current_count]) - 48);
                        current_count++;
                    }
                    current_count++;
                    // MessageBox.Show("channel_no="+channel_no.ToString());
                    while (buffer[current_count] != 59 && buffer[current_count] != 13)
                    {
                        c_catch_value[channel_no] *= 10;
                        c_catch_value[channel_no] += ((buffer[current_count]) - 48);
                        current_count++;
                        if (buffer[current_count] == 13 || buffer[current_count] == 59)
                        {

                            if (c_catch_value[channel_no] <= 1 && c_catch_value[channel_no] >= 0)
                            {
                                if (channel_no >= 1 && channel_no <= Global.Ch_counter)
                                {
                                    if (c_catch_value[channel_no] != 0)
                                    {
                                        dad.Global_value.Sw_now[channel_no-1] = true;

                                        //vvas.ppc[p].Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { vvas.ppc[p].Source = pon; }));
                                    }
                                    else
                                    {
                                        dad.Global_value.Sw_now[channel_no-1] = false;
                                        //vvas.ppc[p].Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { vvas.ppc[p].Source = poff; }));
                                    }
                                    dad.Global_value.set_value(); 
                                }
                            }
                        }
                    }
                    if (buffer[current_count] != 13)
                    {
                        current_count += 4;
                    }
                    else
                    {

                    }

                    for (int u = 0; u < 59; u++)
                    {
                        c_catch_value[u] = 0;
                    }

                }

            }
            
        }
        //=====================================================================









        //=====================================================================
        //通过socket发送开关参数 ： 指令字符串 (不用加入0D0A)

        public bool sendSWvalue(int channel_no, int SWvalue)
        {
            String str;
            str = "SW " + channel_no.ToString() + " " + SWvalue.ToString() + "\r\n";
            try
            {
                IOSocket.tcpclient.Client.Send(Encoding.ASCII.GetBytes(str));

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                LogContext.theInst.LogPrint(ex.ToString(), 2);
                return false;
            }
            return true;
        }


        
        //=====================================================================
        //通过socket发送衰减器参数 ： 指令字符串 (不用加入0D0A)

        public void sendALLSTvalue()
        {
            packetvalue = "";
            try
            {
                for (int i = 0; i <Global.Ch_counter; i++)
                {
                    packetvalue += (dad.Global_value.Sw_now[Global.Ch_counter - 1 - i] == true ? 1 : 0).ToString();
                }
                two_to_ten = Convert.ToInt32(packetvalue, 2);

                packetvalue = "";
                packetvalue = "ST ";
                packetvalue += Convert.ToString(two_to_ten, 10);
                packetvalue += "\r\n";

                IOSocket.tcpclient.Client.Send(Encoding.ASCII.GetBytes(packetvalue));
                packetvalue = "";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                LogContext.theInst.LogPrint(ex.ToString(), 2);
            }

        }


        //=====================================================================
        //通过socket发送指令 参数 ： 指令字符串 (不用加入0D0A)
        public bool sendvalue(String value)
        {
            value += "\r\n";
            try
            {
                IOSocket.tcpclient.Client.Send(Encoding.ASCII.GetBytes(value));
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                LogContext.theInst.LogPrint(ex.ToString(), 2);
                return false;
            }
            return true;
        }

        //=====================================================================
    }
}
