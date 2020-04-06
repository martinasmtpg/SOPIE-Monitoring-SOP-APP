using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Transaksi
{
    public class Transaction : BaseModel
    {
        public int Id { get; set; }

        public Bab Bab { get; set; }
        public int BabId { get; set; }

        public SubBab SubBab { get; set; }
        public int SubBabId { get; set; }

        public SubSubBab SubSubBab { get; set; }
        public int SubSubBabId { get; set; }
    }
}