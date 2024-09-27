namespace SWallet.Core.Models.Features
{
    public class StaticFeatureModel
    {
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public List<StaticPermissionModel> Permissions { get; set; }
    }
}
