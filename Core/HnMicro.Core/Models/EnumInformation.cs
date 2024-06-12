namespace HnMicro.Core.Models
{
    public class EnumInformation<T>
        where T : Enum
    {
        public string Code { get; set; }
        public T Value { get; set; }
    }
}
