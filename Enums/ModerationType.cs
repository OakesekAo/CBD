using System.ComponentModel;

namespace CBD.Enums
{
    public enum ModerationType
    {
        [Description("Political propaganda")]
        Political,
        [Description("Offensive language")]
        Language,
        [Description("Drug refernces")]
        Drugs,
        [Description("Threatening speech")]
        Threatening,
        [Description("Sexual content")]
        Sexual,
        [Description("Hate speech")]
        Hate,
        [Description("Targeted shaming")]
        Shaming
    }
}
