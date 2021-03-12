using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ChartsController : Controller
    {
        private readonly BOOKSTOREContext _context;

        public ChartsController(BOOKSTOREContext context)
        {
            _context = context;
        }

        // GET: Charts
        public async Task<IActionResult> Index()
        {
            var bOOKSTOREContext = _context.Charts.Include(c => c.Customer);
            return View(await bOOKSTOREContext.ToListAsync());
        }

        // GET: Charts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chart = await _context.Charts
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (chart == null)
            {
                return NotFound();
            }

            return View(chart);
        }

        // GET: Charts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address");
            return View();
        }

        // POST: Charts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Cost")] Chart chart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", chart.CustomerId);
            return View(chart);
        }

        // GET: Charts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chart = await _context.Charts.FindAsync(id);
            if (chart == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", chart.CustomerId);
            return View(chart);
        }

        // POST: Charts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Cost")] Chart chart)
        {
            if (id != chart.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartExists(chart.CustomerId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", chart.CustomerId);
            return View(chart);
        }

        // GET: Charts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chart = await _context.Charts
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (chart == null)
            {
                return NotFound();
            }

            return View(chart);
        }

        // POST: Charts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chart = await _context.Charts.FindAsync(id);
            _context.Charts.Remove(chart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartExists(int id)
        {
            return _context.Charts.Any(e => e.CustomerId == id);
        }
        [Authorize]
        public IActionResult Check()
        {

            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var chart = _context.Charts.Find(customerId);

            if (chart != null)
            {
                // TempData["details"] = customerId;
                return RedirectToAction("Index", "ChartLines");
            }

            else
            {
                return RedirectToAction("Check", "Login");
            }

        }
    }
}
