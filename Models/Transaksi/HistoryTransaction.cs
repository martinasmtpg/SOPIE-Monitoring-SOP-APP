using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master;
using AplikasiSOP.Models.Master.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Transaksi
{
    public class HistoryTransaction : BaseModel
    {
        public int Id { get; set; }

        public DetailTransaction DetailTransaction { get; set; }
        public int DetailTransactionId { get; set; }

        public ActivityTimeline ActivityTimeline { get; set; }
        public int ActivityTimelineId { get; set; }

        public ActivityTimeline AktivitasTarget { get; set; }
        public int AktivitasTargetId { get; set; }

        public ProgresStatus ProgresStatus { get; set; }
        public int ProgresStatusId { get; set; }

        public virtual ApplicationUser Inputer { get; set; }
        public string InputerId { get; set; }

        public DateTime? CheckerDate { get; set; }

        public virtual ApplicationUser Checker { get; set; }
        public string CheckerId { get; set; }

        public string CheckKeterangan { get; set; }

        public DateTime? ApproveDate { get; set; }
                
        public virtual ApplicationUser Approver { get; set; }
        public string ApproverId { get; set; }
        public string ApproveKeterangan { get; set; }

    }
}