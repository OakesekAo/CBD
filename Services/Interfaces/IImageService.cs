namespace CBD.Services.Interfaces
{
    public interface IImageService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
        public Task<byte[]> ConvertFileToByteArrayAsync(string fileName);
        public string ConveryByteArrayToFile(byte[] fileData, string extensione);
        public Task<string> EnhancementImageLookupAsync(string enhacenmentName);
    }
}
