﻿using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class HasilReviewsController : Controller
    {
        private ApplicationDbContext _context;

        public HasilReviewsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: HasilReviews
        public ActionResult InputHasilReview()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.HasilReview.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.HasilReview.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(HasilReview hasilreview)
        {
            if (hasilreview.Id == 0)
            {
                hasilreview.CreateDate = DateTime.Now;
                _context.HasilReview.Add(hasilreview);
            }
            else
            {
                var hasilreviewDb = _context.HasilReview.Single(j => j.Id == hasilreview.Id);
                hasilreviewDb.Nama = hasilreview.Nama;
                hasilreviewDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.HasilReview.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.HasilReview.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}