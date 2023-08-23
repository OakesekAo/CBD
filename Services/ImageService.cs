using CBD.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace CBD.Services
{
    public class ImageService : IImageService
    {

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        private readonly string defaultImage = "iamges/Default.png";
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                return byteFile;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(string fileName)
        {
            try
            {
                //overload -  return file path
                var file = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{fileName}";
                return await File.ReadAllBytesAsync(file);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string ConveryByteArrayToFile(byte[] fileData, string extension)
        {
            if (fileData is null) return defaultImage;

            try
            {
                string ImageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:image/{extension};base64,{ImageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> EnhancementImageLookupAsync(string enhancementName)
        {
            string jsonFilePath = null;
            if (string.IsNullOrWhiteSpace(enhancementName))
            {
                // Set a default icon and return it
                return "/images/unknown.png";
            }
            else
            {
                // Generate the JSON file path
                jsonFilePath = "wwwroot/json/homecoming/boost_sets/" + enhancementName.ToLower() + ".json";

                // Rest of your code for processing the JSON file
                // ...
            }

            string json = null; // Declare the variable outside the try block
            string icon = null; // Declare the variable outside the try block

            // Load JSON data from the file
            try
            {
                json = File.ReadAllText(jsonFilePath);
                // Now you have the JSON content in the 'json' variable and can deserialize it.
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                // Return a default icon URL or handle the case when JSON data is missing or invalid
                ///This needs to handle reg IOs and HOs
                return "/images/unknown.png";
            }

            // Parse the JSON using JObject from Newtonsoft.Json
            JObject jsonData = JObject.Parse(json);

            JArray conversionGroups = jsonData["conversion_groups"] as JArray;

                    string rarity = conversionGroups[0].ToString();

            switch (rarity)
            {
                case "Rarity: Archetype":
                case "Rarity: Superior Archetype":
                    icon = $"AO_{jsonData["display_name"].ToString().Replace(" ", "_")}.png";
                    break;
                case "Rarity: Summer Blockbuster Double Feature":
                    icon = $"SBB_{jsonData["display_name"].ToString().Replace(" ", "_")}.png";
                    break;
                case "Rarity: Winter Pack Series":
                case "Rarity: Superior Winter Pack Series":
                    icon = $"WO_{jsonData["display_name"].ToString().Replace(" ", "_")}.png";
                    break;
                default:
                    icon = $"IO_{jsonData["display_name"].ToString().Replace(" ", "_")}.png";
                    break;
            }
                // Construct the icon URL
                string iconUrl = $"/images/icon/boosts/sets/{icon}";
                return iconUrl;

            

            // Return a default icon URL or handle the case when JSON data is missing or invalid
            //return "/images/unknown.png";
        }

    }


}
