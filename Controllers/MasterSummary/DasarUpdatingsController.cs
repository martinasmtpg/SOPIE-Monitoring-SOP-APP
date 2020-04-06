﻿using AplikasiSOP.Models;
using AplikasiSOP.Models.Master.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.MasterSummary
{
    public class DasarUpdatingsController : Controller
    {
        private ApplicationDbContext _context;

        public DasarUpdatingsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: DasarUpdatings
        public ActionResult InputDasarUpdating()
        {
            return View();
        }

        public JsonResult Get()
        {
            var result = _context.DasarUpdating.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int id)
        {
            var result = _context.DasarUpdating.SingleOrDefault(i => i.Id == id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(DasarUpdating dasarupdating)
        {
            if (dasarupdating.Id == 0)
            {
                dasarupdating.CreateDate = DateTime.Now;
                _context.DasarUpdating.Add(dasarupdating);
            }
            else
            {
                var dasarupdatingDb = _context.DasarUpdating.Single(j => j.Id == dasarupdating.Id);
                dasarupdatingDb.JenisDasarUpdating = dasarupdating.JenisDasarUpdating;
                dasarupdatingDb.UpdateDate = DateTime.Now;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            bool result = false;
            var b = _context.DasarUpdating.Where(x => x.Id == id).FirstOrDefault();
            if (b != null)
            {
                _context.DasarUpdating.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}