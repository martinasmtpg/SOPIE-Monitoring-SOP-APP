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
    public class InputsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Inputs
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            List<HistoryTransaction> lasthistory = new List<HistoryTransaction>();
            var ActivityTimelineId = new List<int> { 1, 2 };
            var ProgresStatusId = new List<int> { 1, 2, 3, 7 };
            lasthistory = _context.HistoryTransaction.GroupBy(x => x.DetailTransaction.TransactionId).Select(x => x.OrderByDescending(z => new { z.DetailTransactionId, z.Id }).FirstOrDefault()).ToList().Where(y => ActivityTimelineId.Contains(y.ActivityTimelineId) && ProgresStatusId.Contains(y.ProgresStatusId)).ToList();

            List<HistoryTransaction> relevanhistory = new List<HistoryTransaction>();
            var relevan = _context.HistoryTransaction.GroupBy(x => x.DetailTransaction.TransactionId).Select(x => x.OrderByDescending(z => new { z.DetailTransactionId, z.Id }).FirstOrDefault()).ToList().Where(y => y.ActivityTimelineId == 16).Select(y => y.Id).ToList();
            relevanhistory = _context.HistoryTransaction.Where(x => relevan.Contains(x.Id) && x.DetailTransaction.Timeline <= DateTime.Now).ToList();
            lasthistory.AddRange(relevanhistory);

            var idhistory = lasthistory.Select(x => x.Id).ToList();
            var iddetail = lasthistory.Select(x => x.DetailTransactionId).ToList();
            var detail = _context.DetailTransaction.Include("Transaction.Bab.Book").Where(x => iddetail.Contains(x.Id)).ToList();
            var idtrans = detail.GroupBy(x => x.TransactionId).Select(x => x.FirstOrDefault().TransactionId).ToList();
            var currentBab = _context.Bab.Where(x => x.Kelompok.Singkatan == currentUser.Kelompok).ToList();
            var idbab = currentBab.Select(x => x.Id).ToList();
            var idbuku = currentBab.GroupBy(x => x.BookId).Select(x => x.FirstOrDefault().BookId).ToList();

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
                    idstatus = 1;
                }
                else if (progress.ActivityTimeline.SumHariKerja == target.SumHariKerja)
                {
                    idstatus = 2;
                }
                else
                {
                    idstatus = 3;
                }
            }
            else
            {
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
        public JsonResult GetViewHistory(int id)
        {
            HistoryTransaction result = new HistoryTransaction();
            result = _context.HistoryTransaction.SingleOrDefault(x => x.Id == id && x.ProgresStatusId == 5);

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
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public JsonResult Update(int id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var detrans = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);

            DetailTransaction detailTransaction = new DetailTransaction();
            detailTransaction.TransactionId = detrans.TransactionId;
            detailTransaction.Tgl_Berlaku = detrans.Tgl_Berlaku;
            detailTransaction.Tgl_JatuhTempo = detrans.Tgl_JatuhTempo;
            detailTransaction.Timeline = detrans.Timeline;
            detailTransaction.CreateDate = detrans.Timeline;
            detailTransaction.UpdateDate = DateTime.Now;
            _context.DetailTransaction.Add(detailTransaction);
            _context.SaveChanges();

            HistoryTransaction historyTransaction = new HistoryTransaction();
            historyTransaction.DetailTransactionId = detailTransaction.Id;
            historyTransaction.ActivityTimelineId = 1;
            historyTransaction.AktivitasTargetId = 1;
            historyTransaction.ProgresStatusId = 1;
            historyTransaction.CreateDate = detrans.Timeline;
            historyTransaction.UpdateDate = DateTime.Now;
            historyTransaction.InputerId = currentUser.Id;
            _context.HistoryTransaction.Add(historyTransaction);
            _context.SaveChanges();

            //var result = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);
            var result = _context.DetailTransaction.SingleOrDefault(x => x.Id == detailTransaction.Id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Relevan(int id, DetailTransaction detail)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var detrans = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);
            detrans.Tgl_JatuhTempo = detail.Tgl_JatuhTempo;
            detrans.Timeline = detail.Timeline;
            detrans.UpdateDate = DateTime.Now;
            _context.Entry(detrans).State = EntityState.Modified;

            _context.SaveChanges();

            var historyTransaction = _context.HistoryTransaction.Where(x => x.DetailTransactionId == id).OrderByDescending(x => x.Id).FirstOrDefault();
            historyTransaction.ProgresStatusId = 8;
            historyTransaction.ApproveDate = DateTime.Now;
            historyTransaction.ApproverId = currentUser.Id;
            _context.Entry(historyTransaction).State = EntityState.Modified;
            _context.SaveChanges();

            HistoryTransaction historyTransaction2 = new HistoryTransaction();
            historyTransaction2.DetailTransactionId = detrans.Id;
            historyTransaction2.ActivityTimelineId = 16;
            historyTransaction2.AktivitasTargetId = 16;
            historyTransaction2.ProgresStatusId = 1;
            historyTransaction2.CreateDate = DateTime.Now;
            historyTransaction2.InputerId = currentUser.Id;
            _context.HistoryTransaction.Add(historyTransaction2);


            var result = _context.SaveChanges();
            //var result = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetViewDelete(int id)
        {
            DetailProgressVM result = new DetailProgressVM();
            var detrans = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);
            result.DataTransaction = _context.HistoryTransaction.Include("ActivityTimeline").Include("DetailTransaction").Include("DetailTransaction.Transaction.Bab").Include("DetailTransaction.Transaction.Bab.Book").Include("DetailTransaction.Transaction.Bab.Kelompok").Include("DetailTransaction.Transaction.SubBab").Include("DetailTransaction.Transaction.SubSubBab").Where(x => x.DetailTransaction.TransactionId == detrans.TransactionId).OrderByDescending(x => x.DetailTransaction.Id).ThenByDescending(x => x.Id).FirstOrDefault();
            result.AktivitasTarget = GetTarget(detrans.TransactionId);
            result.Status = GetStatus(detrans.TransactionId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelete(int id)
        {
            var detrans = _context.DetailTransaction.SingleOrDefault(x => x.Id == id);
            if (detrans.PathFiles != null)
            {
                string path = Server.MapPath("~/Files/" + detrans.PathFiles);
                System.IO.File.Delete(path);
            }
            var detranscount = _context.DetailTransaction.Where(x => x.TransactionId == detrans.TransactionId).Count();
            if (detranscount > 1)
            {
                _context.DetailTransaction.Remove(detrans);
            }
            else
            {
                var trans = _context.Transaction.SingleOrDefault(x => x.Id == detrans.TransactionId);
                _context.Transaction.Remove(trans);
            }
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPP(int id)
        {
            SummaryVM summaryVM = new SummaryVM();
            summaryVM.BabData = _context.Bab.Include("Book").Include("Kelompok").SingleOrDefault(x => x.Id == id);
            summaryVM.AspekPedomanData = _context.AspekPedoman.ToList();
            return View(summaryVM);
        }
        public JsonResult SubmitTransaksi(Transaction trans, DetailTransaction detail)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            Transaction transaction = new Transaction();
            transaction.BabId = trans.BabId;
            transaction.SubBabId = trans.SubBabId;
            transaction.SubSubBabId = trans.SubSubBabId;
            transaction.CreateDate = DateTime.Now;
            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            DetailTransaction detailTransaction = new DetailTransaction();
            detailTransaction.TransactionId = transaction.Id;
            detailTransaction.Tgl_Berlaku = detail.Tgl_Berlaku;
            detailTransaction.Tgl_JatuhTempo = detail.Tgl_JatuhTempo;
            detailTransaction.Timeline = detail.Timeline;
            detailTransaction.CreateDate = DateTime.Now;
            _context.DetailTransaction.Add(detailTransaction);
            _context.SaveChanges();

            HistoryTransaction historyTransaction = new HistoryTransaction();
            historyTransaction.DetailTransactionId = detailTransaction.Id;
            historyTransaction.ActivityTimelineId = 2;
            historyTransaction.AktivitasTargetId = 2;
            historyTransaction.ProgresStatusId = 2;
            historyTransaction.CreateDate = DateTime.Now;
            historyTransaction.InputerId = currentUser.Id;
            _context.HistoryTransaction.Add(historyTransaction);
            _context.SaveChanges();

            var result = _context.DetailTransaction.SingleOrDefault(x => x.Id == detailTransaction.Id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditPP(int id)
        {
            EditDetTransVM editDetTransVM = new EditDetTransVM();
            editDetTransVM.DetailTransactionData = _context.DetailTransaction.Include("Transaction.Bab").Include("Transaction.Bab.Book").Include("Transaction.Bab.Kelompok").Include("Transaction.SubBab").Include("Transaction.SubSubBab").Where(x => x.Id == id).FirstOrDefault();
            editDetTransVM.AspekPedomanData = _context.AspekPedoman.ToList();
            return View(editDetTransVM);
        }
        public JsonResult EditTransaksi(Transaction trans, DetailTransaction detail)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var detailTransaction = _context.DetailTransaction.SingleOrDefault(x => x.Id == detail.Id);
            detailTransaction.Tgl_Berlaku = detail.Tgl_Berlaku;
            detailTransaction.Tgl_JatuhTempo = detail.Tgl_JatuhTempo;
            detailTransaction.Timeline = detail.Timeline;
            detailTransaction.UpdateDate = DateTime.Now;
            _context.Entry(detailTransaction).State = EntityState.Modified;

            _context.SaveChanges();

            var transaction = _context.Transaction.SingleOrDefault(x => x.Id == detailTransaction.TransactionId);
            transaction.BabId = trans.BabId;
            transaction.SubBabId = trans.SubBabId;
            transaction.SubSubBabId = trans.SubSubBabId;
            transaction.UpdateDate = DateTime.Now;
            _context.Entry(transaction).State = EntityState.Modified;

            _context.SaveChanges();

            var historyTransaction = _context.HistoryTransaction.Where(x => x.DetailTransactionId == detail.Id).OrderByDescending(x => x.Id).FirstOrDefault();
            if(historyTransaction.ActivityTimelineId == 2)
            {
                historyTransaction.ProgresStatusId = 2;
                historyTransaction.UpdateDate = DateTime.Now;
                historyTransaction.InputerId = currentUser.Id;
                _context.Entry(historyTransaction).State = EntityState.Modified;

                _context.SaveChanges();
            }
            else
            {
                historyTransaction.ProgresStatusId = 8;
                historyTransaction.UpdateDate = DateTime.Now;
                historyTransaction.ApproveDate = DateTime.Now;
                historyTransaction.ApproverId = currentUser.Id;
                _context.Entry(historyTransaction).State = EntityState.Modified;
                _context.SaveChanges();

                HistoryTransaction historyTransaction2 = new HistoryTransaction();
                historyTransaction2.DetailTransactionId = detailTransaction.Id;
                historyTransaction2.ActivityTimelineId = 2;
                historyTransaction2.AktivitasTargetId = 2;
                historyTransaction2.ProgresStatusId = 2;
                historyTransaction2.CreateDate = DateTime.Now;
                historyTransaction2.InputerId = currentUser.Id;
                _context.HistoryTransaction.Add(historyTransaction2);

                _context.SaveChanges();
            }

            var result = _context.DetailTransaction.SingleOrDefault(x => x.Id == detailTransaction.Id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddSummary(SummaryTransaction summary)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (summary.Id == 0)
            {
                SummaryTransaction summaryTransaction = new SummaryTransaction();
                summaryTransaction.DetailTransactionId = summary.DetailTransactionId;
                summaryTransaction.SubBabProsedurId = summary.SubBabProsedurId;
                summaryTransaction.SubSubBabProsedurId = summary.SubSubBabProsedurId;
                summaryTransaction.AspekPedomanId = summary.AspekPedomanId;
                summaryTransaction.HasilReviewId = summary.HasilReviewId;
                summaryTransaction.UpdatingId = summary.UpdatingId;
                summaryTransaction.DasarUpdatingId = summary.DasarUpdatingId;
                summaryTransaction.AcuanUpdatingId = summary.AcuanUpdatingId;
                summaryTransaction.KetExisting = summary.KetExisting;
                summaryTransaction.KetUpdating = summary.KetUpdating;
                summaryTransaction.Keterangan = summary.Keterangan;
                summaryTransaction.CreateDate = DateTime.Now;
                _context.SummaryTransaction.Add(summaryTransaction);
            }
            else
            {
                var summaryTransaction = _context.SummaryTransaction.SingleOrDefault(x => x.Id == summary.Id);
                summaryTransaction.DetailTransactionId = summary.DetailTransactionId;
                summaryTransaction.SubBabProsedurId = summary.SubBabProsedurId;
                summaryTransaction.SubSubBabProsedurId = summary.SubSubBabProsedurId;
                summaryTransaction.AspekPedomanId = summary.AspekPedomanId;
                summaryTransaction.HasilReviewId = summary.HasilReviewId;
                summaryTransaction.UpdatingId = summary.UpdatingId;
                summaryTransaction.DasarUpdatingId = summary.DasarUpdatingId;
                summaryTransaction.AcuanUpdatingId = summary.AcuanUpdatingId;
                summaryTransaction.KetExisting = summary.KetExisting;
                summaryTransaction.KetUpdating = summary.KetUpdating;
                summaryTransaction.Keterangan = summary.Keterangan;
                summaryTransaction.UpdateDate = DateTime.Now;
                _context.Entry(summaryTransaction).State = EntityState.Modified;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataSummaryById(int id)
        {
            var result = _context.SummaryTransaction.Include("SubSubBabProsedur").Include("SubBabProsedur").Include("HasilReview").Include("Updating").Include("DasarUpdating").Include("AcuanUpdating").Include("AspekPedoman").SingleOrDefault(x => x.Id == id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDataSummary(int id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var summary = _context.SummaryTransaction.SingleOrDefault(x => x.Id == id);
            _context.SummaryTransaction.Remove(summary);

            _context.SaveChanges();

            var progresStatus = 2;
            var count = _context.SummaryTransaction.Where(x => x.DetailTransactionId == summary.DetailTransactionId).Count();
            if (count > 0)
            {
                progresStatus = 3;
            }

            var historyTransaction = _context.HistoryTransaction.Where(x => x.DetailTransactionId == summary.DetailTransactionId).OrderByDescending(x => x.Id).FirstOrDefault();
            historyTransaction.ProgresStatusId = progresStatus;
            historyTransaction.UpdateDate = DateTime.Now;
            historyTransaction.InputerId = currentUser.Id;
            _context.Entry(historyTransaction).State = EntityState.Modified;

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubmitSummary(int id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var historyTransaction = _context.HistoryTransaction.Where(x => x.DetailTransactionId == id).OrderByDescending(x => x.Id).FirstOrDefault();
            historyTransaction.ProgresStatusId = 3;
            historyTransaction.UpdateDate = DateTime.Now;
            historyTransaction.InputerId = currentUser.Id;
            _context.Entry(historyTransaction).State = EntityState.Modified;

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadFiles(SummaryVM summary)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                var idDetail = summary.FileModelVM.IdDetailTrans;
                var detail = _context.DetailTransaction.Include("Transaction").Include("Transaction.Bab").Include("Transaction.SubBab").Include("Transaction.SubSubBab").SingleOrDefault(x => x.Id == idDetail);

                #region Upload
                foreach (HttpPostedFileBase file in summary.FileModelVM.files)
                {
                    if (file != null)
                    {
                        if (file.FileName.EndsWith("docx") || file.FileName.EndsWith("doc"))
                        {
                            var ext = Path.GetExtension(file.FileName);

                            var pathfile = detail.Id + "-" + detail.Transaction.Bab.NamaBab + "-" + detail.Transaction.SubBab.NamaSubBab + "-" + detail.Transaction.SubSubBab.NamaSubSubBab + "-" + ext;
                            string path = Server.MapPath("~//Files/" + pathfile);
                            file.SaveAs(path);

                            if (detail.PathFiles != null)
                            {
                                string paths = Server.MapPath("~/Files/" + detail.PathFiles);
                                System.IO.File.Delete(paths);
                            }

                            detail.PathFiles = pathfile;
                            detail.UpdateDate = DateTime.Now;
                            _context.Entry(detail).State = EntityState.Modified;

                            _context.SaveChanges();
                        }
                    }
                }
                #endregion

                if(detail.PathFiles != null)
                {
                    var historyTransaction = _context.HistoryTransaction.Where(x => x.DetailTransactionId == idDetail).OrderByDescending(x => x.Id).FirstOrDefault();
                    historyTransaction.ProgresStatusId = 6;
                    historyTransaction.UpdateDate = DateTime.Now;
                    historyTransaction.InputerId = currentUser.Id;
                    _context.Entry(historyTransaction).State = EntityState.Modified;

                    _context.SaveChanges();
                }

            }

            return RedirectToAction("Index");
        }
    }
}