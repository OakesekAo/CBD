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
    public class CBDServersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CBDServersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CBDServers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CBDServer.Include(c => c.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CBDServers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CBDServer == null)
            {
                return NotFound();
            }

            var cBDServer = await _context.CBDServer
                .Include(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cBDServer == null)
            {
                return NotFound();
            }

            return View(cBDServer);
        }

        // GET: CBDServers/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CBDServers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,Name,Description,Created,Updated,ImageData,ContentType")] CBDServer cBDServer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cBDServer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", cBDServer.AuthorId);
            return View(cBDServer);
        }

        // GET: CBDServers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CBDServer == null)
            {
                return NotFound();
            }

            var cBDServer = await _context.CBDServer.FindAsync(id);
            if (cBDServer == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", cBDServer.AuthorId);
            return View(cBDServer);
        }

        // POST: CBDServers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,Name,Description,Created,Updated,ImageData,ContentType")] CBDServer cBDServer)
        {
            if (id != cBDServer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cBDServer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CBDServerExists(cBDServer.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", cBDServer.AuthorId);
            return View(cBDServer);
        }

        // GET: CBDServers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CBDServer == null)
            {
                return NotFound();
            }

            var cBDServer = await _context.CBDServer
                .Include(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cBDServer == null)
            {
                return NotFound();
            }

            return View(cBDServer);
        }

        // POST: CBDServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CBDServer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CBDServer'  is null.");
            }
            var cBDServer = await _context.CBDServer.FindAsync(id);
            if (cBDServer != null)
            {
                _context.CBDServer.Remove(cBDServer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CBDServerExists(int id)
        {
          return (_context.CBDServer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
