using CBD.Models;
using System.Collections.Generic;

namespace CBD.Models.ViewModels
{
    public class BuildDetailViewModel
    {
        public Build Build { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
