using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CBD.Data;
using CBD.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;
using CBD.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CBD.Controllers
{
    public class ServersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<CBDUser> _userManager;

        public ServersController(ApplicationDbContext context, IImageService imageService, UserManager<CBDUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: Servers
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Server.Include(s => s.CBDUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Servers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server
                .Include(s => s.CBDUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // GET: Servers/Create
        public IActionResult Create()
        {
            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Servers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Server server)
        {

            CBDUser cbdUser = await _userManager.GetUserAsync(User);
            server.CBDUser = cbdUser;
            server.CBDUserId = cbdUser.Id;


           // if (ModelState.IsValid)
           // {
                server.Created = DateTime.UtcNow;
                server.ImageData = await _imageService.ConvertFileToByteArrayAsync(server.Image);
                //server.ContentType = _imageService.ConetentType(server.Image);
                _context.Add(server);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           // }
            /*if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        // Log or debug the error message
                        var errorMessage = error.ErrorMessage;
                    }
                }
            }*/

            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", server.CBDUserId);
            return View(server);
        }

        // GET: Servers/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }
            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", server.CBDUserId);
            return View(server);
        }

        // POST: Servers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Server server, IFormFile Image)
        {
            if (id != server.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    var newServer = await _context.Server.FindAsync(server.Id);
                    newServer.Updated = DateTime.UtcNow;
                    if (newServer.Name != server.Name)
                    {
                        newServer.Name = server.Name;
                    }

                    if (newServer.Description != server.Description)
                    {
                        newServer.Description = server.Description;
                    }

                    if (Image is not null)
                    {
                        newServer.ImageData = await _imageService.ConvertFileToByteArrayAsync(Image);
                    }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerExists(server.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", server.CBDUserId);
            return View(server);
        }

        // GET: Servers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server
                .Include(s => s.CBDUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // POST: Servers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Server == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Server'  is null.");
            }
            var server = await _context.Server.FindAsync(id);
            if (server != null)
            {
                _context.Server.Remove(server);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServerExists(int id)
        {
          return (_context.Server?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
