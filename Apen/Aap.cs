using System;
using System.Collections.Generic;
using System.Text;

namespace Apen
{
    class Aap
    {
        public int ID { get; set; }
        public string Naam { get; set; }
        public List<Boom> Boom_Lijst = new List<Boom>();
        public Aap(int id, string naam)
        {
            this.ID = id;
            this.Naam = naam;
        }
    }
}
