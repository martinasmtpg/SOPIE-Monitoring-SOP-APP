using AplikasiSOP.Models;
using AplikasiSOP.Models.Master;
using AplikasiSOP.Models.Master.Status;
using AplikasiSOP.Models.Transaksi;
using AplikasiSOP.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers.Transaksis
{
    public class HistoriesController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Histories
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var lasthistory = _context.HistoryTransaction.GroupBy(x => x.DetailTransaction.TransactionId).Select(x => x.OrderByDescending(z => new { z.DetailTransactionId, z.Id }).FirstOrDefault()).ToList();
            var idhistory = lasthistory.Select(x => x.Id).ToList();
            var iddetail = lasthistory.Select(x => x.DetailTransactionId).ToList();
            List<DetailTransaction> detail = new List<DetailTransaction>();
            if (currentUser.Kelompok == "PGO")
            {
                detail = _context.DetailTransaction.Include("Transaction.Bab.Book").Where(x => iddetail.Contains(x.Id)).ToList();
            }
            else
            {
                detail = _context.DetailTransaction.Include("Transaction.Bab.Book").Where(x => iddetail.Contains(x.Id) && x.Transaction.Bab.Kelompok.Singkatan == currentUser.Kelompok).ToList();
            }
            
            var idtrans = detail.GroupBy(x => x.TransactionId).Select(x => x.FirstOrDefault().TransactionId).ToList();
            var idbab = detail.GroupBy(x => x.Transaction.BabId).Select(x => x.FirstOrDefault().Transaction.BabId).ToList();
            var idbuku = detail.GroupBy(x => x.Transaction.Bab.BookId).Select(x => x.FirstOrDefault().Transaction.Bab.BookId).ToList();

            ProgressVM result = new ProgressVM();

            result.BukuProgressVM = (from book in _context.Book.Where(x => idbuku.Contains(x.Id)).ToList()
                                     select new BukuProgressVM
                                     {
                                         IdBuku = book.Id,
                                         NoBuku = book.Nomor,
                                         NamaBuku = book.NamaBuku,
                                         BabProgressVM = (from bab in _context.Bab.Where(x => x.BookId == book.Id && idbab.Contains(x.Id)).ToList()
                                                          select new BabProgressVM
                                                          {
                                                              IdBab = bab.Id,
                                                              NoBab = bab.Nomor,
                                                              NamaBab = bab.NamaBab,
                                                              DetailProgressVM = (from trans in _context.Transaction.Where(x => x.BabId == bab.Id && idtrans.Contains(x.Id)).ToList()
                                                                                  select new DetailProgressVM
                                                                                  {
                                                                                      DataTransaction = _context.HistoryTransaction.Include("ActivityTimeline").Include("DetailTransaction.Transaction.Bab.Book").Include("DetailTransaction.Transaction.Bab.Kelompok").Include("DetailTransaction.Transaction.SubBab").Include("DetailTransaction.Transaction.SubSubBab").Include("ProgresStatus").Where(x => x.DetailTransaction.TransactionId == trans.Id && idhistory.Contains(x.Id)).OrderByDescending(x => x.DetailTransaction.Id).ThenByDescending(x => x.Id).FirstOrDefault(),
                                                                                      AktivitasTarget = GetTarget(trans.Id),
                                                                                      Status = GetStatus(trans.Id)
                                                                                  }
                                                                                  ).ToList()
                                                          }
                                                     ).ToList()
                                     }
                          ).ToList();

            result.AspekPedomanData = _context.AspekPedoman.ToList();

            return View(result);
        }
        public ActivityTimeline GetTarget(int id)
        {
            var progrespertama = _context.HistoryTransaction.Where(x => x.DetailTransaction.TransactionId == id).OrderByDescending(x => x.DetailTransaction.Id).ThenBy(x => x.Id).FirstOrDefault();
            DateTime datenow = DateTime.Now;
            ActivityTimeline result = new ActivityTimeline();
            DateTime datefirst = progrespertama.CreateDate;
            var hitung = (datenow.Subtract(datefirst)).Days;
            var harikerja = _context.ActivityTimeline.Sum(x => x.HariKerja);
            if (hitung > harikerja)
            {
                result = _context.ActivityTimeline.Where(x => x.SumHariKerja == harikerja).OrderBy(x => x.Id).FirstOrDefault();
            }
            else
            {
                result = _context.ActivityTimeline.Where(x => x.SumHariKerja >= hitung).OrderBy(x => x.Id).FirstOrDefault();
            }
            return result;
        }
        public ScheduleStatus GetStatus(int id)
        {
            DateTime datenow = DateTime.Now;
            var target = GetTarget(id);

            var progress = _context.HistoryTransaction.Include("ActivityTimeline").Include("DetailTransaction").Where(x => x.DetailTransaction.TransactionId == id).OrderByDescending(x => x.DetailTransaction.Id).ThenByDescending(x => x.Id).FirstOrDefault();

            //int progresstimeline = DateTime.Compare(datenow, progress.DetailTransaction.Timeline);
            int progresstimeline = DateTime.Compare(datenow, progress.DetailTransaction.Tgl_JatuhTempo);
            int idstatus = 1;
            if (progresstimeline < 0)
            {
                if (progress.ActivityTimeline.SumHariKerja > target.SumHariKerja)
                {
                    //ahead
                    idstatus = 1; 
                }
                else if (progress.ActivityTimeline.SumHariKerja == target.SumHariKerja)
                {
                    //on schedule
                    idstatus = 2;
                }
                else
                {
                    //behind
                    idstatus = 3;
                }
            }
            else
            {
                //late
                idstatus = 4;
            }

            var result = _context.ScheduleStatus.SingleOrDefault(x => x.Id == idstatus);
            return result;
        }
        public JsonResult GetViewDetail(int id)
        {
            var result = _context.DetailTransaction.Include("Transaction.Bab").Include("Transaction.Bab.Book").Include("Transaction.Bab.Kelompok").Include("Transaction.SubBab").Include("Transaction.SubSubBab").Where(x => x.Id == id).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataSummary(int dettrans, int aspek)
        {
            var result = _context.SummaryTransaction.Include("SubSubBabProsedur").Include("SubBabProsedur").Include("HasilReview").Include("Updating").Include("DasarUpdating").Include("AcuanUpdating").Where(x => x.DetailTransactionId == dettrans && x.AspekPedomanId == aspek).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(string filePath)
        {
            if (filePath != "null")
            {
                string fullName = Server.MapPath("~//Files/" + filePath);

                byte[] fileBytes = GetFile(fullName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult DownloadFileById(int id)
        {
            var detail = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);
            string filePath = detail.PathFiles;
            if (filePath != "null")
            {
                string fullName = Server.MapPath("~//Files/" + filePath);

                byte[] fileBytes = GetFile(fullName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public JsonResult GetHistory(int id)
        {
            var result = _context.HistoryTransaction.Include("ActivityTimeline").Where(x => x.DetailTransactionId == id && x.ActivityTimelineId != 16).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}