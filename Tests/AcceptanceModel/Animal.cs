using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcceptanceModel
{
    public class Animal
    {
        public string Species { get; set; }

        private string genus;

        public string Genus { get { return genus; } set { genus = value; } }

    }
}
