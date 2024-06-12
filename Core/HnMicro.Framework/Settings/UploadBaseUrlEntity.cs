namespace HnMicro.Framework.Settings
{
    public class UploadBaseUrlEntity : BaseSettingEntity
    {
        public string Address { get; set; }

        public static UploadBaseUrlEntity Create(string address)
        {
            return new UploadBaseUrlEntity
            {
                Address = address
            };
        }

        public static string CreateJson(string address)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Create(address));
        }
    }
}
