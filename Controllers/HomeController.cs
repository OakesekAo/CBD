﻿using CBD.Enums;
using CBD.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static CBD.Models.CharBuildJsonRaw;

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
            CharBuildJsonRaw.Build charBuildData = JsonConvert.DeserializeObject<CharBuildJsonRaw.Build>(jsonData, settings);



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
                // Get the prefix before the second "."
                //string[] parts = powerEntry.PowerName.Split('.');
                //string rawPowerNamePrefix = string.Join(".", parts[0], parts[1]);
                string rawPowerNamePrefix = string.Join(".", powerEntry.PowerName.Split('.').Take(2));


                // Strip the prefix and replace '_' with ' ' to create PowerNameDisplay
                //string rawPowerNameMid = powerEntry.PowerName.Split('.')[1];
                //string rawPowerNameDisplay = rawPowerNameMid.Replace("_", " ");
                string rawPowerNameDisplay = powerEntry.PowerName.Split('.')[2].Replace("_", " ");


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

            // Step 4: Pass the modified data to the view
            return View(charBuildData);
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