using System;
using System.IO;
using System.Data;
using System.Web;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollWebApp.Data;
using PayrollWebApp.Models;
using Microsoft.AspNetCore.Http;
using Grpc.Core;
using CsvHelper;
using CsvHelper.Configuration;

namespace PayrollWebApp.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Payrolls
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["EmployeeId"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["FirstName"] = sortOrder == "First" ? "first_desc" : "First";
            ViewData["LastName"] = sortOrder == "Last" ? "last_desc" : "Last";
            ViewData["PayrollError"] = sortOrder == "Error" ? "error_desc" : "Error";

            var payroll = from s in _context.Payroll
                          select s;

            switch (sortOrder)
            {
                case "id_desc":
                    payroll = payroll.OrderByDescending(s => s.EmployeeId);
                    break;
                case "First":
                    payroll = payroll.OrderBy(s => s.FirstName);
                    break;
                case "first_desc":
                    payroll = payroll.OrderByDescending(s => s.FirstName);
                    break;
                case "Last":
                    payroll = payroll.OrderBy(s => s.LastName);
                    break;
                case "last_desc":
                    payroll = payroll.OrderByDescending(s => s.LastName);
                    break;
                case "Error":
                    payroll = payroll.OrderBy(s => s.PayrollError);
                    break;
                case "error_desc":
                    payroll = payroll.OrderByDescending(s => s.PayrollError);
                    break;
                default:
                    payroll = payroll.OrderBy(s => s.EmployeeId);
                    break;
            }

            return View(await payroll.ToListAsync());
        }

        // GET: Payrolls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payroll
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // GET: Payrolls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payrolls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile csvFile)
        {
            //var payrolls = new Payroll();
            using (var stream = new StreamReader(csvFile.OpenReadStream()))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    HeaderValidated = null,
                };
                using (var csv = new CsvReader(stream, config))
                {
                    csv.Context.RegisterClassMap<PayrollMap>();
                    var payroll = csv.GetRecords<Payroll>().ToList();
                    foreach (var r in payroll)
                    {
                        _context.Add(r);
                        await _context.SaveChangesAsync();
                    }
                }
            }
             
             return RedirectToAction(nameof(Index));
        }

        // GET: Payrolls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payroll.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }
            return View(payroll);
        }

        // POST: Payrolls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,PayrollError")] Payroll payroll)
        {
            if (id != payroll.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payroll);
        }

        // GET: Payrolls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payroll
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Payrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payroll.FindAsync(id);
            _context.Payroll.Remove(payroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayrollExists(int id)
        {
            return _context.Payroll.Any(e => e.EmployeeId == id);
        }
    }
}
