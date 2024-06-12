using HnMicro.Core.Attributes;

namespace Lottery.Core.Enums
{
    public enum UserState
    {
        [EnumDescription("All_UserState")]
        All = 0,

        [EnumDescription("Open_UserState")]
        Open = 1,

        [EnumDescription("Suspended_UserState")]
        Suspended = 2,

        [EnumDescription("Closed_UserState")]
        Closed = 3
    }
}
