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
    public class BuildsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuildsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Builds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Build.Include(b => b.Author).Include(b => b.Server);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Builds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Build == null)
            {
                return NotFound();
            }

            var build = await _context.Build
                .Include(b => b.Author)
                .Include(b => b.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // GET: Builds/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description");
            return View();
        }

        // POST: Builds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServerId,AuthorId,Created,Updated,ReadyStatus,ImageData,ContentType")] Build build)
        {
            if (ModelState.IsValid)
            {
                _context.Add(build);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", build.AuthorId);
            ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description", build.ServerId);
            return View(build);
        }

        // GET: Builds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Build == null)
            {
                return NotFound();
            }

            var build = await _context.Build.FindAsync(id);
            if (build == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", build.AuthorId);
            ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description", build.ServerId);
            return View(build);
        }

        // POST: Builds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServerId,AuthorId,Created,Updated,ReadyStatus,ImageData,ContentType")] Build build)
        {
            if (id != build.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(build);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildExists(build.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", build.AuthorId);
            ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description", build.ServerId);
            return View(build);
        }

        // GET: Builds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Build == null)
            {
                return NotFound();
            }

            var build = await _context.Build
                .Include(b => b.Author)
                .Include(b => b.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        // POST: Builds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Build == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Build'  is null.");
            }
            var build = await _context.Build.FindAsync(id);
            if (build != null)
            {
                _context.Build.Remove(build);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildExists(int id)
        {
          return (_context.Build?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
