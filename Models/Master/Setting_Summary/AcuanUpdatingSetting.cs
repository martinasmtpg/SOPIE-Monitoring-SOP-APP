using AplikasiSOP.Models.Core;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplikasiSOP.Models.Master.Setting_Summary
{
    public class AcuanUpdatingSetting : BaseModel
    {
        public int Id { get; set; }

        public SubBabProsedurSetting SubBabProsedurSetting { get; set; }
        public int SubBabProsedurSettingId { get; set; }

        public AcuanUpdating AcuanUpdating { get; set; }
        public int AcuanUpdatingId { get; set; }
    }
}