using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class SubSubBabsController : Controller
    {
        private ApplicationDbContext _context;

        public SubSubBabsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.SubSubBab.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.SubSubBab.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save (SubSubBab subsubbab)
        {
            if (subsubbab.Id == 0)
            {
                subsubbab.CreateDate = DateTime.Now;
                _context.SubSubBab.Add(subsubbab);
            }
            else
            {
                var subsubbabDb = _context.SubSubBab.Single(j => j.Id == subsubbab.Id);
                subsubbabDb.NamaSubSubBab = subsubbab.NamaSubSubBab;
                subsubbabDb.Nomor = subsubbab.Nomor;
                subsubbabDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.SubSubBab.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.SubSubBab.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}