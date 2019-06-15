using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FotoApplicatie.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FotoApplicatie.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataAccess _context;
        private readonly IHostingEnvironment _env;

        public UsersController(DataAccess context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }
        // GET: Producten
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, int? id)
        {
        
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var users = from s in _context.User
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Name.Contains(searchString));
                //(s => s.Category.Name.Contains(searchString) ||s =>s.Price.Contains(searchstring)) //kan ook is dan extra filter
            }


            switch (sortOrder)
            {
                case "Order by Headcategory":
                    users = users.OrderByDescending(p => p.Email);
                    break;
                case "Order by id":
                    users = users.OrderByDescending(p => p.UserId);
                    break;

                    //case "Price":
                    //    products = products.OrderBy(s => s.Price);
                    //    break;
                    //case "price_desc":
                    //    products = products.OrderByDescending(s => s.Price);
                    //    break;
                    //default:
                    //    products = products.OrderBy(s => s.Description);
                    //    break;
            }
            int pageSize = 5;          
            return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize));

            //return View(await products.AsNoTracking().ToListAsync());

            //var warmeBakkerContext = _context.Products.Include(p => p.Category);
            //return View(await warmeBakkerContext.ToListAsync());
        }
        
        
        //// GET: Users
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.User.ToListAsync());
        //}

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,Email,Company,Introduction")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var user = await _context.User.FindAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}
            //return View(user);
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            string webRoot = _env.WebRootPath;
            string img_p = "";
            string fileName = "";
            if (System.IO.Directory.Exists(webRoot + "/UserFiles/" + user.UserId.ToString() + "/Image/"))
            {
                string[] strfiles = Directory.GetFiles(webRoot + "/UserFiles/" + user.UserId.ToString() + "/Image/", "*.*");

                if (strfiles.Length > 0)
                {

                    for (int i = 0; i < strfiles.Length; i++)
                    {
                        fileName = Path.GetFileName(strfiles[i]);

                        string _CurrentFile = strfiles[i].ToString();
                        if (System.IO.File.Exists(_CurrentFile))
                        {
                            string tempFileURL = "/UserFiles/" + user.UserId.ToString() + "/Image/" + Path.GetFileName(_CurrentFile);
                            img_p = tempFileURL;
                        }

                    }

                }
            }

            if (!string.IsNullOrEmpty(img_p))
            {
                ViewBag.ImgPath = Convert.ToString(img_p);
                ViewBag.FileName = Convert.ToString(fileName);
            }
            else
                ViewBag.ImgPath = "/Images/default.jpg";

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,Email,Company,Introduction")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormCollection form)
        {
            if (form.Files == null || form.Files[0].Length == 0)
                return RedirectToAction("Edit", new { id = Convert.ToString(form["UserId"]) });

            var webRoot = _env.WebRootPath;
            string userId = Convert.ToString(form["UserId"]);

            if (!System.IO.Directory.Exists(webRoot + "/UserFiles/"))
            {
                System.IO.Directory.CreateDirectory(webRoot + "/UserFiles/");
            }
            if (!System.IO.Directory.Exists(webRoot + "/UserFiles/" + userId + "/Image/"))
            {
                System.IO.Directory.CreateDirectory(webRoot + "/UserFiles/" + userId + "/Image/");
            }

            //Delete existing files first and then add new file  
           // DeleteFile(userId);

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot" + "/UserFiles/" + userId + "/Image/",
                        form.Files[0].FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await form.Files[0].CopyToAsync(stream);
            }
            
            return RedirectToAction("Edit", new { id = Convert.ToString(form["UserId"]) });
        }

        public async Task<IActionResult> Download(string img, string userId)
        {
            string filename = img;
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot" + "/UserFiles/" + userId + "/Image/", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"}
            };
        }


    }
}
