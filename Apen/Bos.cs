using System;
using System.Collections.Generic;
using System.Text;

namespace Apen
{
    class Bos
    {
        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
        public int ID { get; set; }


        public Bos(int id, int xmin, int xmax, int ymin, int ymax)
        {
            this.XMax = xmax;
            this.XMin = xmin;
            this.YMin = ymin;
            this.YMax = ymax;
            this.ID = id;
        }
    }
}
