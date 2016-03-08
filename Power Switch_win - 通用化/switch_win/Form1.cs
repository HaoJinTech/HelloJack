using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetSocket;
using switch_win;
using System.Threading;

namespace switch_win
{
    public partial class Form1 : Form
    {
        Communication communication;
        public Global Global_value;
        public Button[] button_ch = new Button[12];
        DateTime dt = new DateTime();
        Boolean Use_date = true;
        public Form1()
        {
            Global_value = new Global(this);
            communication = new Communication(this);
            InitializeComponent();
        }

        public void disconnect()
        {
            groupBox_connect.Enabled = true;
            groupBox1.Enabled = false;
            p.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }
        private void fileFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Global_value.ipaddr = textBox_ipaddr.Text;
                Global_value.port = int.Parse(textBox_ipport.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\r\nError:172");
                LogContext.theInst.LogPrint(err.ToString(), 2);
            }
            try
            {
                if (communication.connecttoAtt(Global_value.ipaddr, Global_value.port, 1000))
                {
                    groupBox_connect.Enabled = false;
                    groupBox1.Enabled = true;
                    p.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    communication.Check_value("SW", "SW");
                    Thread.Sleep(50);
                    communication.startthread();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection Failure, Please check the settings");
                LogContext.theInst.LogPrint("Connection Failure, Please check the settings\r\n" + ex.ToString(), 2);
                communication.closeConnect();
            }
            groupBox_switch_control.Enabled = true;
            groupBox2.Enabled = true;
        }


        private void SET_ALL(int no)
        {
            String Str = "SW ";
            for (int i = 0; i < Global.Ch_counter; i++)
            {
                Str += (i + 1).ToString() + " " + no.ToString() + ";";
            }
            communication.sendvalue(Str);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SET_ALL(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SET_ALL(0);
        }

        public void show_button()
        {
            p.Controls.Clear();
            int column_number = Global.Ch_counter / 2;
            int space = 5;
            int basic_weith = (p.Width - space * (column_number + 1)) / column_number;
            int basic_height = 50;
            for (int ym = 0; ym < 2; ym++)
            {
                for (int xm = 0; xm < Global.Ch_counter / 2; xm++)
                {
                    button_ch[(ym) * column_number + xm] = new Button();
                    button_ch[(ym) * column_number + xm].TabIndex = (ym) * 4 + xm;
                    button_ch[(ym) * column_number + xm].Name = "Button_ch_" + ((ym) * 2 + xm);
                    button_ch[(ym) * column_number + xm].Width = basic_weith;
                    button_ch[(ym) * column_number + xm].Height = basic_height;
                    button_ch[(ym) * column_number + xm].Left = space + (space + basic_weith) * xm;
                    button_ch[(ym) * column_number + xm].Top = space + (space + basic_height) * ym;
                    button_ch[(ym) * column_number + xm].Text = "Channel " + ((ym) * 4 + xm).ToString() + "\r\nOFF";
                    button_ch[(ym) * column_number + xm].Show();
                    button_ch[(ym) * column_number + xm].BackColor = Color.Salmon;
                    button_ch[(ym) * column_number + xm].Click += new EventHandler(button_ch_click);
                    p.Controls.Add(button_ch[(ym) * column_number + xm]);
                }
            }
        }


        private void button_ch_click(object sender, EventArgs e)
        {
            setSW((sender as Button).TabIndex);
        }

        public void setSW(int ch)
        {
            if (Global_value.Sw_now[ch] == true)
            {
                Global_value.Sw_now[ch] = false;
            }
            else
            {
                Global_value.Sw_now[ch] = true;
            }
            //communication.sendALLSTvalue();
            communication.sendSWvalue(ch + 1, Global_value.Sw_now[ch] ? 1 : 0);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            groupBox_switch_control.Enabled = false;
            show_button();
        }

        private void closeSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void aboutAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Power Supply Control System\r\nVersion:1.3.0\r\n\r\nDesigned By HJ Communication Technologies ");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://" + Global_value.ipaddr.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            communication.sendvalue("SAVE");
        }

        private void add_task(string date, string time, string cmd)
        {
            dataGridView1.Rows.Add(date, time, cmd);
            Saved_data.getInst.Date_list.Add(date);
            Saved_data.getInst.Time_list.Add(time);
            Saved_data.getInst.Command_list.Add(cmd);
        }
        private void add_task(string time, string cmd)
        {
            dataGridView1.Rows.Add("null", time, cmd);
            Saved_data.getInst.Date_list.Add("null");
            Saved_data.getInst.Time_list.Add(time);
            Saved_data.getInst.Command_list.Add(cmd);
        }
        private void Delete_task(int id)
        {
            Saved_data.getInst.Date_list.RemoveAt(id);
            Saved_data.getInst.Time_list.RemoveAt(id);
            Saved_data.getInst.Command_list.RemoveAt(id);
            dataGridView1.Rows.RemoveAt(id);
        }
        private void timer_sec_run_Tick(object sender, EventArgs e)
        {
            try
            {
                dt = DateTime.Now;
                String now_date = dt.ToLongDateString();
                String now_time = dt.ToLongTimeString();
                List<int> runover = new List<int>();
                for (int m = 0; m < Saved_data.getInst.Command_list.Count; m++)
                {
                    if (now_date == Saved_data.getInst.Date_list[m] || Use_date == false)
                    {
                        if (now_time == Saved_data.getInst.Time_list[m])
                        {
                            if (communication.sendvalue(Saved_data.getInst.Command_list[m]))
                            {
                                if (Use_date)
                                    runover.Add(m);
                            }
                        }
                    }
                }
                for (int i = runover.Count - 1; i >= 0; i--)
                {
                    Delete_task(runover[i]);
                }
            }
            catch (Exception err)
            {
                timer_sec_run.Enabled = false;
                MessageBox.Show(err.Message);
                LogContext.theInst.LogPrint(err.ToString(), 2);
            }

        }

        private void bt_add_Click(object sender, EventArgs e)
        {
            add_task(dateTimePicker1.Value.ToLongDateString(), dateTimePicker2.Value.ToLongTimeString(), textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            timer_sec_run.Enabled = !timer_sec_run.Enabled;
            if (timer_sec_run.Enabled)
                label3.Text = "Timer Control Enabled";
            else
                label3.Text = "Timer Control Disabled";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                Use_date = false;
            else
                Use_date = true;
        }

        private void bt_del_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                return;
            }
            int id = dataGridView1.SelectedRows[0].Index;
            Delete_task(id);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = ".\\";
            sfd.Filter = "(*.DAT)|*.DAT";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Data_IO.getInst.Save_IO(sfd.FileName);
                Properties.Settings.Default.Path = sfd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = ".\\";
            ofd.Filter = "(*.DAT)|*.DAT";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (Data_IO.getInst.Load_IO(ofd.FileName))
                {
                    for (int i = 0; i < Saved_data.getInst.Command_list.Count; i++)
                    {
                        dataGridView1.Rows.Add(Saved_data.getInst.Date_list[i], Saved_data.getInst.Time_list[i], Saved_data.getInst.Command_list[i]);
                    }
                }
            }
        }

        private void diconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                communication.closeConnect();
            }
            catch (System.Exception ex)
            {
                LogContext.theInst.LogPrint(ex.ToString(), 2);
            }
        }
    }
}
