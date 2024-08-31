using Lottery.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lottery.Core.Helpers
{
    public static class PartnerHelper
    {
        public static class CasinoPartnerKey
        {
            public static string CasinoBookieSettingKey = "CABSKey";
        }

        public static class CasinoReponseCode
        {
            public static int Success = 0;
            public static int Invalid_Operator_ID = 10000;
            public static int Invalid_Signature = 10001;
            public static int Player_account_does_not_exist = 10003;
            public static int Player_account_is_disabled_or_not_allowed_to_log_in = 10005;
            public static int Transaction_not_existed = 10006;
            public static int Invalid_status = 10007;
            public static int Player_is_offline_or_logged_out = 10008;
            public static int Prohibit_to_bet = 10100;
            public static int Credit_is_not_enough = 10101;
            public static int System_is_under_maintenance = 10200;
            public static int Invalid_request_parameter = 40000;
            public static int Server_error = 50000;

        }

        public static class CasinoPathPost
        {
            public static string CheckOrCreate = "/CheckOrCreate";
            public static string Login = "/Login";
            public static string LoginTrial = "/LoginTrial";
            public static string Logout = "/Logout";
            public static string GetPlayerSetting = "/GetPlayerSetting";
            public static string ModifyPlayerSetting = "/ModifyPlayerSetting";
            public static string GetAgentHandicaps = "/GetAgentHandicaps";
            public static string SetDefaultHandicapsForCreatingPlayer = "/SetDefaultHandicapsForCreatingPlayer";
            public static string GetGameTables = "/GetGameTables";
            public static string GetMaintenanceState = "/GetMaintenanceState";
            public static string SetMaintenanceState = "/SetMaintenanceState";
            public static string QueryWinningBetType = "/QueryWinningBetType";

            public static string QueryBetRecordByBetNum = "/QueryBetRecordByBetNum";
            public static string QuickQueryBetRecords = "/QuickQueryBetRecords";
            public static string PagingQueryBetRecords = "/PagingQueryBetRecords";
            public static string QueryModifiedBetRecords = "/QueryModifiedBetRecords";
            public static string QuerySumHistories = "/QuerySumHistories";
            public static string QueryOneDaySumHistories = "/QueryOneDaySumHistories";
            public static string PagingQueryEventRecords = "/PagingQueryEventRecords";
            public static string PagingQueryBetRecordsByPlayer = "/PagingQueryBetRecordsByPlayer";

        }

        public static class CasinoPartnerPath
        {
            public static string GetBalance = "getbalance";
            public static string Transfer = "transfer";
            public static string CancelTransfer = "canceltransfer";

        }

        public static class CasinoBetStatus
        {
            public static int Betting = 100;
            public static int BetFailed = 101;
            public static int NotSettled = 110;
            public static int Settled = 111;
            public static int Refund = 120;

            public static int[] BetRunning = new int[] { Betting, NotSettled };
            public static int[] BetCompleted = new int[] { BetFailed, Settled, Refund };
        }

        public static class CasinoTransferType
        {
            public static int Bet = 10;
            public static int Settle = 20;
            public static int ManualSettle = 21;
            public static int TransferIn = 30;
            public static int TransferOut = 31;
            public static int EventSettle = 40;
        }

        public static class CasinoBetMethod
        {
            public static int Table_Bet = 0;
            public static int Multi_Play = 1;
            public static int Fast_Bet = 2;
            public static int VIP_Reserve = 3;
            public static int VIP_Join = 4;
            public static int VIP_Side_Bet = 5;
        }

        public static class CasinoAppType
        {
            public static int H5Mobile = 3;
            public static int H5PC = 6;
            public static int INT__open_Mobile_close_ = 7;
            public static int INT__open_PC_close_ = 9;
            public static int Android_APP = -1;
            public static int iPhone_APP = -2;
            public static int Android_APP_ = -3;
            public static int iPhone_APP_ = -4;
            public static int iPad_APP = -5;
            public static int iPad_APP_ = -6;
        }

        public static class CasinoGameType
        {
            public static int Normal_Baccarat = 101;
            public static int VIP_Baccarat = 111;
            public static int Quick_Baccarat = 103;
            public static int See_Card_Baccarat = 104;
            public static int Insurance_Baccarat = 110;
            public static int Sicbo_open_HiLo_close_ = 201;
            public static int Fish_Prawn_Crab = 202;
            public static int Dragon_Tiger = 301;
            public static int Roulette = 401;
            public static int Classic_Pok_Deng__slash__Two_Sides_Pok_Deng = 501;
            public static int Rock_Paper_Scissors = 601;
            public static int Bull_Bull = 801;
            public static int Win_Three_Cards__slash__Three_Pictures = 901;
            public static int Ultimate_Texas_Hold = 702;
            public static int Andar_Bahar = 602;
            public static int Teen_Patti_20_dash_20 = 603;
            public static int Casino_War = 703;
            public static int Infinite_Blackjack = 704;
        }

        public static class CasinoBetType
        {
            public static class Baccarat
            {
                public static int Banker = 1001;
                public static int Player = 1002;
                public static int Tie = 1003;
                public static int Banker_Pair = 1006;
                public static int Player_Pair = 1007;
                public static int Lucky_Six = 1100;
                public static int Banker_Natural = 1211;
                public static int Player_Natural = 1212;
                public static int Any_Pair = 1223;
                public static int Perfect_Pair = 1224;
                public static int Banker_Dragon_Bonus = 1231;
                public static int Player_Dragon_Bonus = 1232;
                public static int Banker_Insurance_First = 1301;
                public static int Banker_Insurance_Second = 1302;
                public static int Player_Insurance_First = 1303;
                public static int Player_Insurance_Second = 1304;
                public static int Tiger = 1401;
                public static int Small_Tiger = 1402;
                public static int Big_Tiger = 1403;
                public static int Tiger_Pair = 1404;
                public static int Tiger_Tie = 1405;
                public static int Banker_Fabulous_4 = 1501;
                public static int Player_Fabulous_4 = 1502;
                public static int Banker_Precious_Pair = 1503;
                public static int Player_Precious_Pair = 1504;
                public static int Banker_Black = 1601;
                public static int Banker_Red = 1602;
                public static int Player_Black = 1603;
                public static int Player_Red = 1604;
                public static int Any_6 = 1605;
            }

            public static class DragonTiger
            {
                public static int Dragon = 2001;
                public static int Tiger = 2002;
                public static int Tie = 2003;
            }

            public static class Sicbo
            {
                public static int Small = 3001;
                public static int Odd = 3002;
                public static int Even = 3003;
                public static int Big = 3004;
                public static int All_or_specific_triples_for_one = 3005;
                public static int All_or_specific_triples_for_two = 3006;
                public static int All_or_specific_triples_for_three = 3007;
                public static int All_or_specific_triples_for_four = 3008;
                public static int All_or_specific_triples_for_five = 3009;
                public static int All_or_specific_triples_for_six = 3010;
                public static int All_or_any_triple = 3011;
                public static int Double_1 = 3012;
                public static int Double_2 = 3013;
                public static int Double_3 = 3014;
                public static int Double_4 = 3015;
                public static int Double_5 = 3016;
                public static int Double_6 = 3017;
                public static int Sum_4 = 3018;
                public static int Sum_5 = 3019;
                public static int Sum_6 = 3020;
                public static int Sum_7 = 3021;
                public static int Sum_8 = 3022;
                public static int Sum_9 = 3023;
                public static int Sum_10 = 3024;
                public static int Sum_11 = 3025;
                public static int Sum_12 = 3026;
                public static int Sum_13 = 3027;
                public static int Sum_14 = 3028;
                public static int Sum_15 = 3029;
                public static int Sum_16 = 3030;
                public static int Sum_17 = 3031;
                public static int Two_Dice_Combination_1_comma_2 = 3033;
                public static int Two_Dice_Combination_1_comma_3 = 3034;
                public static int Two_Dice_Combination_1_comma_4 = 3035;
                public static int Two_Dice_Combination_1_comma_5 = 3036;
                public static int Two_Dice_Combination_1_comma_6 = 3037;
                public static int Two_Dice_Combination_2_comma_3 = 3038;
                public static int Two_Dice_Combination_2_comma_4 = 3039;
                public static int Two_Dice_Combination_2_comma_5 = 3040;
                public static int Two_Dice_Combination_2_comma_6 = 3041;
                public static int Two_Dice_Combination_3_comma_4 = 3042;
                public static int Two_Dice_Combination_3_comma_5 = 3043;
                public static int Two_Dice_Combination_3_comma_6 = 3044;
                public static int Two_Dice_Combination_4_comma_5 = 3045;
                public static int Two_Dice_Combination_4_comma_6 = 3046;
                public static int Two_Dice_Combination_5_comma_6 = 3047;
                public static int Single_1 = 3048;
                public static int Single_2 = 3049;
                public static int Single_3 = 3050;
                public static int Single_4 = 3051;
                public static int Single_5 = 3052;
                public static int Single_7 = 3053;
                public static int Hi = 3200;
                public static int Lo = 3201;
                public static int Hi_Lo = 3202;
                public static int Dice_1 = 3203;
                public static int Dice_2 = 3204;
                public static int Dice_3 = 3205;
                public static int Dice_4 = 3206;
                public static int Dice_5 = 3207;
                public static int Dice_6 = 3208;
                public static int _empty_1_dash_2 = 3209;
                public static int _empty_1_dash_3 = 3210;
                public static int _empty_1_dash_4 = 3211;
                public static int _empty_1_dash_5 = 3212;
                public static int _empty_1_dash_6 = 3213;
                public static int _empty_2_dash_3 = 3214;
                public static int _empty_2_dash_4 = 3215;
                public static int _empty_2_dash_5 = 3216;
                public static int _empty_2_dash_6 = 3217;
                public static int _empty_3_dash_4 = 3218;
                public static int _empty_3_dash_5 = 3219;
                public static int _empty_3_dash_6 = 3220;
                public static int _empty_4_dash_5 = 3221;
                public static int _empty_4_dash_6 = 3222;
                public static int _empty_5_dash_6 = 3223;
                public static int _empty_1_dash_Lo = 3224;
                public static int _empty_2_dash_Lo = 3225;
                public static int _empty_3_dash_Lo = 3226;
                public static int _empty_4_dash_Lo = 3227;
                public static int _empty_5_dash_Lo = 3228;
                public static int _empty_6_dash_Lo = 3229;
                public static int _empty_3_dash_Hi = 3230;
                public static int _empty_4_dash_Hi = 3231;
                public static int _empty_5_dash_Hi = 3232;
                public static int _empty_6_dash_Hi = 3233;
                public static int _empty_1_comma_2_comma_3 = 3234;
                public static int _empty_2_comma_3_comma_4 = 3235;
                public static int _empty_3_comma_4_comma_5 = 3236;
                public static int _empty_4_comma_5_comma_6 = 3237;
            }
            public static class Roulette
            {
                public static int Small = 4001;
                public static int Even = 4002;
                public static int Red = 4003;
                public static int Black = 4004;
                public static int Odd = 4005;
                public static int Big = 4006;
                public static int[] First_dozen_to_third_dozen = new int[] { 4007, 4009 };
                public static int[] First_column_to_third_column = new int[] { 4010, 4012 };
                public static int[] Direct_bet = new int[] { 4013, 4049 };
                public static int[] Three_numbers = new int[] { 4050, 4051 };
                public static int Corner_bets = 4052;
                public static int[] Separate_open_0_slash_1_close__comma__open_0_slash_2_close__comma__open_0_slash_3_close_ = new int[] { 4053, 4054, 4055 };
                public static int[] Separate_open_1_slash_2_close__comma__open_2_slash_3_close__comma__open_4_slash_5_close__comma__open_5_slash_6_close__comma__open_7_slash_8_close__comma__open_8_slash_9_close_ = new int[] { 4056, 4057, 4058, 4059, 4060, 4061 };
                public static int[] Separate_open_10_slash_11_close__comma__open_11_slash_12_close__comma__open_13_slash_14_close__comma__open_14_slash_15_close_ = new int[] { 4062, 4063, 4064, 4065 };
                public static int[] Separate_open_16_slash_17_close__comma__open_17_slash_18_close__comma__open_19_slash_20_close__comma__open_20_slash_21_close_ = new int[] { 4066, 4067, 4068, 4069 };
                public static int[] Separate_open_22_slash_23_close__comma__open_23_slash_24_close__comma__open_25_slash_26_close__comma__open_26_slash_27_close_ = new int[] { 4070, 4071, 4072, 4073 };
                public static int[] Separate_open_28_slash_29_close__comma__open_29_slash_30_close__comma__open_31_slash_32_close__comma__open_32_slash_33_close__comma__open_34_slash_35_close__comma__open_35_slash_36_close_ = new int[] { 4074, 4075, 4076, 4077, 4078, 4079 };
                public static int[] Separate_open_1_slash_4_close__comma__open_4_slash_7_close__comma__open_7_slash_10_close_ = new int[] { 4080, 4081, 4082 };
                public static int[] Separate_open_10_slash_13_close__comma__open_13_slash_16_close__comma__open_16_slash_19_close_ = new int[] { 4083, 4084, 4085 };
                public static int[] Separate_open_19_slash_22_close__comma__open_22_slash_25_close__comma__open_25_slash_28_close_ = new int[] { 4086, 4087, 4088 };
                public static int[] Separate_open_28_slash_31_close__comma__open_31_slash_34_close_ = new int[] { 4089, 4090 };
                public static int[] Separate_open_2_slash_5_close__comma__open_5_slash_8_close__comma__open_8_slash_11_close_ = new int[] { 4091, 4092, 4093 };
                public static int[] Separate_open_11_slash_14_close__comma__open_14_slash_17_close__comma__open_17_slash_20_close_ = new int[] { 4094, 4095, 4096 };
                public static int[] Separate_open_20_slash_23_close__comma__open_23_slash_26_close__comma__open_26_slash_29_close_ = new int[] { 4097, 4098, 4099 };
                public static int[] Separate_open_29_slash_32_close__comma__open_32_slash_35_close_ = new int[] { 4100, 4101 };
                public static int[] Separate_open_3_slash_6_close__comma__open_6_slash_9_close__comma__open_9_slash_12_close_ = new int[] { 4102, 4103, 4104 };
                public static int[] Separate_open_12_slash_15_close__comma__open_15_slash_18_close__comma__open_18_slash_21_close_ = new int[] { 4105, 4106, 4107 };
                public static int[] Separate_open_21_slash_24_close__comma__open_24_slash_27_close__comma__open_27_slash_30_close_ = new int[] { 4108, 4109, 4110 };
                public static int[] Separate_open_30_slash_33_close__comma__open_33_slash_36_close_ = new int[] { 4111, 4112 };
                public static int[] Corner_bets_open_1_slash_5_close__comma__open_2_slash_6_close__comma__open_4_slash_8_close__comma__open_5_slash_9_close__comma__open_7_slash_11_close__comma__open_8_slash_12_close_ = new int[] { 4113, 4114, 4115, 4116, 4117, 4118 };
                public static int[] Corner_bets_open_10_slash_14_close__comma__open_11_slash_15_close__comma__open_13_slash_17_close__comma__open_14_slash_18_close__comma__open_16_slash_20_close__comma__open_17_slash_21_close_ = new int[] { 4119, 4120, 4121, 4122, 4123, 4124 };
                public static int[] Corner_bets_open_19_slash_23_close__comma__open_20_slash_24_close__comma__open_22_slash_26_close__comma__open_23_slash_27_close__comma__open_25_slash_29_close__comma__open_26_slash_30_close_ = new int[] { 4125, 4126, 4127, 4128, 4129, 4130 };
                public static int[] Corner_bets_open_28_slash_32_close__comma__open_29_slash_33_close__comma__open_31_slash_35_close__comma__open_32_slash_36_close_ = new int[] { 4131, 4132, 4133, 4134 };
                public static int[] Street_bets_open_1_tilde_3_close__comma__open_4_tilde_6_close__comma__open_7_tilde_9_close__comma__open_10_tilde_12_close_ = new int[] { 4135, 4136, 4137, 4138 };
                public static int[] Street_bets_open_13_tilde_15_close__comma__open_16_tilde_18_close__comma__open_19_tilde_21_close__comma__open_22_tilde_24_close_ = new int[] { 4139, 4140, 4141, 4142 };
                public static int[] Street_bets_open_25_tilde_27_close__comma__open_28_tilde_30_close__comma__open_31_tilde_33_close__comma__open_34_tilde_36_close_ = new int[] { 4143, 4144, 4145, 4146 };
                public static int[] Line_bets_open_1_tilde_6_close__comma__open_4_tilde_9_close__comma__open_7_tilde_12_close_ = new int[] { 4147, 4148, 4149 };
                public static int[] Line_bets_open_10_tilde_15_close__comma__open_13_tilde_18_close__comma__open_16_tilde_21_close_ = new int[] { 4150, 4151, 4152 };
                public static int[] Line_bets_open_19_tilde_24_close__comma__open_22_tilde_27_close__comma__open_25_tilde_30_close__comma__open_28_tilde_33_close__comma__open_31_tilde_36_close_ = new int[] { 4153, 4154, 4155, 4156, 4157 };
            }

            public static class ClassicOrTwoSidesPokDeng
            {
                public static int Player_2__open_Classic_Pok_Deng_close_ = 5002;
                public static int Player_1__open_Classic_Pok_Deng_close_ = 5001;
                public static int Player_3__open_Classic_Pok_Deng_close_ = 5003;
                public static int Player_4__open_Classic_Pok_Deng_close_ = 5004;
                public static int Player_5__open_Classic_Pok_Deng_close_ = 5005;
                public static int Player_1_Pair = 5011;
                public static int Player_2_Pair = 5012;
                public static int Player_3_Pair = 5013;
                public static int Player_4_Pair = 5014;
                public static int Player_5_Pair = 5015;
                public static int Player_1__open_Two_Sides_Pok_Deng_close_ = 5101;
                public static int Player_2__open_Two_Sides_Pok_Deng_close_ = 5102;
                public static int Player_3__open_Two_Sides_Pok_Deng_close_ = 5103;
                public static int Player_4__open_Two_Sides_Pok_Deng_close_ = 5104;
                public static int Player_5__open_Two_Sides_Pok_Deng_close_ = 5105;
                public static int Banker_1 = 5106;
                public static int Banker_2 = 5107;
                public static int Banker_3 = 5108;
                public static int Banker_4 = 5109;
                public static int Banker_5 = 5110;
                public static int Banker_Pair = 5111;
            }

            public static class RockPaperScissors
            {
                public static int Gold_Rock = 6001;
                public static int Gold_Paper = 6002;
                public static int Gold_Scissors = 6003;
                public static int Silver_Rock = 6004;
                public static int Silver_Paper = 6005;
                public static int Silver_Scissors = 6006;
                public static int Bronze_Rock = 6007;
                public static int Bronze_Paper = 6008;
                public static int Bronze_Scissors = 6009;
            }

            public static class BullBull
            {
                public static int Banker_1_Equal = 8001;
                public static int Banker_1_Double = 8011;
                public static int Player_1_Equal = 8101;
                public static int Player_1_Double = 8111;
                public static int Banker_2_Equal = 8002;
                public static int Banker_2_Double = 8012;
                public static int Player_2_Equal = 8102;
                public static int Player_2_Double = 8112;
                public static int Banker_3_Equal = 8003;
                public static int Banker_3_Double = 8013;
                public static int Player_3_Equal = 8103;
                public static int Player_3_Double = 8113;
                public static int Banker_1_Super_Bull = 8021;
                public static int Player_1_Super_Bull = 8121;
                public static int Banker_2_Super_Bull = 8022;
                public static int Player_2_Super_Bull = 8122;
                public static int Banker_3_Super_Bull = 8023;
                public static int Player_3_Super_Bull = 8123;
            }

            public static class WinThreeCardsOrThreePictures
            {
                public static int Dragon = 9001;
                public static int Phoenix = 9002;
                public static int Pair_8_Plus = 9003;
                public static int Straight = 9004;
                public static int Flush = 9005;
                public static int Straight_Flush = 9006;
                public static int Three_of_a_kind = 9007;
                public static int _open_Three_Pictures_close_Dragon = 9101;
                public static int _open_Three_Pictures_close_Phoenix = 9102;
                public static int Tie = 9103;
                public static int Dragon_Three_Pictures = 9114;
                public static int Phoenix_Three_Pictures = 9124;
            }

            public static class UltimateTexasHold
            {
                public static int Player_1__open_Ante_close_ = 7201;
                public static int Player_1__open_Blind_close_ = 7202;
                public static int Player_1__open_Trips_close_ = 7203;
                public static int Player_1_Play_open_Raise_4X_close_ = 7204;
                public static int Player_1_Play_open_Raise_3X_close_ = 7205;
                public static int Player_1_Play_open_Raise_2X_close_ = 7206;
                public static int Player_1_Play_open_Raise_1X_close_ = 7207;
                public static int Player_2__open_Ante_close_ = 7211;
                public static int Player_2__open_Blind_close_ = 7212;
                public static int Player_2__open_Trips_close_ = 7213;
                public static int Player_2_Play_open_Raise_4X_close_ = 7214;
                public static int Player_2_Play_open_Raise_3X_close_ = 7215;
                public static int Player_2_Play_open_Raise_2X_close_ = 7216;
                public static int Player_2_Play_open_Raise_1X_close_ = 7217;
                public static int Player_3__open_Ante_close_ = 7221;
                public static int Player_3__open_Blind_close_ = 7222;
                public static int Player_3__open_Trips_close_ = 7223;
                public static int Player_3_Play_open_Raise_4X_close_ = 7224;
                public static int Player_3_Play_open_Raise_3X_close_ = 7225;
                public static int Player_3_Play_open_Raise_2X_close_ = 7226;
                public static int Player_3_Play_open_Raise_1X_close_ = 7227;
            }

            public static class AndarBahar
            {
                public static int Andar = 6201;
                public static int Bahar = 6202;
                public static int _empty_1_dash_5_Cards = 6203;
                public static int _empty_6_dash_10_Cards = 6204;
                public static int _empty_11_dash_15_Cards = 6205;
                public static int _empty_16_dash_25_Cards = 6206;
                public static int _empty_26_dash_30_Cards = 6207;
                public static int _empty_31_dash_35_Cards = 6208;
                public static int _empty_36_dash_40_Cards = 6209;
                public static int _empty_41_plus__Cards = 6210;
                public static int _empty_1st_Andar = 6211;
                public static int _empty_1st_Bahar = 6212;
            }

            public static class TeenPatti2020
            {
                public static int Player_A = 6301;
                public static int Player_B = 6302;
                public static int Tie = 6303;
                public static int Player_A_Pair_plus_ = 6304;
                public static int Player_B_Pair_plus_ = 6305;
                public static int _empty_6_Card_Bonus = 6306;
            }

            public static class FishPrawnCrab
            {
                public static int Fish = 3301;
                public static int Prawn = 3302;
                public static int Calabash = 3303;
                public static int Coins = 3304;
                public static int Crab = 3305;
                public static int Cock = 3306;
                public static int Specific_Single_Red = 3307;
                public static int Specific_Single_Green = 3308;
                public static int Specific_Single_Blue = 3309;
                public static int Specific_Double_Red = 3310;
                public static int Specific_Double_Green = 3311;
                public static int Specific_Double_Blue = 3312;
                public static int Specific_Triple_Red = 3313;
                public static int Specific_Triple_Green = 3314;
                public static int Specific_Triple_Blue = 3315;
                public static int Any_Triple_Color = 3316;
                public static int Calabash__ampersand__Prawn = 3317;
                public static int Calabash__ampersand__Fish = 3318;
                public static int Calabash__ampersand__Cock = 3319;
                public static int Calabash__ampersand__Crab = 3320;
                public static int Calabash__ampersand__Coins = 3321;
                public static int Prawn__ampersand__Fish = 3322;
                public static int Prawn__ampersand__Cock = 3323;
                public static int Prawn__ampersand__Crab = 3324;
                public static int Prawn__ampersand__Coins = 3325;
                public static int Fish__ampersand__Cock = 3326;
                public static int Fish__ampersand__Crab = 3327;
                public static int Fish__ampersand__Coins = 3328;
                public static int Cock__ampersand__Crab = 3329;
                public static int Cock__ampersand__Coins = 3330;
                public static int Crab__ampersand__Coins = 3331;
            }

            public static class CasinoWar
            {
                public static int Player_1_Ante = 7301;
                public static int Player_1_First_Tie = 7302;
                public static int Player_1_War = 7303;
                public static int Player_1_Second_Tie = 7304;
                public static int Player_2_Ante = 7311;
                public static int Player_2_First_Tie = 7312;
                public static int Player_2_War = 7313;
                public static int Player_2_Second_Tie = 7314;
                public static int Player_3_Ante = 7321;
                public static int Player_3_First_Tie = 7322;
                public static int Player_3_War = 7323;
                public static int Player_3_Second_Tie = 7324;
            }

            public static class InfiniteBlackjack
            {
                public static int Ante = 7401;
                public static int Ante_Double = 7402;
                public static int Split = 7403;
                public static int Pair = 7404;
                public static int Lucky_3 = 7405;
                public static int Hot_3 = 7406;
                public static int Bust_it = 7407;
                public static int Insurance = 7408;
                public static int Lucky_Ladies = 7409;
            }
        }

        public static string FindStaticFieldName(Type type, int value)
        {

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType.Name == typeof(Int32).Name && (Int32)field.GetValue(null) == value)
                {
                    return GetFieldName(field.Name);
                }
                else if (field.FieldType.Name == typeof(Int32[]).Name && ((Int32[])field.GetValue(null)).Contains(value))
                {
                    return GetFieldName(field.Name);
                }
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                var result = FindStaticFieldName(nestedType, value);
                if (result != null)
                {
                    return GetFieldName(result);
                }
            }

            return null;
        }

        public static string GetFieldName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return name.Replace("_ampersand_", "&").Replace("_slash_", "/").Replace("_empty_", "").Replace("_plus_", "+").Replace("_comma_", ",").Replace("_close_", ")").Replace("_open_", "(").Replace("_dash_", "-").Replace("_", " ");
        }
    }
}
