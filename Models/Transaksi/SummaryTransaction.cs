using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Transaksi
{
    public class SummaryTransaction : BaseModel
    {
        public int Id { get; set; }
        public DetailTransaction DetailTransaction { get; set; }
        public int DetailTransactionId { get; set; }
        public SubBabProsedur SubBabProsedur { get; set; }
        public int SubBabProsedurId { get; set; }
        public SubSubBabProsedur SubSubBabProsedur { get; set; }
        public int SubSubBabProsedurId { get; set; }
        public AspekPedoman AspekPedoman { get; set; }
        public int AspekPedomanId { get; set; }
        public HasilReview HasilReview { get; set; }
        public int HasilReviewId { get; set; }
        public Updating Updating { get; set; }
        public int UpdatingId { get; set; }
        public DasarUpdating DasarUpdating { get; set; }
        public int DasarUpdatingId { get; set; }
        public AcuanUpdating AcuanUpdating { get; set; }
        public int AcuanUpdatingId { get; set; }
        public string KetExisting { get; set; }
        public string KetUpdating { get; set; }
        public string Keterangan { get; set; }
    }
}