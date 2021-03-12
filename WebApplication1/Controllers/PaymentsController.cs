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
    public class PaymentsController : Controller
    {
        private readonly BOOKSTOREContext _context;

        public PaymentsController(BOOKSTOREContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Done(int? id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();
            _context.Payments.Find(customerId).Amount = (int)id;
            return RedirectToAction("Edit", "Customers");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Navigate(int? id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();
            var payment = _context.Payments.Find(customerId);
            _context.Payments.Remove(payment);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        // GET: Payments
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Create
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
            var chart = _context.Charts.Find(customerId);

            var payment = new Payment()
            {
                Amount = (int)chart.Cost
            };
            _context.Add(payment);
                 
            await _context.SaveChangesAsync();
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(customerId);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,Amount")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel()
        {
            return RedirectToAction("Home", "Index");
        }
        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}
