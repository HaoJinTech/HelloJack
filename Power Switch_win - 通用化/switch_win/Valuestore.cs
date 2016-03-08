using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace switch_win
{
    public class Valuestore
    {
        public int switch_channel = 12;
        public int line_x = 2;
        public int line_y
        {
            get { return switch_channel / line_x; }
        }
        public int x_space = 10;
        public int y_space = 10;
        public int x_init = 50;
        public int y_init = 100;
    }
}
