namespace HnMicro.Framework.Helpers
{
    public static class RouteHelper
    {
        public static class BaseRoute
        {
            public static class V1
            {
                public const string BaseRoute = "api/v1/[controller]";
                public const string BaseRouteNoneController = "api/v1";
            }
        }
    }
}
