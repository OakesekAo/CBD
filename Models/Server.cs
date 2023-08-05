using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBD.Models
{
    public class Server
    {
        public int Id { get; set; } 
        public string AuthorId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Name { get; set; } // Name of the community server

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Description { get; set; } //Description for the community server


        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } //may not use
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } //may not use


        [Display(Name = "Server Image")]
        public byte[] ImageData { get; set; } //image banner for the server
        [Display(Name = "Image Type")]
        public string ContentType { get; set; } //what is the file format
        [NotMapped]
        public IFormFile Image { get; set; }


        //Navigation Properties
        public virtual CBDUser Author { get; set; }
        public virtual ICollection<Build> Builds { get; set; } = new HashSet<Build>();

    }
}
