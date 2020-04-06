using AplikasiSOP.Models;
using AplikasiSOP.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplikasiSOP.Controllers
{
    public class ReportsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        // GET: Reports
        public ActionResult Index(int id)
        {
            ViewBag.Reports = id;

            return View();
        }

        public JsonResult GetListReport()
        {
            var result = _context.listReport.ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        #region Summary Lampiran NI Persetujuan Update PP (existing updating)
        [HttpPost]
        public void DownloadSummaryPenyusunan(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(57, 51, 50, 34); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Query

            var heading = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    Updating = x.Updating.Nama,
                    Aspek = x.AspekPedoman.Nama,
                    Dasar = x.DasarUpdating.Nama,
                    Buku = x.DetailTransaction.Transaction.Bab.Book.NamaBuku,
                    Bab = x.DetailTransaction.Transaction.Bab.NamaBab,
                    SubBab = x.DetailTransaction.Transaction.SubBab.NamaSubBab,
                    SubSubBab = x.DetailTransaction.Transaction.SubSubBab.NamaSubSubBab
                })
                .FirstOrDefault();

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    Aspek = x.AspekPedoman.Nama,
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    Acuan = x.AcuanUpdating.Nama,
                    KetExisting = x.KetExisting,
                    KetUpdating = x.KetUpdating,
                    Keterangan = x.Keterangan
                })
                .GroupBy(x => new { x.Aspek, x.SubSubBabProsedur, x.Acuan })
                .ToList();

            var buku = heading.Buku;
                buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
                bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            var subbab = heading.SubBab;
                subbab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(subbab.ToLower());

            var subsubbab = heading.SubSubBab;
                subsubbab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(subsubbab.ToLower());

            #endregion

            #region Title
            Chunk chunk = new Chunk("Summary " + heading.Updating + " Dokumen Pedoman Perusahaan", FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 45f, 5f, 150f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 30f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nama Buku PP"), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(buku), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Bab / Sub Bab / Sub Sub Bab"), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(bab + " / " + subbab + " / " +subsubbab), FontFactory.GetFont("Calibri", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Table Summary
            //Table
            table = new PdfPTable(8);
            widths = new float[] { 3f, 3f, 35f, 4f, 48f, 4f, 48f, 50f };
            var backcolour = new BaseColor(146, 205, 220); //RGB see on ms.word template
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 30f;

            #endregion

            #region Header Table Summary
            //Cell Header
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Aspek Penyusunan PP"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Existing"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Updating"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Dasar " + heading.Updating + " PP"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            #endregion

            #region Isi Tabel Summary

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Colspan = 3;
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(i.ToString() + ". "), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().SubSubBabProsedur), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.Border = Rectangle.RIGHT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(i.ToString() + ". "), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().SubSubBabProsedur), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.Border = Rectangle.RIGHT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().Aspek), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.Colspan = 3;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().KetExisting), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().KetUpdating), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().Keterangan), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(" • "), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    cell.Border = Rectangle.LEFT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().SubSubBabProsedur), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Colspan = 2;
                    cell.Border = Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Colspan = 2;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("- "), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = Rectangle.BOTTOM_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.FirstOrDefault().Acuan), FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Colspan = 2;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format("")));
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER;
                    table.AddCell(cell);

                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Summary Lampiran NI Persetujuan Update PP (existing updating).pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region NI Persetujuan Penyusunan PP Baru
        [HttpPost]
        public void NIPersetujuanPenyusunanPPBaru(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(71, 59, 63, 58); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title
            Chunk chunk = new Chunk("Tangerang Selatan, ", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 40f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            #region Query

            var heading = _context.DetailTransaction
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab,
                    Kelompok = x.Transaction.Bab.Kelompok.Singkatan,
                    Wilayah = x.Transaction.Bab.Kelompok.Wilayah
                })
                .FirstOrDefault();

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    SubBabProsedur = x.SubBabProsedur.Nama,
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    KetUpdating = x.KetUpdating,
                })
                .ToList();

            var buku = heading.Buku;
            buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
            bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 25f, 5f, 17f, 153f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 20f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nomor"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("PGO/5.3/      /NI"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Kepada"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Pemimpin Divisi"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("")));
            cell.Border = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("melalui "), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Wilayah + " dan Kelompok " + heading.Kelompok), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Dari"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Kelompok PGO"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("Hal"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Permohonan Persetujuan Update Pedoman Perusahaan (PP) " + buku + " Bab " + bab), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 6
            cell = new PdfPCell(new Phrase(String.Format("Lamp"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("1 (satu) Set"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Isi Notin
            //Paragraf Isi Pertama
            chunk = new Chunk("Menunjuk perihal pada pokok nota intern tersebut diatas, bersama ini kami sampaikan beberapa hal sebagai berikut:", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_JUSTIFIED;
            pdfDoc.Add(para);

            #region Table Buat Penomoran Poin 1

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("1."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Telah dilakukan penyusunan dokumen Pedoman Perusahaan (PP) " + bab + " sebagai Bab baru pada Buku PP " + buku + ", dengan penjelasan sebagai berikut :"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table PP

            table = new PdfPTable(5);
            widths = new float[] { 7f, 45f, 45f, 45f, 58f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            #endregion

            #region Cell Row Buat Header Table PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BUKU PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SUB BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("KETERANGAN PENYUSUNAN"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Cell Row Buat Isi Tabel PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Buku), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Bab), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubBabProsedur), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetUpdating), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cell);
                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);
            #endregion

            #region Table Buat Penomoran Poin 2

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("2."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Sehubungan dengan hal tersebut di atas, terlampir kami sampaikan summary penyusunan dan draft Pedoman Perusahaan dimaksud, serta lembar persetujuan untuk mendapatkan validasi Pemimpin."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            //Paragraf Isi Kedua
            chunk = new Chunk("Demikian kami sampaikan, arahan dan keputusan Pemimpin kami nantikan.", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            //Footer
            chunk = new Chunk("Kelompok Pengembangan Operasional", FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Nota Intern Persetujuan Penyusunan PP Baru.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region NI Persetujuan Penyusunan PP Update
        [HttpPost]
        public void NIPersetujuanPenyusunanPPUpdate(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(71, 59, 63, 58); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title
            Chunk chunk = new Chunk("Tangerang Selatan, ", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 40f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            #region Query

            var heading = _context.DetailTransaction
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab,
                    Kelompok = x.Transaction.Bab.Kelompok.Singkatan,
                    Wilayah = x.Transaction.Bab.Kelompok.Wilayah
                })
                .FirstOrDefault();

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    SubBabProsedur = x.SubBabProsedur.Nama,
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    KetUpdating = x.KetUpdating,
                })
                .ToList();

            var buku = heading.Buku;
            buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
            bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 25f, 5f, 17f, 153f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 20f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nomor"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("PGO/5.3/      /NI"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Kepada"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Pemimpin Divisi"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("")));
            cell.Border = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("melalui "), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Wilayah + " dan Kelompok " + heading.Kelompok), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Dari"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Kelompok PGO"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("Hal"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Permohonan Persetujuan Update Pedoman Perusahaan (PP) " + buku + " Bab " + bab), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 6
            cell = new PdfPCell(new Phrase(String.Format("Lamp"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("1 (satu) Set"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Isi Notin
            //Paragraf Isi Pertama
            chunk = new Chunk("Menunjuk perihal pada pokok nota intern tersebut diatas, bersama ini kami sampaikan beberapa hal sebagai berikut:", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_JUSTIFIED;
            pdfDoc.Add(para);

            #region Table Buat Penomoran Poin 1

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("1."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Telah dilakukan update dokumen Pedoman Perusahaan (PP) " + buku + " Bab " + bab + ", setelah dilakukan review pada dokumen PP tersebut, dengan penjelasan :"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table PP

            table = new PdfPTable(5);
            widths = new float[] { 7f, 45f, 45f, 51f, 52f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            #endregion

            #region Cell Row Buat Header Table PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BUKU PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SUB BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("KETERANGAN UPDATE"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Cell Row Buat Isi Tabel PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Buku), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Bab), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubBabProsedur), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetUpdating), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cell);
                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);
            #endregion

            #region Table Buat Penomoran Poin 2

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("2."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Sehubungan dengan hal tersebut di atas, terlampir kami sampaikan summary update PP, draft Pedoman Perusahaan dimaksud, serta lembar persetujuan untuk mendapatkan validasi Pemimpin."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            //Paragraf Isi Kedua
            chunk = new Chunk("Demikian kami sampaikan, arahan dan keputusan Pemimpin kami nantikan.", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            //Footer
            chunk = new Chunk("Kelompok Pengembangan Operasional", FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Nota Intern Persetujuan Update PP.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        //#region Lembar Persetujuan PPS
        //[HttpPost]
        //public void LembarPersetujuanPPS(int id)
        //{
        //    Document pdfDoc = new Document();
        //    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

        //    pdfDoc.SetPageSize(PageSize.A4);
        //    pdfDoc.SetMargins(43, 28, 43, 28); //( point marginLeft, point marginRight, point marginTop, point marginBottom )

        //    //var content = pdfWriter.DirectContent;
        //    //var pageBorderRect = new Rectangle(pdfDoc.PageSize);

        //    //pageBorderRect.Left += pdfDoc.LeftMargin;
        //    //pageBorderRect.Right -= pdfDoc.RightMargin;
        //    //pageBorderRect.Top -= pdfDoc.TopMargin;
        //    //pageBorderRect.Bottom += pdfDoc.BottomMargin;

        //    //pdfWriter.SetColorStroke(BaseColor.RED);
        //    //pdfWriter.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
        //    pdfDoc.Open();

        //    #region Header
        //    //Table 1
        //    PdfPTable table = new PdfPTable(2);
        //    float[] widths = new float[] { 70f, 30f };
        //    table.SetWidths(widths);
        //    table.WidthPercentage = 100;
        //    table.HorizontalAlignment = Element.ALIGN_CENTER;
        //    table.HorizontalAlignment = 0;
        //    table.SpacingBefore = 20f;
        //    table.SpacingAfter = 40f;

        //    //Cell no 1
        //    PdfPCell cell = new PdfPCell();
        //    cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
        //    cell.Border = 0;
        //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    cell = new PdfPCell();
        //    cell.PaddingTop = 15f;
        //    cell.Border = 0;
        //    Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
        //    //image.ScaleAbsolute(200, 150);
        //    image.ScalePercent(10);
        //    cell.AddElement(image);
        //    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    cell.VerticalAlignment = Element.ALIGN_BOTTOM;
        //    table.AddCell(cell);

        //    //Cell no 2
        //    cell = new PdfPCell(new Phrase(String.Format("LEMBAR PERSETUJUAN"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
        //    cell.PaddingTop = 30f;
        //    cell.Border = 0;
        //    cell.Colspan = 2;
        //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    //Cell no 3
        //    cell = new PdfPCell(new Phrase(String.Format("PEDOMAN PERUSAHAAN SEMENTARA"), FontFactory.GetFont("Arial", 12, Font.NORMAL | Font.UNDERLINE, BaseColor.BLACK)));
        //    cell.Border = 0;
        //    cell.Colspan = 2;
        //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    pdfDoc.Add(table);
        //    #endregion

        //    #region Query

        //    var datenow = _context.DetailTransaction
        //        .Where(x => x.CreateDate.Year == DateTime.Now.Year && x.CreateDate.Month == DateTime.Now.Month && x.CreateDate.Day == DateTime.Now.Day);

        //    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //    var currentUser = manager.FindById(User.Identity.GetUserId());

        //    var query = _context.DetailTransaction.Include("Transaction")
        //        .Include("Transaction.Bab").Include("Transaction.Bab.Book").Include("Transaction.Bab.Kelompok")
        //        .Where(x => x.Id == id)
        //        .Select(x => new
        //        {
        //            Buku = x.Transaction.Bab.Book.NamaBuku,
        //            Bab = x.Transaction.Bab.NamaBab,
        //            Kelompok = x.Transaction.Bab.Kelompok.Nama
        //        })
        //        .FirstOrDefault();

        //    #endregion

        //    //Paragraf 1

        //    var FontColor = new BaseColor(227, 108, 10);
        //    var defaultTextFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
        //    var default2TextFont = FontFactory.GetFont("Arial", 12, Font.ITALIC, BaseColor.BLACK);
        //    var isianTextFont = FontFactory.GetFont("Arial", 12, Font.BOLDITALIC | Font.UNDERLINE, FontColor);

        //    var defaultChunk = new Chunk("Nama Pedoman Perusahaan : ", defaultTextFont);
        //    var BukuChunk = new Chunk((query.Buku), isianTextFont);
        //    var default2Chunk = new Chunk(" BAB ", default2TextFont);
        //    var BabChunk = new Chunk((query.Bab), isianTextFont);

        //    var phrase = new Phrase(defaultChunk);
        //    phrase.Add(BukuChunk);
        //    var phrase2 = new Phrase(default2Chunk);
        //    phrase2.Add(BabChunk);

        //    pdfDoc.Add(phrase);
        //    pdfDoc.Add(phrase2);

        //    #region TTD 1
        //    //TTD
        //    Chunk chunk = new Chunk("Dipersiapkan oleh, ", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    Paragraph para = new Paragraph();
        //    para.Add(chunk);
        //    para.Alignment = Element.ALIGN_CENTER;
        //    para.SpacingBefore = 30f;
        //    para.SpacingAfter = 20f;
        //    pdfDoc.Add(para);

        //    chunk = new Chunk("Tanggal, " /*+ datevm.startdate.ToString("d MMMM yyyy", new CultureInfo("id-ID"))*/, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.Alignment = Element.ALIGN_CENTER;
        //    para.SpacingBefore = 10f;
        //    para.SpacingAfter = 80f;
        //    pdfDoc.Add(para);

        //    chunk = new Chunk("(......................................)", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.Alignment = Element.ALIGN_CENTER;
        //    para.SpacingBefore = 10f;
        //    para.SpacingAfter = 20f;
        //    pdfDoc.Add(para);

        //    #endregion

        //    //Paragraf 3
        //    chunk = new Chunk("Berdasarkan validasi serta masukan dari Divisi/Satuan/Unit terkait diantaranya:", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.SpacingBefore = 10f;
        //    pdfDoc.Add(para);

        //    //Table 2
        //    table = new PdfPTable(2);
        //    widths = new float[] { 7f, 193f };
        //    table.SetWidths(widths);
        //    table.WidthPercentage = 100;
        //    table.HorizontalAlignment = Element.ALIGN_LEFT;
        //    table.SpacingAfter = 30f;

        //    //Cell Row 1
        //    cell = new PdfPCell(new Phrase(String.Format("-"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
        //    cell.Border = 0;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    cell = new PdfPCell(new Phrase(String.Format("Kelompok Rekonsilialisasi dan Pembayaran " + "(" + query.Kelompok + ")", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK))));
        //    cell.Border = 0;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    //Cell Row 2
        //    cell = new PdfPCell(new Phrase(String.Format("-"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
        //    cell.Border = 0;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    cell = new PdfPCell(new Phrase(String.Format("Kelompok Pengembangan Operasional (PGO)"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
        //    cell.Border = 0;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    table.AddCell(cell);

        //    pdfDoc.Add(table);

        //    //Paragraf 4
        //    chunk = new Chunk("maka ditetapkan Pedoman Perusahaan tersebut di atas oleh:", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.SpacingBefore = 20f;
        //    pdfDoc.Add(para);

        //    #region TTD 2
        //    //TTD 2
        //    chunk = new Chunk("Tanggal, " /*+ datevm.startdate.ToString("d MMMM yyyy", new CultureInfo("id-ID"))*/, FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.Alignment = Element.ALIGN_CENTER;
        //    para.SpacingBefore = 10f;
        //    para.SpacingAfter = 80f;
        //    pdfDoc.Add(para);

        //    chunk = new Chunk("(......................................)", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
        //    para = new Paragraph();
        //    para.Add(chunk);
        //    para.Alignment = Element.ALIGN_CENTER;
        //    para.SpacingBefore = 10f;
        //    para.SpacingAfter = 10f;
        //    pdfDoc.Add(para);

        //    #endregion

        //    pdfWriter.CloseStream = false;
        //    pdfDoc.Close();
        //    Response.Buffer = true;
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=Lembar Persetujuan PPS.pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();
        //}

        //#endregion

        #region Lembar Persetujuan PP
        [HttpPost]
        public void LembarPersetujuanPP(int id)
        {
            
            

            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(43, 28, 43, 28); //( point marginLeft, point marginRight, point marginTop, point marginBottom )

            //layout.Left += pdfDoc.LeftMargin;
            //layout.Right -= pdfDoc.RightMargin;
            //layout.Top -= pdfDoc.TopMargin;
            //layout.Bottom += pdfDoc.BottomMargin;
            Rectangle layout = new Rectangle(pdfDoc.PageSize);
            //layout = pdfDoc.SetMargins(43, 28, 43, 28);
            layout.BorderColor = BaseColor.BLACK;
            layout.BorderWidth = 6;
            layout.Border = Rectangle.BOX;

            //layout.setLeft(layout.getLeft() + 10);
            //pageRect.setRight(pageRect.getRight() - 10);
            //pageRect.setTop(pageRect.getTop() - 10);
            //pageRect.setBottom(pageRect.getBottom() + 10)



            //PdfContentByte content = writer.getDirectContent();
            //Rectangle pageRect = doc.getPageSize();

            //pageRect.setLeft(pageRect.getLeft() + 10);
            //pageRect.setRight(pageRect.getRight() - 10);
            //pageRect.setTop(pageRect.getTop() - 10);
            //pageRect.setBottom(pageRect.getBottom() + 10);

            //content.setColorStroke(Color.red);
            //content.rectangle(pageRect.getLeft(), pageRect.getBottom(), pageRect.getWidth(), pageRect.getHeight());
            //content.stroke();

            //base.LembarPersetujuanPP(id);

            //var content = pdfWriter.DirectContent;
            //var pageBorderRect = new Rectangle(pdfDoc.PageSize);

            //pageBorderRect.Left += pdfDoc.LeftMargin;
            //pageBorderRect.Right -= pdfDoc.RightMargin;
            //pageBorderRect.Top -= pdfDoc.TopMargin;
            //pageBorderRect.Bottom += pdfDoc.BottomMargin;

            //content.SetColorStroke(BaseColor.BLACK);
            //content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            //content.Stroke();

            pdfDoc.Open();

            #region Header
            //Table 1
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 70f, 30f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 40f;

            //Cell no 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.PaddingTop = 15f;
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
            //image.ScaleAbsolute(200, 150);
            image.ScalePercent(10);
            cell.AddElement(image);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cell);

            //Cell no 2
            cell = new PdfPCell(new Phrase(String.Format("LEMBAR PERSETUJUAN"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 30f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell no 3
            cell = new PdfPCell(new Phrase(String.Format("PEDOMAN PERUSAHAAN"), FontFactory.GetFont("Arial", 12, Font.NORMAL | Font.UNDERLINE, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Query

            var query = _context.DetailTransaction.Include("Transaction")
                .Include("Transaction.Bab").Include("Transaction.Bab.Book").Include("Transaction.Bab.Kelompok")
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab,
                    Kelompok = x.Transaction.Bab.Kelompok.Nama,
                    Singkatan = x.Transaction.Bab.Kelompok.Singkatan
                })
                .FirstOrDefault();

            #endregion

            //Paragraf 1

            //var FontColor = new BaseColor(227, 108, 10);
            var defaultTextFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            var default2TextFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            var isianTextFont = FontFactory.GetFont("Arial", 12, Font.NORMAL | Font.UNDERLINE);

            var defaultChunk = new Chunk("Nama Pedoman Perusahaan : ", defaultTextFont);
            var BukuChunk = new Chunk((query.Buku), isianTextFont);
            var default2Chunk = new Chunk(" BAB ", default2TextFont);
            var BabChunk = new Chunk((query.Bab), isianTextFont);

            var phrase = new Phrase(defaultChunk);
            phrase.Add(BukuChunk);
            var phrase2 = new Phrase(default2Chunk);
            phrase2.Add(BabChunk);

            pdfDoc.Add(phrase);
            pdfDoc.Add(phrase2);

            #region TTD 1
            //TTD
            Chunk chunk = new Chunk("Dipersiapkan oleh, ", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 30f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            chunk = new Chunk("Tanggal,", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 80f;
            pdfDoc.Add(para);

            chunk = new Chunk("(............................................)", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            //Paragraf 3
            chunk = new Chunk("Berdasarkan kajian serta masukan (validasi) dari Divisi/Satuan/Unit terkait diantaranya:", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            //Table 2
            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.SpacingAfter = 30f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("- "), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(query.Kelompok + " " + query.Singkatan), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("- "), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Kelompok Pengembangan Operasional (PGO)"), FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);

            //Paragraf 4
            chunk = new Chunk("maka ditetapkan Pedoman Perusahaan tersebut di atas oleh:", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.SpacingBefore = 20f;
            pdfDoc.Add(para);

            #region TTD 2
            //TTD 2
            chunk = new Chunk("Tanggal,", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 80f;
            pdfDoc.Add(para);

            chunk = new Chunk("(............................................)", FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Lembar Persetujuan.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region Memo Penyampaian PP Baru
        [HttpPost]
        public void MemoPenyampaianPPBaru(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(64, 66, 63, 58); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title
            Chunk chunk = new Chunk("Tangerang Selatan, ", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 40f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            #region Query

            var heading = _context.DetailTransaction
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab
                })
                .FirstOrDefault();

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    SubBabProsedur = x.SubBabProsedur.Nama,
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    KetUpdating = x.KetUpdating,
                })
                .ToList();

            var buku = heading.Buku;
            buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
            bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 25f, 5f, 170f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 20f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nomor"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("OPR/5.3/"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Kepada"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("DIVISI TATA KELOLA KEBIJAKAN (PGV)"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Dari"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("DIVISI OPERASIONAL (OPR)"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Hal"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Penyampaian Dokumen Pedoman Perusahaan (PP) " + buku + " Bab " + bab), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("Lamp"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("1 (satu) Set"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Isi Notin
            //Paragraf Isi Pertama
            chunk = new Chunk("Menunjuk perihal pada pokok memo tersebut diatas, dengan ini kami sampaikan beberapa hal sebagai berikut:", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_JUSTIFIED;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Table Buat Penomoran Poin 1

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("1."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Telah dilakukan penyusunan dokumen Pedoman Perusahaan (PP) " + buku + " Bab " + bab + " sebagai Bab baru dengan penjelasan:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table PP

            table = new PdfPTable(5);
            widths = new float[] { 7f, 45f, 45f, 51f, 52f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            #endregion

            #region Cell Row Buat Header Table PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BUKU PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SUB BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("KETERANGAN PENYUSUNAN"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Cell Row Buat Isi Tabel PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Buku), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Bab), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubBabProsedur), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetUpdating), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cell);
                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);
            #endregion

            #region Table Buat Penomoran Poin 2

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("2."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Sehubungan dengan hal tersebut, terlampir kami sampaikan softcopy & hardcopy PP dimaksud berikut review – released dokumen PP, serta lembar persetujuan dan tanda terima (softcopy kami email ke alamat bniepp@bni.co.id). Selanjutnya, dimohon kerja samanya untuk:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table Anak Penomoran Poin 2
            table = new PdfPTable(3);
            widths = new float[] { 7f, 7f, 186f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("")));
            cell.Border = 0;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("a."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Melakukan upload dan sertifikasi atas dokumen PP " + buku + " – Bab " + bab + " pada aplikasi BNI e-PP."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("b."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 6
            cell = new PdfPCell(new Phrase(String.Format("Mengembalikan tanda terima dokumen PP tersebut pada kesempatan pertama."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table Buat Penomoran Poin 3

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("3."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Apabila membutuhkan konfirmasi atau informasi lebih lanjut, dapat menghubungi staff kami:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table di dalam penomoran poin 3

            table = new PdfPTable(5);
            widths = new float[] { 7f, 10f, 58f, 58f, 67f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;

            //HEADER
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("No."), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nama"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Telepon"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("E-mail"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.Rowspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("1"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Ilona Jeane Florence"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("021-80826840 ext.8556"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("ilona.jf@bni.co.id"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("2"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Adriane Herliandari"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("021-80826840 ext.8551"), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("adriane.herliandari@bni.co.id "), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            //Paragraf Isi Kedua
            chunk = new Chunk("Demikian kami sampaikan, atas perhatian dan kerja samanya diucapkan terima kasih.", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Footer
            chunk = new Chunk("DIVISI OPERASIONAL", FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Memo Penyampaian PP Baru.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region Memo Penyampaian Update PP
        [HttpPost]
        public void MemoPenyampaianUpdatePP(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(64, 66, 63, 58); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title
            Chunk chunk = new Chunk("Tangerang Selatan, ", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 40f;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            #region Query

            var heading = _context.DetailTransaction
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab
                })
                .FirstOrDefault();

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    SubBabProsedur = x.SubBabProsedur.Nama,
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    KetUpdating = x.KetUpdating,
                })
                .ToList();

            var buku = heading.Buku;
                buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
                bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 25f, 5f, 170f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 20f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nomor"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("OPR/5.3/"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Kepada"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("DIVISI TATA KELOLA KEBIJAKAN (PGV)"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Dari"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("DIVISI OPERASIONAL (OPR)"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Hal"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Penyampaian Update Dokumen Pedoman Perusahaan (PP) " + buku + " Bab " + bab), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("Lamp"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("1 (satu) Set"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Isi Notin
            //Paragraf Isi Pertama
            chunk = new Chunk("Menunjuk perihal pada pokok memo tersebut diatas, dengan ini kami sampaikan beberapa hal sebagai berikut:", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_JUSTIFIED;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            #region Table Buat Penomoran Poin 1

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("1."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Telah dilakukan update pada dokumen Pedoman Perusahaan (PP) " + buku + " Bab " + bab + ", dengan penjelasan berikut:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table PP

            table = new PdfPTable(5);
            widths = new float[] { 7f, 45f, 45f, 51f, 52f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            #endregion

            #region Cell Row Buat Header Table PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BUKU PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SUB BAB PP"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("KETERANGAN UPDATE"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Cell Row Buat Isi Tabel PP

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Buku), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(heading.Bab), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubBabProsedur), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetUpdating), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cell);
                    i++;
                }
            }
            else
            {
                cell = new PdfPCell(new Phrase(String.Format("Data Tidak tersedia"), FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK)));
                cell.Colspan = 4;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
            }

            pdfDoc.Add(table);
            #endregion

            #region Table Buat Penomoran Poin 2

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("2."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Sehubungan dengan hal tersebut, terlampir kami sampaikan softcopy & hardcopy PP dimaksud berikut perbandingan antara e-PP existing dan updating, lembar persetujuan dan tanda terima (softcopy kami email ke alamat bniepp@bni.co.id). Selanjutnya, dimohon kerja samanya untuk:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table Anak Penomoran Poin 2
            table = new PdfPTable(3);
            widths = new float[] { 7f, 7f, 186f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("")));
            cell.Border = 0;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("a."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Melakukan upload dan sertifikasi atas dokumen update PP " + buku + " – Bab " + bab + " ke dalam aplikasi BNI e-PP."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format("b."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 6
            cell = new PdfPCell(new Phrase(String.Format("Mengembalikan tanda terima dokumen PP tersebut pada kesempatan pertama."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table Buat Penomoran Poin 3

            table = new PdfPTable(2);
            widths = new float[] { 7f, 193f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("3."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Apabila membutuhkan konfirmasi atau informasi lebih lanjut, dapat menghubungi staff kami:"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion

            #region Table di dalam penomoran poin 3

            table = new PdfPTable(5);
            widths = new float[] { 7f, 10f, 58f, 58f, 67f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;

            //HEADER
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("No."), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Nama"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Telepon"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("E-mail"), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #region Query isi PPM

            var ppm = _context.PPM.ToList();

            #endregion

            #region Isi Tabel PPM
            int j = 1;
            if (ppm != null)
            {
                foreach (var ppm2 in ppm)
                {
                    cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(j.ToString()), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(ppm2.Nama), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(ppm2.Telepon), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(ppm2.Email), FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    j++;
                }
            }


            pdfDoc.Add(table);

            #endregion
            
            #endregion

            //Paragraf Isi Kedua
            chunk = new Chunk("Demikian kami sampaikan, atas perhatian dan kerja samanya diucapkan terima kasih.", FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            //Footer
            chunk = new Chunk("DIVISI OPERASIONAL", FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 10f;
            pdfDoc.Add(para);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Memo Penyampaian Update PP.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region Summary Lampiran Memo Penyampaian Update PP (existing updating)
        [HttpPost]
        public void SummaryLampiranMemoPenyampaianUpdate(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            pdfDoc.SetMargins(57, 51, 50, 34); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title
            Chunk chunk = new Chunk("Lampiran Memo No. ……………………………", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            Paragraph para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingBefore = 20f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            chunk = new Chunk("Perbandingan antara e-PP Existing dan Updating - Dokumen PP", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            para = new Paragraph();
            para.Add(chunk);
            para.Alignment = Element.ALIGN_LEFT;
            para.SpacingAfter = 20f;
            pdfDoc.Add(para);

            #endregion

            #region Query Heading

            var heading = _context.DetailTransaction.Include("Transaction")
                .Include("Transaction.Bab").Include("Transaction.Bab.Book").Include("Transaction.SubBab").Include("Transaction.SubSubBab")
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    Buku = x.Transaction.Bab.Book.NamaBuku,
                    Bab = x.Transaction.Bab.NamaBab,
                    SubBab = x.Transaction.SubBab.NamaSubBab,
                    SubSubBab = x.Transaction.SubSubBab.NamaSubSubBab
                })
                .FirstOrDefault();

            var buku = heading.Buku;
            buku = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(buku.ToLower());

            var bab = heading.Bab;
            bab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(bab.ToLower());

            var subbab = heading.SubBab;
            subbab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(subbab.ToLower());

            var subsubbab = heading.SubSubBab;
            subsubbab = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(subsubbab.ToLower());

            #endregion

            #region Heading
            //Table 1
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 45f, 5f, 150f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Nama PP"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(buku), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Bab / Sub Bab / Sub Sub Bab"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(bab + " / " + subbab + " / " + subsubbab), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);
            #endregion

            #region Table Summary
            //Table
            table = new PdfPTable(5);
            widths = new float[] { 4f, 65f, 4f, 65f, 47f };
            var backcolour = new BaseColor(146, 205, 220);
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 20f;

            #endregion

            #region Header Table Summary

            //Cell Header
            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("Existing"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Updating"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Dasar Perubahan"), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            #endregion

            #region Query isi

            var isi = _context.SummaryTransaction
                .Where(x => x.DetailTransactionId == id)
                .Select(x => new
                {
                    SubSubBabProsedur = x.SubSubBabProsedur.Nama,
                    KetExisting = x.KetExisting,
                    KetUpdating = x.KetUpdating,
                    Keterangan = x.Keterangan
                })
                .ToList();

            #endregion

            #region Isi Tabel Summary

            int i = 1;
            if (isi != null)
            {
                foreach (var isi2 in isi)
                {
                    cell = new PdfPCell(new Phrase(String.Format(i.ToString() + ". "), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubSubBabProsedur), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(i.ToString() + ". "), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    cell.Border = Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.SubSubBabProsedur), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.TOP_BORDER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.Keterangan), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_TOP;
                    cell.Rowspan = 2;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetExisting), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(String.Format(isi2.KetUpdating), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.BOTTOM_BORDER;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    i++;
                }
            }


            pdfDoc.Add(table);

            #endregion

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Summary Lampiran Memo Penyampaian Update PP (existing updating).pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        #endregion

        #region Form Serah Terima Soft Copy Dokumen e-PP
        [HttpPost]
        public ActionResult FormSerahTerima(int id)
        {
            Document pdfDoc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

            pdfDoc.SetPageSize(PageSize.A4);
            pdfDoc.SetMargins(62, 62, 25, 72); //( point marginLeft, point marginRight, point marginTop, point marginBottom )
            pdfDoc.Open();

            #region Title and LOGO
            //Table 1
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 70f, 30f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;

            //Cell Row 1
            PdfPCell cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.PaddingTop = 15f;
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/BNI_logo.png"));
            image.ScalePercent(10);
            cell.AddElement(image);
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("PT Bank Negara Indonesia (Persero) Tbk."), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.PaddingTop = 30f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Formulir Serah Terima Softcopy Dokumen yang Akan di Upload di Aplikasi BNI e-PP"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.PaddingTop = 10f;
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion


            #region Header
            //Table 2
            table = new PdfPTable(9);
            widths = new float[] { 41f, 3f, 75f, 12f, 3f, 20f, 10f, 3f, 33f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingAfter = 10f;

            #region Header Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Hari / Tanggal"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("( Nama Hari ) / ( Tanggal Cetak Memo )"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("Divisi / Kelompok"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Operasional / ( Kelompok Pemilik PP )"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 3
            cell = new PdfPCell(new Phrase(String.Format("Memo"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("OPR/5.3/"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Tgl. Memo"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 3;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(free text)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 4
            cell = new PdfPCell(new Phrase(String.Format("Nama Dokumen"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Pedoman Perusahaan (Nama Buku PP) – (Nama Bab PP) (Buku / File) *)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 5
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("*) pilih salah satu/coret yang tidak benar"), FontFactory.GetFont("Arial", 10, Font.ITALIC, BaseColor.BLUE)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 6
            cell = new PdfPCell(new Phrase(String.Format("Jenis Dokumen"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("- Migrasi & Updating (baik dari PP Online atau hardcopy)/Baru *)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 7
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("- Pedoman Perusahaan / Pedoman Perusahaan Sementara / Internal/Eksternal *)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 8
            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("*) pilih salah satu/coret yang tidak benar"), FontFactory.GetFont("Arial", 10, Font.ITALIC, BaseColor.BLUE)));
            cell.Colspan = 7;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 9
            cell = new PdfPCell(new Phrase(String.Format("PIC 1"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(Nama PIC Kelompok Pemilik PP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("NPP"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(NPP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Ext"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(nomor telp)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 10
            cell = new PdfPCell(new Phrase(String.Format("PIC 2"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(Nama PIC PPM)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("NPP"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(NPP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Ext"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(nomor telp)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 11
            cell = new PdfPCell(new Phrase(String.Format("Validator 1"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(Nama Pimkel Pemilik PP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("NPP"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(NPP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Ext"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(nomor telp)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            #region Header Cell Row 12
            cell = new PdfPCell(new Phrase(String.Format("Validator 2"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(Nama DGM sesuai grouping)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("NPP"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(NPP)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Ext"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(":"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("(nomor telp)"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            pdfDoc.Add(table);

            #endregion

            #region Table Form Serah Terima

            table = new PdfPTable(5);
            widths = new float[] { 10f, 65f, 40f, 40f, 45f };
            var backcolour = new BaseColor(217, 217, 217); //RGB see on ms.word template
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 10f;

            #region Header Table Form Serah Terima

            cell = new PdfPCell();
            cell = new PdfPCell(new Phrase(String.Format("NO"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("NAMA FOLDER / FILE"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("SIZE"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("CONTAINS"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("CREATED"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = backcolour;
            table.AddCell(cell);

            #endregion

            #region Isi Table Form Serah Terima

            cell = new PdfPCell(new Phrase(String.Format("1."), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.Rowspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Pedoman Perusahaan (Nama Buku PP ) – Bab ( Nama Bab PP ) dengan nama file  free text "), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("free text"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("free text"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("free text"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            #endregion

            pdfDoc.Add(table);

            #endregion

            #region TTD
            //Table
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.SpacingBefore = 20f;

            //Cell Row 1
            cell = new PdfPCell(new Phrase(String.Format("Mengetahui,"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Yang Menyerahkan,"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("Yang Menerima,"), FontFactory.GetFont("Arial", 11, Font.BOLD, BaseColor.BLACK)));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cell);

            //Cell Row 2
            cell = new PdfPCell(new Phrase(String.Format("free text"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format("free text"), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Format(""), FontFactory.GetFont("Arial", 11, Font.NORMAL, BaseColor.BLACK)));
            cell.PaddingTop = 100f;
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            table.AddCell(cell);

            pdfDoc.Add(table);

            #endregion


            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Form Serah Terima Softcopy Dokumen.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return View();
        }

        #endregion

        [HttpPost]
        public ActionResult Reset()
        {
            return RedirectToAction("Index");
        }
    }
}