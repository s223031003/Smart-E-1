using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_E.Data;
using Smart_E.Models;

namespace Smart_E.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public FileResult Export()
        {
            DataTable dt = new DataTable("Transactions");
            dt.Columns.AddRange(new DataColumn[6]
            {
                new DataColumn("AccountNumber"),
                new DataColumn("AccountOwner"),
                new DataColumn("BankName"),
                new DataColumn("CVV"),
                new DataColumn("Amount"),
                new DataColumn("Date")
            });

            var transaction = from t in _context.Transactions.ToList() select t;
            foreach (var t in transaction)
            {
                dt.Rows.Add(t.AccountNumber, t.AccountOwner, t.BankName, t.CVV, t.Amount,t.Date);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml", "Transactions.xlsx");
                }
            }
        }

        //search transaction
        //[HttpGet]
        //public async Task<IActionResult> Transactions(string search)
        //{
        //    ViewData["Details"] = search;

        //    var query = from t in _context.Transactions select t;
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        query = query.Where(s => s.AccountOwner.Contains(search));
        //    }
        //    return View(await query.AsNoTracking().ToListAsync());
        //}

        // GET: Transaction
        public async Task<IActionResult> Transactions()
        {
            return View(await _context.Transactions.ToListAsync());

           // return View()
;        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transactionsModel = await _context.Transactions
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transactionsModel == null)
            {
                return NotFound();
            }

            return View(transactionsModel);
        }

        // GET: Transaction/AddOrEdit
        // GET: Transaction/AddOrEdit/5
        public IActionResult Create()
        {
            return View(new TransactionsModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,AccountNumber,AccountOwner,BankName,CVV,Amount,Date")] TransactionsModel transactionsModel)
        {
            if (ModelState.IsValid)
            {
                    transactionsModel.Date = DateTime.Now;
                    _context.Add(transactionsModel);
                    await _context.SaveChangesAsync();
                
                return RedirectToAction("ThankYou");
            }
            return BadRequest("Please fill all the fields.");
        }
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
