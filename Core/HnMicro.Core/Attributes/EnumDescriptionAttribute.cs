namespace HnMicro.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumDescriptionAttribute : Attribute
    {
        public string Code;

        public EnumDescriptionAttribute(string code)
        {
            Code = code;
        }
    }
}
