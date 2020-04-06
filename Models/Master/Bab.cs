using AplikasiSOP.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master
{
    public class Bab : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public Kelompok Kelompok { get; set; }
        [ForeignKey("Kelompok")]
        public int KelompokId { get; set; }

        [Required(ErrorMessage = "Silahkan pilih Nama Buku")]
        [Display(Name = "Nama Buku")]
        public Book Book { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Nomor tidak boleh kosong")]
        public string Nomor { get; set; }

        [Required(ErrorMessage = "Nama Bab tidak boleh kosong")]
        [Display(Name = "Nama Bab")]
        public string NamaBab { get; set; }
    }
}