namespace SWallet.Core.Helpers
{
    public static class RouteHelper
    {
        public static class V1
        {
            public static class CustomerBankAccount
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/customer-bank-account";
            }
        }
    }
}
