namespace HnMicro.Framework.Models
{
    //  This class only uses to give an example
    public class ExampleInformation
    {
        public DateOnly Date { get; set; }

        public int TemperatureCelsius { get; set; }

        public int TemperatureFahrenheit => 32 + (int)(TemperatureCelsius / 0.5556);

        public string Summary { get; set; }
    }
}
