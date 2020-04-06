using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class BabsController : Controller
    {
        private ApplicationDbContext _context;

        public BabsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.Bab.Include("Book").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.Bab.Single(x => x.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(Bab bab)
        {
            if (bab.Id == 0)
            {
                bab.Book = _context.Book.Where(x => x.Id == bab.BookId).SingleOrDefault();
                bab.BookId = bab.Book.Id;
                bab.CreateDate = DateTime.Now;
                _context.Bab.Add(bab);
            }
            else
            {
                var babDb = _context.Bab.Single(j => j.Id == bab.Id);
                babDb.NamaBab = bab.NamaBab;
                babDb.Nomor = bab.Nomor;
                babDb.BookId = bab.BookId;
                babDb.UpdateDate = DateTime.Now;
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
            var b = _context.Bab.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.Bab.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}