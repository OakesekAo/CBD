using CBD.Enums;
using CBD.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using static CBD.Models.Build;

namespace CBD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ImportJSON()
        {
            return View();
        }

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
                    else if(charBuildData.PowerSets[0].Name == rawPowerNamePrefix)
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
                    else if (charBuildData.PowerSets[4].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (charBuildData.PowerSets[5].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (charBuildData.PowerSets[6].Name == rawPowerNamePrefix)
                    {
                        powerSetType = PowerSetType.Pool;
                    }
                    else if (charBuildData.PowerSets[7].Name == rawPowerNamePrefix)
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
            }
            var inherentPowerSetGroups = charBuildData.PowerSets
    .Where(ps => ps.Type == CBD.Enums.PowerSetType.Inherent)
    .GroupBy(ps => ps.Name.Split('.')[1])
    .Select(group => new
    {
        Name = group.Key,
        PowerEntries = charBuildData.PowerEntries
            .Where(pe => pe.PowerSetType == CBD.Enums.PowerSetType.Inherent && pe.PowerName.StartsWith(group.Key))
    });

            // Step 4: Pass the modified data and filename to the view
            ViewBag.InherentPowerSetGroups = inherentPowerSetGroups;
            ViewBag.Filename = $"{charBuildData.Class}_{charBuildData.Name.Replace(" ", "_")}";
            return View(charBuildData);
        }

        public IActionResult DownloadRawJson(string rawData, string filename)
        {
            // Set the content type and return the raw JSON data as a file
            return File(Encoding.UTF8.GetBytes(rawData), "application/json", $"{filename}.json");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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