using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master.Summary
{
    public class AcuanUpdating : BaseModel
    {
        public int Id { get; set; }

        public string Nama { get; set; }
    }
}