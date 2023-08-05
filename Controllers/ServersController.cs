using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CBD.Data;
using CBD.Models;

namespace CBD.Controllers
{
    public class ServersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Server.Include(s => s.Author);
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
                .Include(s => s.Author)
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Servers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,Name,Description,Created,Updated,ImageData,ContentType")] Server server)
        {
            if (ModelState.IsValid)
            {
                _context.Add(server);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", server.AuthorId);
            return View(server);
        }

        // GET: Servers/Edit/5
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", server.AuthorId);
            return View(server);
        }

        // POST: Servers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,Name,Description,Created,Updated,ImageData,ContentType")] Server server)
        {
            if (id != server.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(server);
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
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", server.AuthorId);
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
                .Include(s => s.Author)
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
