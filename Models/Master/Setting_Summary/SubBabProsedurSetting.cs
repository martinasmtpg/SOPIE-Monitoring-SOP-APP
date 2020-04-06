using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master.Setting_Summary
{
    public class SubBabProsedurSetting : BaseModel
    {
        public int Id { get; set; }

        public SubBabProsedur SubBabProsedur { get; set; }
        public int SubBabProsedurId { get; set; }

        public AspekPedoman AspekPedoman { get; set; }
        public int AspekPedomanId { get; set; }

        public Book Book { get; set; }
        public int BookId { get; set; }
    }
}