using System;
using System.Collections.Generic;
using System.Text;

namespace Apen
{
    class Boom
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int ID { get; set; }

        public Boom(int x, int y, int id)
        {
            this.X = x;
            this.Y = y;
            this.ID = id;
        }
    }
}
