using CBD.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace CBD.Models
{
    public class CharBuildJsonRaw
    {





        public class Build
        {
            
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
        }

        public class PowerSets
        {
            public string Name { get; set; }
            public string NameDisplay { get; set; }
            public PowerSetType Type { get; set; } // Define a PowerSetType enum or similar
        }

        public class Builtwith
        {
            public string App { get; set; }
            public string Version { get; set; }
            public string Database { get; set; }
            public string DatabaseVersion { get; set; }
        }

        public class Powerentry
        {
            public string PowerName { get; set; }
            public string PowerNameDisplay { get; set; }
            public PowerSetType PowerSetType { get; set; }
            public int Level { get; set; }
            public bool StatInclude { get; set; }
            public bool ProcInclude { get; set; }
            public int VariableValue { get; set; }
            public int InherentSlotsUsed { get; set; }
            public object[] SubPowerEntries { get; set; }
            public Slotentry[] SlotEntries { get; set; }
        }

        public class Slotentry
        {
            public int Level { get; set; }
            public bool IsInherent { get; set; }

            public Enhancement Enhancement { get; set; }
            public object FlippedEnhancement { get; set; }
        }

        public class Enhancement
        {
            [JsonProperty("Enhancement")]
            public string EnhancementName { get; set; } 
            public string Grade { get; set; }
            public int IoLevel { get; set; }
            public string RelativeLevel { get; set; }
            public bool Obtained { get; set; }
        }

    }
}
