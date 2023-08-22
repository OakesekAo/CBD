using CBD.Enums;
using CBD.Models;
using CBD.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CBD.Controllers;
using CBD.Data;

namespace CBD.Services
{
    public class BuildService : IBuildService
    {


        private readonly ApplicationDbContext _context;

        public BuildService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Build> ImportBuildFromJsonAsync(IFormFile jsonFile, string jsonData, string ImportFileName, string userId)
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

            if (string.IsNullOrWhiteSpace(charBuildData.Name))
            {
                charBuildData.Name = ImportFileName;
            }
            charBuildData.CBDUserId = userId;

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
                server.CBDUserId = userId;
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
            //ViewBag.Filename = $"{charBuildData.Class}_{charBuildData.Name.Replace(" ", "_")}";
            return charBuildData;
        }


        //Custom json mapping

        public class PowerSetsConverter : JsonConverter<List<PowerSets>>
        {
            public override List<PowerSets> ReadJson(JsonReader reader, Type objectType, List<PowerSets> existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var token = JToken.Load(reader);
                var powerSets = token.Select(t => new PowerSets
                {
                    Name = t.Value<string>(),
                    NameDisplay = StripAndFormatName(t.Value<string>()),
                    Type = DeterminePowerSetType(t.Path)
                }).ToList();

                return powerSets;
            }

            public override void WriteJson(JsonWriter writer, List<PowerSets>? value, JsonSerializer serializer)
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
