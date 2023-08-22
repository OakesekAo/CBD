using CBD.Models;

namespace CBD.Services.Interfaces
{
    public interface IBuildService
    {
        public Task<Build> ImportBuildFromJsonAsync(IFormFile jsonFile, string jsonData, string ImportFileName, string userId);

    }
}
