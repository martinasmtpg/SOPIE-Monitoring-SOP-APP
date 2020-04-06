using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class PPMsController : Controller
    {
        private ApplicationDbContext _context;

        public PPMsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.PPM.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.PPM.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(PPM ppm)
        {
            if (ppm.Id == 0)
            {
                ppm.CreateDate = DateTime.Now;
                _context.PPM.Add(ppm);
            }
            else
            {
                var picDb = _context.PPM.Single(j => j.Id == ppm.Id);
                picDb.Nama = ppm.Nama;
                picDb.Telepon = ppm.Telepon;
                picDb.Email = ppm.Email;
                picDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.PPM.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.PPM.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}