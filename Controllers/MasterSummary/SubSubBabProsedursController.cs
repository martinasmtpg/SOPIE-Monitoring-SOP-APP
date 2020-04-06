using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class SubSubBabProsedursController : Controller
    {
        private ApplicationDbContext _context;

        public SubSubBabProsedursController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: SubSubBabProsedurs
        public ActionResult InputSubSubBabProsedur()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.SubSubBabProsedur.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.SubSubBabProsedur.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByFkId(int id)
        {
            var result = _context.SubSubBabProsedur.Where(x => x.SubBabProsedurId == id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(SubSubBabProsedur subsubbabprosedur)
        {
            if (subsubbabprosedur.Id == 0)
            {
                subsubbabprosedur.SubBabProsedur = _context.SubBabProsedur.Where(x => x.Id == subsubbabprosedur.SubBabProsedurId).SingleOrDefault();
                subsubbabprosedur.SubBabProsedurId = subsubbabprosedur.SubBabProsedur.Id;
                subsubbabprosedur.CreateDate = DateTime.Now;
                _context.SubSubBabProsedur.Add(subsubbabprosedur);
            }
            else
            {
                var subsubbabprosedurDb = _context.SubSubBabProsedur.Single(j => j.Id == subsubbabprosedur.Id);
                subsubbabprosedurDb.NamaSubSubBabProsedur = subsubbabprosedur.NamaSubSubBabProsedur;
                subsubbabprosedurDb.SubBabProsedurId = subsubbabprosedur.SubBabProsedurId;
                subsubbabprosedurDb.UpdateDate = DateTime.Now;
            }

            try
            {
                var result = _context.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(e, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.SubSubBabProsedur.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.SubSubBabProsedur.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}