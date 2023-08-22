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

namespace CBD.Controllers
{
    public class BuildsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<CBDUser> _userManager;
        private readonly IBuildService _buildService;

        public BuildsController(ILogger<HomeController> logger, ApplicationDbContext context, IImageService imageService, UserManager<CBDUser> userManager, IBuildService buildService)
        {
            _logger = logger;
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _buildService = buildService;
        }

        // GET: Builds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Build?
                .Include(b => b.CBDUser)
                .Include(b => b.Server)
                .Include(b => b.PowerSets);
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
                .Include(b => b.BuiltWith)         // Eager load BuiltWith
                .Include(b => b.CBDUser)           // Eager load CBDUser
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

            return View(build);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ImportJSON()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BuildImport(IFormFile jsonFile, string jsonData)
        {

            CBDUser cbdUser = await _userManager.GetUserAsync(User);

            string tempFilename = Path.GetFileNameWithoutExtension(jsonFile.FileName);
            //string tempFilename = jsonFile.FileName; // Get the original filename
            // Call the BuildService method to perform the logic.
            var charBuildData = await _buildService.ImportBuildFromJsonAsync(jsonFile, jsonData, tempFilename, cbdUser.Id);

            // Step 4: Pass the modified data and filename to the view
            ViewBag.Filename = tempFilename;
            return View(charBuildData);
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
            return View(build);
        }

        // POST: Builds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReadyStatus,Name,Comment")] Build build)
        {
            if (id != build.Id)
            {
                return NotFound();
            }
            try
            {
                var newBuild = await _context.Build
                                                .Include(b => b.BuiltWith)         // Eager load BuiltWith
                                                .Include(b => b.CBDUser)           // Eager load CBDUser
                                                .Include(b => b.PowerEntries)      // Eager load PowerEntries
                                                    .ThenInclude(pe => pe.SlotEntries) // Eager load SlotEntries within PowerEntries
                                                .Include(b => b.PowerSets)         // Eager load PowerSets
                                                .FirstOrDefaultAsync(b => b.Id == build.Id);

                newBuild.Updated = DateTime.UtcNow;
                newBuild.Name = build.Name;
                newBuild.Comment = build.Comment;
                newBuild.ReadyStatus = build.ReadyStatus;

                _context.Update(newBuild);
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

        public class PowerSetsConverter : Newtonsoft.Json.JsonConverter<PowerSets[]>
        {
            public override PowerSets[] ReadJson(JsonReader reader, Type objectType, PowerSets[] existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var token = JToken.Load(reader);
                var powerSets = token.Select(t => new PowerSets
                {
                    Name = t.Value<string>(),
                    NameDisplay = StripAndFormatName(t.Value<string>()),
                    Type = DeterminePowerSetType(t.Path)
                }).ToArray();

                return powerSets;
            }

            public override void WriteJson(JsonWriter writer, PowerSets[] value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            private string StripAndFormatName(string rawName)
            {
                return rawName.Substring(rawName.IndexOf('.') + 1).Replace('_', ' ');
            }

            private PowerSetType DeterminePowerSetType(string path)
            {
                if (path.EndsWith("[0]"))
                {
                    return PowerSetType.Primary;
                }
                else if (path.EndsWith("[1]"))
                {
                    return PowerSetType.Secondary;
                }
                else if (path.EndsWith("[3]") || path.EndsWith("[4]") || path.EndsWith("[5]") || path.EndsWith("[6]"))
                {
                    return PowerSetType.Pool;
                }
                else
                {
                    return PowerSetType.Epic;
                }
            }
        }
    }
}
