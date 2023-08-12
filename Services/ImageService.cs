using CBD.Services.Interfaces;

namespace CBD.Services
{
    public class ImageService : IImageService
    {

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        private readonly string defaultImage = "Iamge/Default.png";
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
    }
}
