using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CBD.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int BuildId { get; set; }
        public string CBDUserId { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Text { get; set; }

        //Navigation Properties
        public virtual Build Build { get; set; }
        public virtual CBDUser CBDUser { get; set; }
    }
}
