using HnMicro.Core.Attributes;

namespace Lottery.Core.Enums
{
    public enum SessionState
    {
        [EnumDescription("All_SessionState")]
        All = 0,

        [EnumDescription("Offline_SessionState")]
        Offline = 1,

        [EnumDescription("Online_SessionState")]
        Online = 2
    }
}
