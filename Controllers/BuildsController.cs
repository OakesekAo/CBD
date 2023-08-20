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

namespace CBD.Controllers
{
    public class BuildsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<CBDUser> _userManager;

        public BuildsController(ILogger<HomeController> logger, ApplicationDbContext context, IImageService imageService, UserManager<CBDUser> userManager)
        {
            _logger = logger;
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: Builds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Build
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


            // Check if a file was uploaded and use its content
            if (jsonFile != null && jsonFile.Length > 0)
            {
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    jsonData = reader.ReadToEnd();
                }
            }

            // Step 2: Deserialize JSON data into the CharBuild.Rootobject class
            var settings = new JsonSerializerSettings
            {
                Converters = { new PowerSetsConverter() } // Register a custom converter for PowerSets
            };
            Build charBuildData = JsonConvert.DeserializeObject<Build>(jsonData, settings);

            CBDUser cbdUser = await _userManager.GetUserAsync(User);
            charBuildData.CBDUser = cbdUser;
            charBuildData.CBDUserId = cbdUser.Id;

            // Assign the raw JSON to the property
            charBuildData.RawJson = jsonData;

            // Step 3: Modify properties
            charBuildData.ClassDisplay = charBuildData.Class.Replace("Class_", "");
            // Add similar logic for other properties if needed
            var powerSetTypes = new PowerSetType[]
            {
                    PowerSetType.Primary,
                    PowerSetType.Secondary,
                    PowerSetType.Empty,
                    PowerSetType.Pool,
                    PowerSetType.Pool,
                    PowerSetType.Pool,
                    PowerSetType.Pool,
                    PowerSetType.Epic
            };


            // Convert PowerSets to a list
            List<PowerSets> powerSetsList = charBuildData.PowerSets.ToList();

            for (int i = 0; i < powerSetsList.Count; i++)
            {
                string rawName = powerSetsList[i].Name; // Access the list element using [i]
                string strippedName = rawName.Substring(rawName.IndexOf('.') + 1).Replace('_', ' ');

                powerSetsList[i] = new PowerSets { Name = rawName, NameDisplay = strippedName, Type = powerSetTypes[i] };
            }

            // Convert the List<PowerSets> back to an array
            //charBuildData.PowerSets = powerSetsList.ToArray();




            //PowerSets naming assigned
            //var powerSetsList = new List<PowerSets>();
            // for (int i = 0; i < charBuildData.PowerSets.Length; i++)
            //{
            //     string rawName = charBuildData.PowerSets[i].Name; // Access the array element using [i]
            //     string strippedName = rawName.Substring(rawName.IndexOf('.') + 1).Replace('_', ' ');

            //     powerSetsList.Add(new PowerSets { Name = rawName, NameDisplay = strippedName, Type = powerSetTypes[i] });
            // }

            // List of prefixes corresponding to Inherent power set type
            List<string> inherentPrefixes = new List<string>
                    {
                        "Inherent.Inherent",
                        "Inherent.Fitness"
                        // Add other possible prefixes here
                    };
            List<string> incarnatetPrefixes = new List<string>
                    {
                        "Incarnate.Alpha",
                        "Incarnate.Interface",
                        "Incarnate.Destiny",
                        "Incarnate.Hybrid"
                        // Add other possible prefixes here
                    };

            // Convert PowerEntries to a list
            List<Powerentry> powerEntriesList = charBuildData.PowerEntries.ToList();
            //Power names adjusted and assigned
            foreach (var powerEntry in charBuildData.PowerEntries)
            {
                // Check if the PowerName is not empty or whitespace
                if (!string.IsNullOrWhiteSpace(powerEntry.PowerName))
                {
                    // Get the prefix before the second "."
                    string rawPowerNamePrefix = string.Join(".", powerEntry.PowerName.Split('.').Take(2));
                    // Strip the prefix and replace '_' with ' ' to create PowerNameDisplay
                    string[] parts = powerEntry.PowerName.Split('.');
                    string rawPowerNameDisplay = parts.Length > 2 ? parts[2].Replace("_", " ") : parts[1];
                    // Determine the PowerSetType based on the raw power name
                    PowerSetType powerSetType;
                    if (inherentPrefixes.Contains(rawPowerNamePrefix))
                    {
                        powerSetType = PowerSetType.Inherent;
                    }
                    else if (incarnatetPrefixes.Contains(rawPowerNamePrefix))
                    {
                        powerSetType = PowerSetType.Incarnate;
                    }
                    else if (powerSetsList[0].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Primary;
                    }
                    else if (powerSetsList[1].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Secondary;
                    }
                    else if (powerSetsList[3].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (powerSetsList[4].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (powerSetsList[5].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (powerSetsList[6].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (powerSetsList[7].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Epic;
                    }
                    else
                    {
                        powerSetType = PowerSetType.Temporary;
                    }

                    // Assign the values
                    powerEntry.PowerNameDisplay = rawPowerNameDisplay;
                    powerEntry.PowerSetType = powerSetType;
                }


                // Iterate through SlotEntries
                foreach (var slotEntry in powerEntry.SlotEntries)
                {
                    if (slotEntry.Enhancement != null && slotEntry.Enhancement is Enhancement enhancementData)
                    {
                        // Create or retrieve Enhancement entity
                        Enhancement enhancement = await _context.Enhancement.FirstOrDefaultAsync(e => e.EnhancementName == enhancementData.EnhancementName);
                        if (enhancement == null)
                        {
                            // Create a new Enhancement if not found
                            enhancement = new Enhancement
                            {
                                EnhancementName = enhancementData.EnhancementName,
                                // Set other properties as needed
                            };
                            _context.Enhancement.Add(enhancement);
                            await _context.SaveChangesAsync();
                        }

                        // Create Slotentry and associate it with Enhancement
                        Slotentry newSlotEntry = new Slotentry
                        {
                            // Set Slotentry properties
                            // ...
                            Enhancement = enhancement,
                            EnhancementId = enhancement.Id // Set the foreign key to the Enhancement
                        };

                        // Add the new Slotentry to the context
                        _context.Slotentry.Add(newSlotEntry);
                    }
                }
            }
            // Save changes to the context
            await _context.SaveChangesAsync();
            //save to db

            charBuildData.Created = DateTime.UtcNow;

            // Retrieve or create the Server entity
            Server server = await _context.Server.FirstOrDefaultAsync(s => s.Name == charBuildData.BuiltWith.Database);
            if (server == null)
            {
                // Create a new Server entity if not found
                server = new Server { Name = charBuildData.BuiltWith.Database };
                server.CBDUser = cbdUser;
                server.CBDUserId = cbdUser.Id;
                _context.Server.Add(server);
                await _context.SaveChangesAsync();
            }

            // Set the Server and ServerId properties
            charBuildData.Server = server;
            charBuildData.ServerId = server.Id;

            //charBuildData.ImageData = await _imageService.ConvertFileToByteArrayAsync(charBuildData.Image);
            //server.ContentType = _imageService.ConetentType(server.Image);
            _context.Add(charBuildData);
            await _context.SaveChangesAsync();


            // Step 4: Pass the modified data and filename to the view
            ViewBag.Filename = $"{charBuildData.Class}_{charBuildData.Name.Replace(" ", "_")}";
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
