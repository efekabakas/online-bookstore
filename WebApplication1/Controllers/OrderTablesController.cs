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
    [Authorize]
    public class OrderTablesController : Controller
    {
        private readonly BOOKSTOREContext _context;

        public OrderTablesController(BOOKSTOREContext context)
        {
            _context = context;
        }

        // GET: OrderTables
        public IActionResult Index(OrderTable ot)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var lines = _context.ChartLines
                .Where(c => c.ChartId == customerId);

            foreach (var i in lines)
            {
                _context.ChartLines.Remove(i);
            }
            _context.Charts.Find(customerId).Cost = 0;
            _context.SaveChangesAsync();
                


            return View(ot);
        }

        public async Task<IActionResult> Orders()
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();


            var bOOKSTOREContext = _context.OrderTables
                .Where(cl => cl.CustomerId == customerId);
            return View(await bOOKSTOREContext.ToListAsync());
        }

        // GET: OrderTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orderTable = await _context.OrderTables
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderTable == null)
            {
                return NotFound();
            }

            return View(orderTable);
        }

        // POST: OrderTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var orderTable = new OrderTable()
            {
                CustomerId = customerId,
                Customer = _context.Customers.Find(customerId),
                OrderDate = DateTime.Now,
                TotalPrice = (int)_context.Charts.Find(customerId).Cost,
            };
            _context.Add(orderTable);
            await _context.SaveChangesAsync();

            foreach (var i in _context.ChartLines.Where(c => c.ChartId == customerId)){
                var orderLine = new OrderLine()
                {
                    OrderId = orderTable.OrderId,
                    ProductId = i.ProductId,                  
                    Quantity = i.Quantity
                };
                _context.Add(orderLine);
            };
           
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","OrderTables", orderTable);
        }

        // GET: OrderTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", orderTable.CustomerId);
            return View(orderTable);
        }

        // POST: OrderTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TotalPrice,OrderId,CustomerId,OrderDate")] OrderTable orderTable)
        {
            if (id != orderTable.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderTableExists(orderTable.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Address", orderTable.CustomerId);
            return View(orderTable);
        }

        // GET: OrderTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderTable = await _context.OrderTables
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderTable == null)
            {
                return NotFound();
            }

            return View(orderTable);
        }

        // POST: OrderTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderTable = await _context.OrderTables.FindAsync(id);
            _context.OrderTables.Remove(orderTable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderTableExists(int id)
        {
            return _context.OrderTables.Any(e => e.OrderId == id);
        }
    }
}
