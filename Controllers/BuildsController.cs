using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CBD.Data;
using CBD.Models;
using CBD.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CBD.Services.Interfaces;
using CBD.Services;
using X.PagedList;
using CBD.Models.ViewModels;
using System.Drawing.Printing;
using CBD.Data.Migrations;

namespace CBD.Controllers
{
    public class BuildsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<CBDUser> _userManager;
        private readonly IBuildService _buildService;
        private readonly SearchService _searchService;

        public BuildsController(ILogger<HomeController> logger, ApplicationDbContext context, IImageService imageService, UserManager<CBDUser> userManager, IBuildService buildService, SearchService searchService)
        {
            _logger = logger;
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _buildService = buildService;
            _searchService = searchService;
        }

        // GET: Builds
        public async Task<IActionResult> Index(int? page, string tag)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;

            IQueryable<Build> buildsQuery = _context.Build;

            if (!string.IsNullOrWhiteSpace(tag))
            {
                // Filter builds by the selected tag
                buildsQuery = buildsQuery.Where(b => b.Tags.Any(t => t.Text == tag));
            }

            // You can add additional filters here if needed
            // For example, filtering by ReadyStatus:
            // buildsQuery = buildsQuery.Where(b => b.ReadyStatus == ReadyStatus.PublicReady);

            var builds = await buildsQuery
                .Include(b => b.CBDUser)
                .Include(b => b.Server)
                .Include(b => b.PowerSets)
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);

            return View(builds);
        }

        public async Task<IActionResult> TagIndex(int? page, string tag)
        {
            ViewData["Tag"] = tag;
            var pageNumber = page ?? 1;
            var pageSize = 5;

            // Get all builds that have the specified tag
            var buildsQuery = _context.Build
                .Where(b => b.Tags.Any(t => t.Text.ToLower() == tag))
                .Include(b => b.CBDUser)
                .Include(b => b.Server)
                .Include(b => b.PowerSets)
                .OrderByDescending(p => p.Created);

            var builds = await buildsQuery.ToPagedListAsync(pageNumber, pageSize);

            return View("Index", builds); // Reuse the Index view
        }


        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 12;

            var builds = _searchService.Search(searchTerm);

            return View(await builds.ToPagedListAsync(pageNumber, pageSize));

        }



        // GET: Builds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Build == null)
            {
                return NotFound();
            }

            var build = await _context.Build
                .Include(b => b.BuiltWith)         // Eager load BuiltWith
                .Include(b => b.CBDUser)           // Eager load CBDUser
                .Include(p => p.Tags)
                .Include(b => b.Server)
                .Include(b => b.PowerEntries)      // Eager load PowerEntries
                    .ThenInclude(b => b.SlotEntries) // Eager load SlotEntries within PowerEntries
                        .ThenInclude(pe => pe.Enhancement) // Eager load SlotEntries within PowerEntries
                .Include(b => b.PowerSets)         // Eager load PowerSets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            var dataVM = new BuildDetailViewModel()
            {
                Build = build,
                Tags = _context.Tags
                        .Select(t => t.Text.ToLower())
                        .Distinct().ToList()

            };
            return View(dataVM);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ImportJSON()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BuildImport([Bind("ReadyStatus")] IFormFile jsonFile, string jsonData, List<string> tagValues)
        {

            CBDUser cbdUser = await _userManager.GetUserAsync(User);
            //var cbdUserId = cbdUser.Id;
            string tempFilename = Path.GetFileNameWithoutExtension(jsonFile.FileName);
            //string tempFilename = jsonFile.FileName; // Get the original filename
            // Call the BuildService method to perform the logic.
            var charBuildData = await _buildService.ImportBuildFromJsonAsync(jsonFile, jsonData, tempFilename, cbdUser.Id);

            //Loop through the incoming list of tags
            foreach (var tagText in tagValues)
            {
                _context.Add(new Tag()
                {
                    BuildId = charBuildData.Id,
                    CBDUserId = cbdUser.Id,
                    Text = tagText
                });
            }

            await _context.SaveChangesAsync();

            // Step 4: Pass the modified data and filename to the view
            ViewBag.Filename = tempFilename;
            return RedirectToAction("Details", "Builds", new { id = charBuildData.Id });
        }


        // GET: Builds/Create

        public IActionResult Create()
        {
            //ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description");
            return View();
        }

        // POST: Builds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServerId,Name,Description,ReadyStatus,Image")] IFormFile jsonFile, string jsonData, Build build)
        {

            if (ModelState.IsValid)
            {
                _context.Add(build);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", build.CBDUserId);
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
            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", build.CBDUserId);
            ViewData["ServerId"] = new SelectList(_context.Server, "Id", "Description", build.ServerId);
            ViewData["TagValues"] = string.Join(",", build.Tags.Select(t => t.Text));

            return View(build);
        }

        // POST: Builds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReadyStatus,Name,Comment")] Build build, List<string> tagValues)
        {

            CBDUser cbdUser = await _userManager.GetUserAsync(User);

            if (id != build.Id)
            {
                return NotFound();
            }
            try
            {
                var newBuild = await _context.Build
                                                .Include(b => b.BuiltWith)         //  load BuiltWith
                                                .Include(b => b.CBDUser)           //  load CBDUser
                                                .Include(b => b.PowerEntries)      //  load PowerEntries
                                                    .ThenInclude(pe => pe.SlotEntries) //  load SlotEntries within PowerEntries
                                                .Include(b => b.PowerSets)         //  load PowerSets
                                                .FirstOrDefaultAsync(b => b.Id == build.Id);

                newBuild.Updated = DateTime.UtcNow;
                newBuild.Name = build.Name;
                newBuild.Comment = build.Comment;
                newBuild.ReadyStatus = build.ReadyStatus;

                _context.Update(newBuild);
                await _context.SaveChangesAsync();

                //remove all tags previously associated with this Post
                _context.Tags.RemoveRange(newBuild.Tags);

                //add in the new tags from the edit form
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        BuildId = id,
                        CBDUserId = cbdUser.Id,
                        Text = tagText
                    });
                }


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




            ViewData["CBDUserId"] = new SelectList(_context.Users, "Id", "Id", build.CBDUserId);
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
                .Include(b => b.CBDUser)
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
            var build = await _context.Build
                                    .Include(b => b.PowerEntries)
                                        .ThenInclude(s => s.SlotEntries)
                                    // Include other related entities if needed
                                    .FirstOrDefaultAsync(b => b.Id == id);
            if (build != null)
            {
                // Remove related Slotentry records first
                _context.Slotentry.RemoveRange(build.PowerEntries.SelectMany(pe => pe.SlotEntries));
                // Remove related Powerentry records
                _context.Powerentry.RemoveRange(build.PowerEntries);
                // Remove the Build record
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
