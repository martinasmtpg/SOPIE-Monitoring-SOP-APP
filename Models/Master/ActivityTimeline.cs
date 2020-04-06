using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Transaksi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master
{
    public class ActivityTimeline : BaseModel
    {
        public int Id { get; set; }

        public string AktivitasProgress { get; set; }

        public int HariKerja { get; set; }
        public int SumHariKerja { get; set; }

        public decimal PersentaseProgress { get; set; }

        public decimal SumPersentaseProgress { get; set; }
    }
}