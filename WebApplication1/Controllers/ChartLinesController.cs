using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ChartLinesController : Controller
    {
        private readonly BOOKSTOREContext _context;

        public ChartLinesController(BOOKSTOREContext context)
        {
            _context = context;
        }

        // GET: ChartLines
        public async Task<IActionResult> Index()
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();


            var bOOKSTOREContext = _context.ChartLines
                .Include(c => c.Chart).Include(c => c.Product)
                .Where(cl => cl.ChartId == customerId);
            return View(await bOOKSTOREContext.ToListAsync());
        }



        // GET: ChartLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartLine = await _context.ChartLines
                .Include(c => c.Chart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.ChartId == id);
            if (chartLine == null)
            {
                return NotFound();
            }

            return View(chartLine);
        }

        // GET: ChartLines/Create
        public IActionResult Create()
        {
            ViewData["ChartId"] = new SelectList(_context.Charts, "CustomerId", "CustomerId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Genre");
            return View();
        }

        // POST: ChartLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChartId,Quantity,ProductId")] ChartLine chartLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChartId"] = new SelectList(_context.Charts, "CustomerId", "CustomerId", chartLine.ChartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Genre", chartLine.ProductId);
            return View(chartLine);
        }




        public async Task<IActionResult> Edit(int? id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();
            var chart = _context.Charts.Find(customerId);
            var product = _context.Products.Find(id);
            var chartLine = _context.ChartLines.Find(customerId, id);
            chartLine.Quantity++;
            chart.Cost += (int)product.Price;
            _context.Update(chartLine);
            _context.Update(chart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ChartLines");
        }

        // GET: ChartLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();
            var chart = _context.Charts.Find(customerId);
            var product = _context.Products.Find(id);
            var chartLine = _context.ChartLines.Find(customerId, id);
            chart.Cost -= (int)chartLine.Quantity * (int)product.Price;
            _context.ChartLines.Remove(chartLine);
            _context.Update(chart);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ChartLines");
        }

    }
}
