using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master;
using AplikasiSOP.Models.Master.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Transaksi
{
    public class DetailTransaction : BaseModel
    {
        public int Id { get; set; }

        public Transaction Transaction { get; set; }
        public int TransactionId { get; set; }

        public DateTime Tgl_Berlaku { get; set; }

        public DateTime Tgl_JatuhTempo { get; set; }

        public DateTime Timeline { get; set; }

        public string PathFiles { get; set; }
    }
}