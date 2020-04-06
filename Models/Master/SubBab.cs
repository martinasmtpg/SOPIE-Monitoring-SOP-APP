using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master
{
    public class SubBab : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nomor tidak boleh kosong")]
        public string Nomor { get; set; }

        [Required(ErrorMessage = "Nama Sub Bab tidak boleh kosong")]
        [Display(Name = "Nama Sub Bab")]
        public string NamaSubBab { get; set; }
    }
}