using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly BOOKSTOREContext _context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public LoginController(BOOKSTOREContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // GET: Logins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Logins.ToListAsync());
        }

        // GET: Logins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins
                .FirstOrDefaultAsync(m => m.Email == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // GET: Login/Create
        public IActionResult Create()
        {   
            return View();
        }

        [HttpGet][HttpPost]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }

        }



        // POST: Login/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Email,Password,Address,Phone")] Ortak ortak)
        {

            

            Customer customer = new Customer
            {
                CustomerId = ortak.CustomerId,
                CustomerName = ortak.CustomerName,
                Email = ortak.Email,
                Address = ortak.Address,
                Phone = ortak.Phone
            };

            if (ModelState.IsValid)
            {
                //_context.Add(login);
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }

            Chart chart = new Chart
            {
                CustomerId = customer.CustomerId,
                Customer = customer
            };

            WishList wishList = new WishList
            {
                CustomerId = customer.CustomerId,
                Customer = customer
            };

            if (ModelState.IsValid)
            {
                _context.Add(wishList);
                _context.Add(chart);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }

            var user = new AppUser { UserName = customer.Email};
            var result = await userManager.CreateAsync(user, ortak.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Check","Login");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            /*
            Login login = new Login
            {
                Email = ortak.Email,
                Password = ortak.Password,
                Type = false
            };*/







            return View(ortak);
        }

        // GET: Logins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            return View(login);
        }

        // POST: Logins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email,Password,Type")] Login login)
        {
            if (id != login.Email)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(login);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginExists(login.Email))
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
            return View(login);
        }

        // GET: Logins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var login = await _context.Logins
                .FirstOrDefaultAsync(m => m.Email == id);
            if (login == null)
            {
                return NotFound();
            }

            return View(login);
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var login = await _context.Logins.FindAsync(id);
            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginExists(string id)
        {
            return _context.Logins.Any(e => e.Email == id);
        }

        public IActionResult First()
        {
            return View();
        }

        public IActionResult Check()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Check([Bind("Email, Password, Type")] Login sigin, string ReturnUrl)
        {

            //var user = new AppUser {UserName = sigin.Email, Email = sigin.Email};
            var result = await signInManager.PasswordSignInAsync(sigin.Email, sigin.Password, true, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid User");
            return View(sigin);

            //return View();
            /*Login login;
            if ((login = _context.Logins.Find(sigin.Email)) != null && login.Password.Equals(sigin.Password))
            {

                return RedirectToAction("Index", "Home");
   

            }
            else
            {
                return RedirectToAction(nameof(Check));


            }*/
        }

        public IActionResult Welcome()
        {
            ViewBag.message = User.Identity.Name;
            return View();
        }
        public void SetCookie(string key, string value)
        {
            HttpContext.Response.Cookies.Append(key, value);
        }

        public string GetCookie(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            return value;
        }

        [HttpPost][HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
