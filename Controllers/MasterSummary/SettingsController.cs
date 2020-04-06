using AplikasiSOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class SettingsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Settings
        public JsonResult GetSubBabProsedur(int book, int aspek)
        {
            var result = _context.SubBabProsedurSetting.Include("SubBabProsedur").Where(i => i.BookId == book && i.AspekPedomanId == aspek).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubSubBabProsedur(int book, int aspek, int subbab)
        {
            var subbabprosedur = _context.SubBabProsedurSetting.Where(i => i.BookId == book && i.AspekPedomanId == aspek && i.SubBabProsedurId == subbab).ToList();
            var result = _context.SubSubBabProsedurSetting.Include("SubSubBabProsedur").Where(i => i.SubBabProsedurSetting.BookId == book && i.SubBabProsedurSetting.AspekPedomanId == aspek && i.SubBabProsedurSetting.SubBabProsedurId == subbab).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAcuanUpdating(int book, int aspek, int subbab)
        {
            var result = _context.AcuanUpdatingSetting.Include("AcuanUpdating").Where(i => i.SubBabProsedurSetting.BookId == book && i.SubBabProsedurSetting.AspekPedomanId == aspek && i.SubBabProsedurSetting.SubBabProsedurId == subbab).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDasarUpdating(int book, int aspek, int subbab)
        {
            var result = _context.DasarUpdatingSetting.Include("DasarUpdating").Where(i => i.SubBabProsedurSetting.BookId == book && i.SubBabProsedurSetting.AspekPedomanId == aspek && i.SubBabProsedurSetting.SubBabProsedurId == subbab).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}