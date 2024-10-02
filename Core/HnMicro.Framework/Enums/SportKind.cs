using HnMicro.Core.Attributes;

namespace HnMicro.Framework.Enums
{
    public enum SportKind
    {
        [EnumDescription("Cock Fight")]
        CockFight = 600,

        [EnumDescription("Casino")]
        Casino = 700,

        [EnumDescription("Lottery")]
        Lottery = 840,

        [EnumDescription("Turbo Lottery (3m)")]
        Turbo3mLottery = 841,

        [EnumDescription("Turbo Lottery (5m)")]
        Turbo5mLottery = 842
    }
}
