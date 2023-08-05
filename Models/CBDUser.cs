using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CBD.Models
{
    public class CBDUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 2)]
        public string LastName { get; set; }


        [Display(Name = "@Global")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max of {1} characters long", MinimumLength = 4)]
        public string GlobalName { get; set; }


        [Display(Name = "Avatar Image")]
        public byte[] ImageData { get; set; } //image banner for the server
        [Display(Name = "Image Type")]
        public string ContentType { get; set; } //what is the file format

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        public virtual ICollection<Server> Servers { get; set; } = new HashSet<Server>();
        public virtual ICollection<Build> Builds { get; set; } = new HashSet<Build>();
    }
}
