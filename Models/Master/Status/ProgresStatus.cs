using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master.Status
{
    public class ProgresStatus : BaseModel
    {
        public int Id { get; set; }
        public string NamaStatus { get; set; }
        public string WarnaStatus { get; set; }
    }
}