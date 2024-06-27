namespace Lottery.Core.Helpers
{
    public static class RouteHelper
    {
        public static class V1
        {
            public static class Prize
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/prizes";
            }

            public static class Channel
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/channels";
            }

            public static class BetKind
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/bet-kind";
            }

            public static class PositionTaking
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/position-taking";
            }

            public static class Announcement
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/announcements";
            }

            public static class MatchResult
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/match-result";
            }

            public static class Setting
            {
                public const string BaseRoute = HnMicro.Framework.Helpers.RouteHelper.BaseRoute.V1.BaseRouteNoneController + "/settings";
            }
        }
    }
}
