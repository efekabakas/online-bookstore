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
    public class ProductsController : Controller
    {
        private readonly BOOKSTOREContext _context;

        
        public ProductsController(BOOKSTOREContext context)
        {
            _context = context;
        }
        
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
              
                return NotFound();
            }



            return View(product);
        }



        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Pages,ProductId,Isbn,Date,ProductType,Stock,Price,Language,Genre,Rating,PublisherId,AuthorId,SupplierId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", product.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", product.PublisherId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", product.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", product.PublisherId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Pages,ProductId,Isbn,Date,ProductType,Stock,Price,Language,Genre,Rating,PublisherId,AuthorId,SupplierId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", product.AuthorId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", product.PublisherId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Publisher)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        [Authorize]
        public async Task<IActionResult> AddToChart(int id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();
            var product = _context.Products.Find(id);
            var chart = _context.Charts.Find(customerId);
            var line = _context.ChartLines.Find(customerId, id);

            if (line != null)
            {
                line.Quantity++;
                chart.Cost += (int)product.Price;
                _context.Update(line);
            }
            else
            {
                line = new ChartLine()
                {
                    ProductId = id,
                    ChartId = customerId,
                    Product = product,
                    Chart = chart,
                    Quantity = 1
                    
            };
                chart.Cost += (int)product.Price;
                _context.Add(line);
            }
            await _context.SaveChangesAsync();





            return RedirectToAction("Index", "Products");

        }
        [Authorize] 
        public async Task<IActionResult> AddToWishlist(int id)
        {
            string email = User.Identity.Name;

            var customers = _context.Customers
                .Where(c => c.Email == email)
                .Select(id => id.CustomerId)
                .ToList();

            int customerId = customers.First();

            var product = _context.Products.Find(id);
            var wishlist = _context.WishLists.Find(customerId);
            var line = _context.WishListLines.Find(customerId, id);

            if (line != null)
            {
                line.Quantity++;
                _context.Update(line);
            }
            else
            {
                line = new WishListLine()
                {
                    ProductId = id,
                    WistListId = customerId,
                    Product = product,
                    WistList = wishlist,

                    Quantity = 1

                };

                _context.Add(line);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "WishListLines");

        }
        [HttpPost]
        public async Task<IActionResult> SearchAsync(string SearchString)
        {
            var products = from a in _context.Products
                         select a;

            if (!String.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.Title.Contains(SearchString));
            }

            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> FilterBy(String parameter, int? num)
        {
            var products = from a in _context.Products
                           select a;

            if (parameter.Equals("Turkish") || parameter.Equals("English"))
            {
                products = products.Where(s => s.Language.Equals(parameter));
            }
            else if (parameter.Equals("Classic") || parameter.Equals("Fiction") || parameter.Equals("Science Fiction") || parameter.Equals("Drama"))
            {
                products = products.Where(s => s.Genre.Equals(parameter));

            }
            else if (parameter.Equals("Under ₺10") || parameter.Equals("Under ₺20") || parameter.Equals("Under ₺50") || parameter.Equals("Under ₺100"))
            {
                products = products.Where(s => s.Price < num);
            }
            else if (parameter.Equals("Under 500 Pages") || parameter.Equals("Under 1000 Pages"))
            {
                products = products.Where(s => s.Pages < num);
            }
            else if (parameter.Equals("3 stars and above") || parameter.Equals("4 stars and above"))
            {
                products = products.Where(s => s.Rating < num);
            }

            return View(await products.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> BooksDetails()
        {
            var books = from b in _context.Products
                        select b;


            books = books.Where(s => s.ProductType.Equals("Book"));

            return View(await books.ToListAsync());
        }




        [HttpGet]
        public async Task<IActionResult> MagazinesDetails()
        {
            var magazines = from b in _context.Products
                            select b;


            magazines = magazines.Where(s => s.ProductType.Equals("Magazine"));

            return View(await magazines.ToListAsync());
        }

        public async Task<IActionResult> Rating(int id, [Bind("Rating")] Product product)
        {
            var productDB = _context.Products.Find(id);
            var previousRating = productDB.Rating;
            var previousCount = productDB.Count;


            productDB.Rating = Convert.ToByte((previousCount * previousRating + product.Rating) / (previousCount + 1));
            productDB.Count++;

            _context.Update(productDB);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
            //return RedirectToAction("Index","Products");

        }


    }
}
