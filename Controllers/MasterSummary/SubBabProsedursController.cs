using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class SubBabProsedursController : Controller
    {
        private ApplicationDbContext _context;

        public SubBabProsedursController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: SubBabProsedurs
        public ActionResult InputSubBabProsedur()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.SubBabProsedur.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.SubBabProsedur.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(SubBabProsedur subbabprosedur)
        {
            if (subbabprosedur.Id == 0)
            {
                subbabprosedur.CreateDate = DateTime.Now;
                _context.SubBabProsedur.Add(subbabprosedur);
            }
            else
            {
                var subbabprosedurDb = _context.SubBabProsedur.Single(j => j.Id == subbabprosedur.Id);
                subbabprosedurDb.Nama = subbabprosedur.Nama;
                subbabprosedurDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.SubBabProsedur.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.SubBabProsedur.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}