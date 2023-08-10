using CBD.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace CBD.Models
{

    public class Build
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string CBDUserId { get; set; }

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
        public Builtwith BuiltWith { get; set; }
        public string Class { get; set; }
        public string ClassDisplay { get; set; }
        public string Origin { get; set; }
        public string Alignment { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public PowerSets[] PowerSets { get; set; }
        public int LastPower { get; set; }
        public Powerentry[] PowerEntries { get; set; }
        // hold the raw JSON data
        public string RawJson { get; set; }

        //Navigation Properties
        public virtual Server Server { get; set; }
        public virtual CBDUser CBDUser { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }

    public class PowerSets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameDisplay { get; set; }
        public PowerSetType Type { get; set; } // Define a PowerSetType enum or similar
    }

    public class Builtwith
    {
        public int Id { get; set; }
        public string App { get; set; }
        public string Version { get; set; }
        public string Database { get; set; }
        public string DatabaseVersion { get; set; }
    }

    public class Powerentry
    {
        public int Id { get; set; }
        public string PowerName { get; set; }
        public string PowerNameDisplay { get; set; }
        public PowerSetType PowerSetType { get; set; }
        public int Level { get; set; }
        public bool StatInclude { get; set; }
        public bool ProcInclude { get; set; }
        public int VariableValue { get; set; }
        public int InherentSlotsUsed { get; set; }
        [NotMapped]
        public object[] SubPowerEntries { get; set; }
        public Slotentry[] SlotEntries { get; set; }
    }

    public class Slotentry
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public bool IsInherent { get; set; }

        public Enhancement Enhancement { get; set; }
        [NotMapped]
        public object FlippedEnhancement { get; set; }
    }

    public class Enhancement
    {
        public int Id { get; set; }
        [JsonProperty("Enhancement")]
        public string EnhancementName { get; set; }
        public string Grade { get; set; }
        public int IoLevel { get; set; }
        public string RelativeLevel { get; set; }
        public bool Obtained { get; set; }
    }


}
