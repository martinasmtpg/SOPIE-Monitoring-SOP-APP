using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class AcuanUpdatingsController : Controller
    {
        private ApplicationDbContext _context;

        public AcuanUpdatingsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: AcuanUpdatings
        public ActionResult InputAcuanUpdating()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.AcuanUpdating.Include("AspekPedoman").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.AcuanUpdating.Single(x => x.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(AcuanUpdating acuanupdating)
        {
            if (acuanupdating.Id == 0)
            {
                acuanupdating.AspekPedoman = _context.AspekPedoman.Where(x => x.Id == acuanupdating.AspekPedomanId).SingleOrDefault();
                acuanupdating.AspekPedomanId = acuanupdating.AspekPedoman.Id;
                acuanupdating.CreateDate = DateTime.Now;
                _context.AcuanUpdating.Add(acuanupdating);
            }
            else
            {
                var acuanupdatingDb = _context.AcuanUpdating.Single(j => j.Id == acuanupdating.Id);
                acuanupdatingDb.JenisAcuanUpdating = acuanupdating.JenisAcuanUpdating;
                acuanupdatingDb.AspekPedomanId = acuanupdating.AspekPedomanId;
                acuanupdatingDb.UpdateDate = DateTime.Now;
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
            var b = _context.AcuanUpdating.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.AcuanUpdating.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByFkId(int id)
        {
            var result = _context.AcuanUpdating.Include("AspekPedoman").Where(x => x.AspekPedomanId == id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}