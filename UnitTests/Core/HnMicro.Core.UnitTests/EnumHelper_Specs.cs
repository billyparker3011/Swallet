using HnMicro.Core.Helpers;
using HnMicro.Core.UnitTests.Enums;
using HnMicro.Framework.UnitTests.Models;

namespace HnMicro.Core.UnitTests
{
    [TestClass]
    public class EnumHelper_Specs : BaseSpecs
    {
        [TestMethod("Convert Enum To Int")]
        public void Convert_Enum_To_Int()
        {
            var iEnum = TestEnum.Int.ToInt();
            Assert.AreEqual(0, iEnum);
        }

        [TestMethod("Convert Int To Enum")]
        public void Convert_Int_To_Enum()
        {
            var e = 1.ToEnum<TestEnum>();
            Assert.AreEqual(TestEnum.Long, e);
        }

        [TestMethod("Compare enums")]
        [DataRow(TestEnum.Int, TestEnum.Int, true)]
        [DataRow(TestEnum.Int, TestEnum.Long, false)]
        [DataRow(TestEnum.Long, TestEnum.Int, false)]
        public void Compare_Enum(TestEnum x, TestEnum y, bool output)
        {
            Assert.AreEqual(output, x.Is(y));
        }

        [TestMethod("Enum GetEnumInformation")]
        public void Enum_GetEnumInformation()
        {
            var x = TestEnum.Int;
            var info = x.GetEnumInformation();

            Assert.IsNotNull(info);
            Assert.AreEqual("Int", info.Code);
            Assert.AreEqual(x, info.Value);
        }

        [TestMethod("Enum GetListEnumInformation")]
        public void Enum_GetListEnumInformation()
        {
            var list = typeof(TestEnum).GetListEnumInformation<TestEnum>();

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Int", list[0].Code);
            Assert.AreEqual(TestEnum.Int, list[0].Value);

            Assert.AreEqual("Long", list[1].Code);
            Assert.AreEqual(TestEnum.Long, list[1].Value);
        }
    }
}