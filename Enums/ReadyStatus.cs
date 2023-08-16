using System.ComponentModel;

namespace CBD.Enums
{
    public enum ReadyStatus
    {
        Incomplete,
        [Description("Private Ready")]
        PrivateReady,
        [Description("Private Link Ready")]
        PrivateLinkReady,
        [Description("Public Ready")]
        PublicReady
    }
}
