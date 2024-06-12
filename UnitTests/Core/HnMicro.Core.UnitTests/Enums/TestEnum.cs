using HnMicro.Core.Attributes;

namespace HnMicro.Core.UnitTests.Enums
{
    public enum TestEnum
    {
        [EnumDescription("Int")]
        Int = 0,

        [EnumDescription("Long")]
        Long = 1
    }
}
