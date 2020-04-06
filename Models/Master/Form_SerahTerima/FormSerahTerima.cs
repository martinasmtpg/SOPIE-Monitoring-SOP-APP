using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Transaksi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master.Form_SerahTerima 
{
    public class FormSerahTerima : BaseModel
    {
        public int Id { get; set; }

        public DetailTransaction DetailTransaction { get; set; }
        public int DetailTransactionId { get; set; }

        public string Hari { get; set; }

        public DateTime TglCetak { get; set; }

        public int NomorMemo { get; set; }

        public DateTime TglMemo { get; set; }

        public string JenisDokumen { get; set; }

        public string JenisPedoman { get; set; }

        public string NamaFolder { get; set; }

        public int Size { get; set; }

        public string Contains { get; set; }

        public string Created { get; set; }
    }
}