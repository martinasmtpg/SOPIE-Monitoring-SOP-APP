using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master
{
    public class Kelompok : BaseModel
    {
        public int Id { get; set; }

        public string Nama { get; set; }

        public string Singkatan { get; set; }

        public string Wilayah { get; set; }
    }
}