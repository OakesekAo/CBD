using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CBD.Models
{
    public class CBDUser : IdentityUser
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 2)]
        public string? Name { get; set; }


        [Required]
        [Display(Name = "@Global")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 4)]
        public string? GlobalName { get; set; }


        public virtual ICollection<Server> Servers { get; set; } = new HashSet<Server>();
        public virtual ICollection<Build> Builds { get; set; } = new HashSet<Build>();
    }
}
