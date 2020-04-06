using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class UpdatingsController : Controller
    {
        private ApplicationDbContext _context;

        public UpdatingsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Updatings
        public ActionResult InputUpdating()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.Updating.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.Updating.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(Updating updating)
        {
            if (updating.Id == 0)
            {
                updating.CreateDate = DateTime.Now;
                _context.Updating.Add(updating);
            }
            else
            {
                var updatingDb = _context.Updating.Single(j => j.Id == updating.Id);
                updatingDb.Nama = updating.Nama;
                updatingDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.Updating.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.Updating.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}