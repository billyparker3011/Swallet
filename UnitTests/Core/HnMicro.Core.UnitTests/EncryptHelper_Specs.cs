using HnMicro.Core.Helpers;
using HnMicro.Framework.UnitTests.Models;

namespace HnMicro.Core.UnitTests
{
    [TestClass]
    public class EncryptHelper_Specs : BaseSpecs
    {
        [TestMethod("Md5")]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("KY3KWCVFUt", "6eb5fa8dada1c5659a7200c271dc98e8")]
        [DataRow("lOlkeVKpII", "1f69c1eb5f90abbdccf25c0eb220bf9e")]
        [DataRow("rhTT4ktnIV", "b2f43c037bd2f5ce5f089c32c9ad9cf1")]
        [DataRow("3GYdLB83hv", "ab96023478dc26f2dbcaa8373d83a3ef")]
        public void Md5(string input, string output)
        {
            var expected = input.Md5();
            Assert.AreEqual(output, expected);
        }

        [TestMethod("Sha")]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("KY3KWCVFUt", "294f1fbb77867e3f84ccdc888daafd2503f2093faf09b740332dfa19770a3e37")]
        [DataRow("lOlkeVKpII", "e0dd10972cb01c59b2b0bb29a73cbe486b0982c326123c2c734955d99250216a")]
        [DataRow("rhTT4ktnIV", "1a4eaf81ea21d64ca920ceaf40d521c9ed7fe967bdbf26d9cde0fd1a7d2dc040")]
        [DataRow("3GYdLB83hv", "e941d7ca04c8a5328c3f0f579b0b2f1a5e8a637fc7514330404bbeaeb7bd48e0")]
        public void Sha(string input, string output)
        {
            var expected = input.Sha();
            Assert.AreEqual(output, expected);
        }

        [TestMethod("Base64Encode")]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("KY3KWCVFUt", "S1kzS1dDVkZVdA==")]
        [DataRow("lOlkeVKpII", "bE9sa2VWS3BJSQ==")]
        [DataRow("rhTT4ktnIV", "cmhUVDRrdG5JVg==")]
        [DataRow("3GYdLB83hv", "M0dZZExCODNodg==")]
        public void Base64Encode(string input, string output)
        {
            var expected = input.Base64Encode();
            Assert.AreEqual(output, expected);
        }

        [TestMethod("Base64Decode")]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("S1kzS1dDVkZVdA==", "KY3KWCVFUt")]
        [DataRow("bE9sa2VWS3BJSQ==", "lOlkeVKpII")]
        [DataRow("cmhUVDRrdG5JVg==", "rhTT4ktnIV")]
        [DataRow("M0dZZExCODNodg==", "3GYdLB83hv")]
        public void Base64Decode(string input, string output)
        {
            var expected = input.Base64Decode();
            Assert.AreEqual(output, expected);
        }

        [TestMethod("CustomPassword")]
        [DataRow("KY3KWCVFUt", "S1kzS1dDVkZVdA==", "98ccdbd9f303eceb6d28d082cea1d9a8")]
        [DataRow("lOlkeVKpII", "bE9sa2VWS3BJSQ==", "6cd82e68c460ae8edfb258f950926307")]
        [DataRow("rhTT4ktnIV", "cmhUVDRrdG5JVg==", "ce35b203ab828c5aea18113f10d0b3c2")]
        [DataRow("3GYdLB83hv", "M0dZZExCODNodg==", "88a590a709986b6e370ed924bdddaee5")]
        public void CustomPassword(string input, string hash, string output)
        {
            var expected = input.CustomPassword(hash);
            Assert.AreEqual(output, expected);
        }
    }
}