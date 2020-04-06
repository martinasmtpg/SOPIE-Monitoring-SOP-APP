using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class ActivityTimelinesController : Controller
    {
        private ApplicationDbContext _context;

        public ActivityTimelinesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: ActivityTimelines
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.ActivityTimeline.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.ActivityTimeline.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SumHariKerja()
        {
            int result = 0;
            var data = _context.ActivityTimeline;
            var count = data.Count();
            if (count > 0)
            {
                result = data.Sum(i => i.HariKerja);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Sum()

        {
            decimal result = 0;
            var data = _context.ActivityTimeline;
            var count = data.Count();
            if (count > 0)
            {
                result = data.Sum(i => i.PersentaseProgress);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SumWhenEdit()

        //{
        //    decimal result = 0;
        //    var data = _context.ActivityTimeline;
        //    var count = data.Count();
        //    if (count > 0)
        //    {
        //        result = data.Sum(i => i.PersentaseProgress);
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SumWhenDelete()

        //{
        //    decimal result = 0;
        //    var data = _context.ActivityTimeline;
        //    var count = data.Count();
        //    if (count > 0)
        //    {
        //        result = data.Sum(i => i.PersentaseProgress);
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult Save(ActivityTimeline act)
        {
            if (act.Id == 0)
            {
                act.CreateDate = DateTime.Now;
                _context.ActivityTimeline.Add(act);
            }
            else
            {
                var actDb = _context.ActivityTimeline.Single(j => j.Id == act.Id);
                actDb.AktivitasProgress = act.AktivitasProgress;
                actDb.HariKerja = act.HariKerja;
                actDb.SumHariKerja = act.SumHariKerja;
                actDb.PersentaseProgress = act.PersentaseProgress;
                actDb.SumPersentaseProgress = act.SumPersentaseProgress;
                actDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.ActivityTimeline.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.ActivityTimeline.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSum()
        {
            var result = _context.ActivityTimeline.Sum(x => x.HariKerja);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}