using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace switch_win
{
        [Serializable]
    public class Saved_data
    {
        public List<String> Command_list = new List<string>();
        public List<String> Date_list = new List<string>();
        public List<String> Time_list = new List<string>();
        static Saved_data inst;
        public static Saved_data getInst
        {
            get
            {
                if (inst == null)
                {
                    inst = new Saved_data();
                }
                return inst;
            }
            set
            {
                inst = value;
            }
        }
    }
    class Data_IO
    {

        static Data_IO inst;
        public static Data_IO getInst
        {
            get
            {
                if (inst == null)
                {
                    inst = new Data_IO();
                }
                return inst;
            }
        }

        public bool Load_IO(string _path)
        {
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                string path = _path;
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                Saved_data Data = (Saved_data)formatter.Deserialize(stream);
                stream.Close();
                Saved_data.getInst = Data;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                LogContext.theInst.LogPrint(e.ToString(), 2);
                return false;
            }
        }
        public bool Save_IO(string _path)
        {
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                string path = _path;
                stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, Saved_data.getInst);
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                LogContext.theInst.LogPrint(e.ToString(), 2);
                return false;
            }
        }
    }
}
