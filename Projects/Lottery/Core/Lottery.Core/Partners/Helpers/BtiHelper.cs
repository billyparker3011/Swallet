using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lottery.Core.Partners.Helpers
{
    public static class BtiHelper
    {
        public static class BtiResponseCodeHelper
        {
            public static int No_Errors = 0;
            public static int Generic_Error  = -1;
            public static int Customer_Not_Found = -2;
            public static int Token_Not_Valid = -3;
            public static int Insufficient_Funds = -4;
            public static int Restricted_Customer = -6;
            public static int No_Existing_Session = -23;
        }

        public static class BtiTicketStatusHelper
        {
            public static int Open = 1;
            public static int Debit = 2;
            public static int Commit = 3;
            public static int Complete = 4;
            public static int Cancel = 5;

            public static List<int> Betting = new List<int> { Open, Debit };
            public static List<int> Betted = new List<int> { Commit };
        }

        public static class BtiGameStatusHelper
        {
            public static int Opened = 0;
            public static int Lost = 1;
            public static int Won = 2;
            public static int Draw = 3;
            public static int Canceled = 4;
            public static int Half_Lost = 16;
            public static int Half_Won = 17;
            public static int Cashout = 32;
            public static int First_Place = 7;
            public static int Second_Place = 8;
            public static int Third_Place = 9;
            public static int Fourth_Place = 10;
            public static int Fifth_Place = 11;
            public static int Sixth_Plac = 12;
            public static int Seventh_Place = 18;
            public static int Eighth_Place = 19;
            public static int Ninth_Place = 20;
            public static int Tenth_Place = 21;
            public static int Eleventh_Place = 22;
            public static int Twelve_Place = 23;
            public static int Thirteenth_Place = 24;
            public static int Fourteenth_Place = 25;
            public static int Fifteenth_Place = 26;
            public static int Sixteenth_Place = 27;
            public static int Seventeenth_Place = 28;
            public static int Eighteenth_Place = 29;
            public static int Nineteenth_Place = 30;
            public static int Twentieth_Place = 31;

        }

        public static class BtiTypeHelper
        {
            public static int Reverse = 1;
            public static int DebitReverse = 2;
            public static int CancelReverse = 3;
            public static int CommitReverse = 4;
            public static int DebitCustomer = 5;
            public static int CreditCustomer = 6;

            public static List<int> AmountType = new List<int> { Reverse, DebitReverse, DebitCustomer, CreditCustomer };
        }

        public static async Task<T> DeserializeXmlAsync<T>(Stream xmlStream)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var reader = new StreamReader(xmlStream))
                {
                    var xmlContent = await reader.ReadToEndAsync();
                    using (var stringReader = new StringReader(xmlContent))
                    {
                        return (T)xmlSerializer.Deserialize(stringReader);
                    }
                }
            }
            catch (Exception ex)
            {

               return default(T);
            }

        }
    }
}
