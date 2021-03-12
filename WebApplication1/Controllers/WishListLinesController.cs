using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class WishListLinesController : Controller
    {
        private readonly BOOKSTOREContext _context;

        public WishListLinesController(BOOKSTOREContext context)
        {
            _context = context;
        }

        // GET: WishListLines
        public async Task<IActionResult> Index()
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var bOOKSTOREContext = _context.WishListLines.Include(w => w.Product).Include(w => w.WistList).Where(a => a.WistListId == customerId);
            return View(await bOOKSTOREContext.ToListAsync());
        }

        // GET: WishListLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishListLine = await _context.WishListLines
                .Include(w => w.Product)
                .Include(w => w.WistList)
                .FirstOrDefaultAsync(m => m.WistListId == id);
            if (wishListLine == null)
            {
                return NotFound();
            }

            return View(wishListLine);
        }

        // GET: WishListLines/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Genre");
            ViewData["WistListId"] = new SelectList(_context.WishLists, "CustomerId", "CustomerId");
            return View();
        }

        // POST: WishListLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WistListId,ProductId,Quantity")] WishListLine wishListLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wishListLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Genre", wishListLine.ProductId);
            ViewData["WistListId"] = new SelectList(_context.WishLists, "CustomerId", "CustomerId", wishListLine.WistListId);
            return View(wishListLine);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var wishlistline = await _context.WishListLines.FindAsync(customerId, id);
            _context.WishListLines.Remove(wishlistline);
            _context.Update(_context.WishLists.Find(customerId));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "WishlistLines");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var wishlistline = await _context.WishListLines.FindAsync(customerId, id);
            wishlistline.Quantity++;
            _context.Update(wishlistline);
            _context.Update(_context.WishLists.Find(customerId));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "WishlistLines");
        }

        private bool WishListLineExists(int id)
        {
            return _context.WishListLines.Any(e => e.WistListId == id);
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
                return RedirectToAction("Index", "WishlistLines");
            }
            else
            {
                return RedirectToAction("Check", "Login");
            }
        }
    }
}
