namespace SWallet.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumPaymentPartnerAttribute : Attribute
    {
        public EnumPaymentPartnerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
