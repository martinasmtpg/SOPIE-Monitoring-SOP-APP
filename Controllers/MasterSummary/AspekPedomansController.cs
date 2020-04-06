using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class AspekPedomansController : Controller
    {
        private ApplicationDbContext _context;

        public AspekPedomansController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: AspekPedomans
        public ActionResult InputAspekPedoman()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.AspekPedoman.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.AspekPedoman.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(AspekPedoman aspekpedoman)
        {
            if (aspekpedoman.Id == 0)
            {
                aspekpedoman.CreateDate = DateTime.Now;
                _context.AspekPedoman.Add(aspekpedoman);
            }
            else
            {
                var aspekpedomanDb = _context.AspekPedoman.Single(j => j.Id == aspekpedoman.Id);
                aspekpedomanDb.Nama = aspekpedoman.Nama;
                aspekpedomanDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.AspekPedoman.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.AspekPedoman.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}