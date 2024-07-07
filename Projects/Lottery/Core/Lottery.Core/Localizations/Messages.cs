namespace Lottery.Core.Localizations
{
    public static class Messages
    {
        public static class Auth
        {
            public const string UserNameIsBlank = "Username is blank";
            public const string PasswordIsBlank = "Password is blank";
            public const string UserNameIsRequired = "Username is required.";
            public const string PasswordIsRequired = "Password is required.";
            public const string SecurityCodeIsRequired = "Security Code is required.";
            public const string PasswordIsIncorrect = "Password is incorrect.";
            public const string SecurityCodeIsIncorrect = "Security Code is incorrect.";
        }

        public static class Agent
        {
            public const string UserNameIsRequired = "Username is required.";
            public const string PasswordIsRequired = "Password is required.";
            public const string CreditIsRequired = "Credit is required.";
            public const string UserNameNotContainsWhiteSpace = "Username not allow to contain white space.";
            public const string UserNameDoesNotContainSpecialCharacters = "Username does not contain special characters";
            public const string UserNameMustHaveAtLeastTwoCharacters = "Username must have at least 2 characters";
        }

        public static class Match
        {
            public const string KickOffIsGreaterOrEqualsCurrentDate = "Kickoff must be greater or equals today.";
        }

        public static class MatchResult
        {
            public const string ResultsCanNotBeNull = "Results cannot be NULL.";
        }

        public static class Ticket
        {
            public const string BetKindIdIsBlank = "Bet Kind is required.";
            public const string MatchIdIsBlank = "Match is required.";
            public const string NumbersAreBlank = "Selected Numbers is required.";
            public const string NumbersAreDuplicate = "Numbers are duplicate.";
            public const string NumberIsGreaterThanOrEqualToZero_LessThanOrEqual99 = "Number is greater than or equals to 0, less than or equals to 99.";
            public const string PointIsGreaterThanToZero = "Point is greater than 0.";
        }

        public static class Odd
        {
            public const string OddsIsBlank = "Odds is required.";
            public const string OddIdIsBlank = "Odd is required.";
            public const string BetKindIsBlank = "BetKind is required.";
            public const string MinBuyIsBlank = "MinBuy is required.";
            public const string MaxBuyIsBlank = "MaxBuy is required.";
            public const string MaxBuyIsGreaterThanOrEqualToMinBuy = "MaxBuy is greater than or equal to MinBuy.";
            public const string BuyIsBlank = "Buy is required.";
            public const string BuyIsGreaterThanOrEqualToMinBuy = "Buy is greater than or equal to MinBuy.";
            public const string BuyIsLessThanOrEqualToMaxBuy = "Buy is less than or equal to MaxBuy.";
            public const string MinBetIsBlank = "MinBet is required.";
            public const string MinBetIsGreaterThanZero = "MinBet is grerater than zero.";
            public const string MaxBetIsBlank = "MaxBet is required.";
            public const string MaxBetIsGreaterThanOrEqualToMinBet = "MinBet is grerater than or equal to MinBet.";
            public const string MaxPerNumberIsBlank = "MaxPerNumber is required.";
            public const string MaxPerNumberIsGreaterThanZero = "MaxPerNumber is grerater than zero.";
        }

        public static class Announcement
        {
            public const string AnnouncementLevelIsRequired = "Announcement level is required.";
        }

        public static class BalanceTableSetting
        {
            public const string ErrorValueBalanceTableSetting = "Value setting of balance table setting is not valid.";
        }
    }
}
