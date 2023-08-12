namespace CBD.Services.Interfaces
{
    public interface IBuildService
    {
        public Task ImportBuildFromJsonAsync(IFormFile jsonFile, string jsonData);

    }
}
