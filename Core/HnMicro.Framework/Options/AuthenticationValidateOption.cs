namespace HnMicro.Framework.Options
{
    public class AuthenticationValidateOption
    {
        public const string AppSettingName = "AuthenticationValidate";

        public string AuthenticationAddress { get; set; }
        //  Audience
        public bool ValidateAudience { get; set; }
        public string ValidAudience { get; set; }
        //  Issuer
        public bool ValidateIssuer { get; set; }
        public string ValidIssuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string IssuerSigningKey { get; set; }
        //  Lifetime
        public bool ValidateLifetime { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}
