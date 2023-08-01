using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CBD.Models
{
    public class CBDUser : IdentityUser
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 2)]
        public string Name { get; set; }


        [Display(Name = "@Global")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 4)]
        public string GlobalName { get; set; }



    }
}
