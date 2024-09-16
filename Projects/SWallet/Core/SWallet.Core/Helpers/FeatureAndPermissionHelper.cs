using SWallet.Core.Consts;
using SWallet.Core.Models.Features;

namespace SWallet.Core.Helpers
{
    public static class FeatureAndPermissionHelper
    {
        public static List<StaticFeatureModel> AllFeatures = new()
        {
            FeatureConsts.BankAccountFeature,
            FeatureConsts.BankFeature,
            FeatureConsts.CloneFeature,
            FeatureConsts.FeaturesAndPermissionsFeature,
            FeatureConsts.PaymentMethodFeature,
            FeatureConsts.CustomerLevelFeature,
            FeatureConsts.RoleFeature,
            FeatureConsts.AgentFeature,
            FeatureConsts.ManagerFeature,
            FeatureConsts.CustomerFeature,
            FeatureConsts.TicketsFeature,
            FeatureConsts.TransactionsFeature
        };
    }
}
