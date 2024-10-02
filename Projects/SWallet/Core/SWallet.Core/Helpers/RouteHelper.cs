namespace SWallet.Core.Helpers
{
    public static class RouteHelper
    {
        public static class V1
        {
            public static class BankAccount
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/bank-account";
            }

            public static class CustomerBankAccount
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/customer-bank-account";
            }

            public static class PaymentMethod
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/payment-method";
            }
        }
    }
}
