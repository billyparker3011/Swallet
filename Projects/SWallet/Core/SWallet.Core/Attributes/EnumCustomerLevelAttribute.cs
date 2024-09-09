namespace SWallet.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumCustomerLevelAttribute : Attribute
    {
        public EnumCustomerLevelAttribute(string code)
        {
            LevelCode = code;
        }

        public string LevelCode { get; set; }
    }
}
