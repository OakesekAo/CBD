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
            var applicationDbContext = _context.Build.Include(b => b.CBDUser).Include(b => b.Server);
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
                .Include(b => b.CBDUser)
                .Include(b => b.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        [HttpGet]
        public IActionResult ImportJSON()
        {
            return View();
        }

        //Covert and save build
        [HttpPost]
        public IActionResult BuildImport(IFormFile jsonFile, string jsonData)
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

            //PowerSets naming assigned
            var powerSetsList = new List<PowerSets>();
            for (int i = 0; i < charBuildData.PowerSets.Length; i++)
            {
                string rawName = charBuildData.PowerSets[i].Name; // Access the array element using [i]
                string strippedName = rawName.Substring(rawName.IndexOf('.') + 1).Replace('_', ' ');

                powerSetsList.Add(new PowerSets { Name = rawName, NameDisplay = strippedName, Type = powerSetTypes[i] });
            }

            charBuildData.PowerSets = powerSetsList.ToArray();

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
                    if (charBuildData.PowerSets[0].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Primary;
                    }
                    else if (charBuildData.PowerSets[1].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Secondary;
                    }
                    else if (charBuildData.PowerSets[3].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else
                    {
                        powerSetType = PowerSetType.Epic;
                    }

                    // Assign the values
                    powerEntry.PowerNameDisplay = rawPowerNameDisplay;
                    powerEntry.PowerSetType = powerSetType;
                }
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServerId,CBDUserId,Created,Updated,ReadyStatus,ImageData,ContentType,Class,ClassDisplay,Origin,Alignment,Name,Comment,LastPower,RawJson")] Build build)
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
