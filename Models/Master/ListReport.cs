using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master
{
    public class ListReport : BaseModel
    {
        public int Id { get; set; }

        public string Nama { get; set; }

        public string Controller { get; set; }

        public string Method { get; set; }
    }
}