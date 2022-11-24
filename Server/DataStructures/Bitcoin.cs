using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataStructures
{
    class Bitcoin
    {
        public DateTime time{ get; set; }
        public string asset_id_base { get; set; }
        public string asset_id_quote { get; set; }
        public double rate { get; set; }
    }
}
