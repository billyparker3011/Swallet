namespace Lottery.Core.Helpers
{
    public static class ErrorCodeHelper
    {
        public static class Auth
        {
            public const int UserPasswordIsWrong = -1;
            public const int UserClosed = -2;
            public const int UserLocked = -3;
        }

        public static class ChangeInfo
        {
            public const int OldPasswordWrong = -2;
            public const int NewPasswordDoesnotMatch = -3;
            public const int PasswordComplexityIsWeak = -4;
            public const int WrongSecurityCodeFormat = -5;
            public const int NewSecurityCodeDoesnotMatch = -6;
        }

        public static class Agent
        {
            public const int InvalidCredit = -6;
            public const int UsernameIsExist = -7;
            public const int CreditOverLimitation = -8;
        }

        public static class ProcessTicket
        {
            public const int TheBetKindDoesNotAllowProcessing = -1;
            public const int MatchClosedOrSuspended = -2;
            public const int NotAccepted = -3;
            public const int CannotReadBetSetting = -4;
            public const int CannotReadAgentOdds = -5;
            public const int PointIsInvalid = -6;
            public const int MaxPerNumberIsInvalid = -7;
            public const int GivenCreditIsInvalid = -8;
            public const int CannotFindOddsOfNumber = -9;
            public const int FirstNorthern_Northern_DeTruot_MustChooseAtLeast10 = -10;
            public const int FirstNorthern_Northern_LoTruot_MustChooseAtLeast4 = -11;
            public const int PlayerIsSuspended = -12;
            public const int PlayerIsClosed = -13;
            public const int ChannelIsClosed = -14;
            public const int PrizeOrPostionIsInvalid = -15;
            public const int NoOfSelectedNumbersExceed512 = -16;
            public const int NumberIsLessThan = -17;
            public const int NumbersWasSuspended = -18;
            public const int CannotFindMatch = -19;
        }

        public static class Match
        {
            public const int KickOffIsGreaterOrEqualsCurrentDate = -1;
            public const int RunningMatchIsAlreadyExisted = -2;
            public const int MatchCodeIsAlreadyExisted = -3;
        }

        public static class MatchChangeState
        {
            public const int NorthernHasMoreResult = -1;
            public const int NorthernResultHasNotUpdatedYet = -2;
            public const int NorthernResultCannotFindPrize = -3;
            public const int NorthernResultDoesntMatchPrize = -4;
            public const int NorthernResultIsBadFormat = -5;
        }

        public static class CockFight
        {
            public const int PlayerHasNotBeenInitiatedYet = -100;
            public const int BookieSettingIsNotBeingInitiated = -101;
            public const int PartnerAccountIdHasNotBeenProvided = -102;
        }
    }
}
