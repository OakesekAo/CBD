using System.ComponentModel;

namespace CBD.Enums
{
    public enum PowerSetType
    {
        [Description("Primary Powerset")]
        Primary,
        [Description("Secondary Powerset")]
        Secondary,
        [Description("Empty")]
        Empty,
        [Description("Pool Powerset")]
        Pool,
        [Description("Epic Powerset")]
        Epic,
        [Description("Inherent Powers")]
        Inherent
    }
}
