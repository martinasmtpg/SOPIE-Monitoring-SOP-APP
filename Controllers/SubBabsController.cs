using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class SubBabsController : Controller
    {
        private ApplicationDbContext _context;

        public SubBabsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.SubBab.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.SubBab.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(SubBab subbab)
        {
            if (subbab.Id == 0)
            {
                subbab.CreateDate = DateTime.Now;
                _context.SubBab.Add(subbab);
            }
            else
            {
                var subbabDb = _context.SubBab.Single(j => j.Id == subbab.Id);
                subbabDb.NamaSubBab = subbab.NamaSubBab;
                subbabDb.Nomor = subbab.Nomor;
                subbabDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult Delete(int id)
        {
            bool result = false;
            var b = _context.SubBab.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.SubBab.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}