using System.Collections.Generic;

namespace HnMicro.Framework.Settings
{
    public class UploadSettingEntity : BaseSettingEntity
    {
        public long MaxFileSizeInKb { get; set; }
        public List<string> Extensions { get; set; }

        public static UploadSettingEntity Create(long maxSize, List<string> extensions)
        {
            return new UploadSettingEntity
            {
                MaxFileSizeInKb = maxSize, 
                Extensions = extensions
            };
        }

        public static string CreateJson(long maxSize, List<string> extensions)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Create(maxSize, extensions));
        }
    }
}
