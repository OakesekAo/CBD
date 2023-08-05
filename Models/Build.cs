using CBD.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CBD.Models
{
    public class Build
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string AuthorId { get; set; }

        //this is where all the build properties go
        //this is where all the build properties go
        //this is where all the build properties go
        //this is where all the build properties go
        //this is where all the build properties go
        //this is where all the build properties go




        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } //may not use
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } //may not use

        public ReadyStatus ReadyStatus { get; set; } //private/public view flag


        [Display(Name = "Build Image")]
        public byte[] ImageData { get; set; } //image banner for the server
        [Display(Name = "Image Type")]
        public string ContentType { get; set; } //what is the file format
        [NotMapped]
        public IFormFile Image { get; set; }


        //Navigation Properties
        public virtual Server Server { get; set; }
        public virtual CBDUser Author { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
