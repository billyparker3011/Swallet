using HnMicro.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace HnMicro.Core.Helpers
{
    public static class StringHelper
    {
        private static List<string> _onlyNumerics = new() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private static List<string> _characters = new() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private const string _chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789";
        private const string _charsWithSpecial = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789`~!@#$%^&*()_-+={}[]\\|;:'\",<.>/?";
        private const string _emailPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";
        private const string _regexOnlyNumberPattern = @"^\d+$";

        public const int MaxHashLength = 10;

        public static JsonSerializerSettings CamelCaseJsonSetting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static bool IsEmail(this string email)
        {
            email = string.IsNullOrEmpty(email) ? string.Empty : email.Trim();
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, _emailPattern, RegexOptions.IgnoreCase);
        }

        public static bool IsStrongPassword(this string password, int length = 8)
        {
            if (password.Length < length) return false;

            var success = 0;

            var i = 0;
            var count = 0;

            //  First rule
            const string uppercaseLetter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            while (i < password.Length)
            {
                if (uppercaseLetter.IndexOf(password.Substring(i, 1), StringComparison.Ordinal) >= 0) count++;
                i++;
            }
            if (count > 0) success++;

            //  Second rule
            const string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            i = 0;
            count = 0;
            while (i < password.Length)
            {
                if (lowercaseLetters.IndexOf(password.Substring(i, 1), StringComparison.Ordinal) >= 0) count++;
                i++;
            }
            if (count > 0) success++;

            //  Third rule
            const string numbers = "0123456789";
            i = 0;
            count = 0;
            while (i < password.Length)
            {
                if (numbers.IndexOf(password.Substring(i, 1), StringComparison.Ordinal) >= 0) count++;
                i++;
            }
            if (count > 0) success++;

            //  Fourth rule
            const string specialCharacter = "`~!@#$%^&*()_+=[]{}\\|;:'\",<.>/?";
            i = 0;
            count = 0;
            while (i < password.Length)
            {
                if (specialCharacter.IndexOf(password.Substring(i, 1), StringComparison.Ordinal) >= 0) count++;
                i++;
            }
            if (count > 0) success++;

            if (password.IndexOf(" ", StringComparison.Ordinal) > -1) return false;
            return success >= 4;
        }

        public static string RandomString(this int length, bool useSpecial = false)
        {
            return useSpecial
                    ? new string(Enumerable.Repeat(_charsWithSpecial, length).Select(f => f[OtherHelper.Rnd.Next(f.Length)]).ToArray())
                    : new string(Enumerable.Repeat(_chars, length).Select(f => f[OtherHelper.Rnd.Next(f.Length)]).ToArray());
        }

        public static string Hs256Signature()
        {
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[256 / 8];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static PasswordStrength CheckPasswordComplexity(string password)
        {
            int score = 0;
            int minimumLength = 12;
            int repeatCharPenalty = 2;
            int characterBonus = 5;
            // Length Bonus
            if (password.Length > minimumLength)
            {
                score += password.Length - minimumLength;
            }

            // Character Class Bonus
            bool hasUpper = password.Any(char.IsUpper);
            if (hasUpper) score += characterBonus;
            bool hasLower = password.Any(char.IsLower);
            if (hasLower) score += characterBonus;
            bool hasNumber = password.Any(char.IsDigit);
            if (hasNumber) score += characterBonus;
            bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));
            if (hasSymbol) score += characterBonus;

            // Penalty for Repeated Characters
            int consecutiveRepeat = 1;
            for (int i = 1; i < password.Length; i++)
            {
                if (password[i] == password[i - 1])
                {
                    consecutiveRepeat++;
                }
                else
                {
                    score -= Math.Min(consecutiveRepeat - 1, repeatCharPenalty); // Apply penalty only for repeats exceeding 1
                    consecutiveRepeat = 1;
                }
            }
            score -= Math.Min(consecutiveRepeat - 1, repeatCharPenalty); // Apply penalty for repeats at the end of the string

            // Define Strength Levels based on score
            if (score < 15)
            {
                return PasswordStrength.Weak;
            }
            else if (score < 20)
            {
                return PasswordStrength.Medium;
            }
            else if (score < 30)
            {
                return PasswordStrength.Strong;
            }
            else
            {
                return PasswordStrength.VeryStrong;
            }
        }

        public static bool IsNumberOnly(this string inputString)
        {
            return Regex.IsMatch(inputString, _regexOnlyNumberPattern);
        }

        private static string GetNextDoubleBySource(this List<string> suffix, List<string> source)
        {
            var orderedListCurrentUsername = suffix.OrderByDescending(f => f).FirstOrDefault();
            var rs = string.Empty;
            if (string.IsNullOrEmpty(orderedListCurrentUsername)) rs = "00";
            else
            {
                var firstElement = orderedListCurrentUsername.FirstOrDefault().ToString();
                var lastElement = orderedListCurrentUsername.LastOrDefault().ToString();

                var firstIdx = 0;
                var lastIdx = 0;
                var idx = 0;
                foreach (var item in source)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    if (item.Equals(firstElement)) firstIdx = idx;
                    if (item.Equals(lastElement)) lastIdx = idx;
                    idx++;
                }

                if (lastIdx == source.Count - 1)
                {
                    if (firstIdx == source.Count - 1)
                    {
                        for (int i = 0; i < source.Count - 1; i++)
                        {
                            for (int j = 0; j < source.Count - 1; j++)
                            {
                                lastIdx--;
                                var degradeNumb = string.Format("{0}{1}", firstIdx, lastIdx);
                                if (!suffix.Contains(degradeNumb)) return degradeNumb;
                            }
                            firstIdx--;
                        }

                        rs = "00";
                    }
                    else
                    {
                        rs = string.Format("{0}0", source[++firstIdx]);
                    }
                }
                else
                {
                    rs = string.Format("{0}{1}", source[firstIdx], source[++lastIdx]);
                }
            }
            return rs;
        }

        private static string GetNextTripleBySource(this List<string> suffix, List<string> source)
        {
            var orderedListCurrentUsername = suffix.OrderByDescending(f => f).FirstOrDefault();
            var rs = string.Empty;
            if (string.IsNullOrEmpty(orderedListCurrentUsername)) rs = "000";
            else
            {
                var splitElement = new List<string>();
                foreach (var item in orderedListCurrentUsername) splitElement.Add(item.ToString());
                if (splitElement.Count != 3) rs = "000";
                else
                {
                    var firstElement = splitElement[0];
                    var secondElement = splitElement[1];
                    var lastElement = splitElement[2];

                    var firstIdx = 0;
                    var secondIdx = 0;
                    var lastIdx = 0;
                    var idx = 0;
                    foreach (var item in source)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Equals(firstElement)) firstIdx = idx;
                        if (item.Equals(secondElement)) secondIdx = idx;
                        if (item.Equals(lastElement)) lastIdx = idx;
                        idx++;
                    }

                    if (lastIdx == source.Count - 1)
                    {
                        if (secondIdx == source.Count - 1)
                        {
                            if (firstIdx == source.Count - 1) rs = "000";
                            else rs = string.Format("{0}{1}{2}", source[++firstIdx], source[secondIdx], source[lastIdx]);
                        }
                        else
                        {
                            rs = string.Format("{0}{1}{2}", source[firstIdx], source[++secondIdx], source[lastIdx]);
                        }
                    }
                    else
                    {
                        rs = string.Format("{0}{1}{2}", source[firstIdx], source[secondIdx], source[++lastIdx]);
                    }
                }
            }
            return rs;
        }

        public static string GetNextTripleCharacters(this List<string> suffix)
        {
            return suffix.GetNextTripleBySource(_characters);
        }

        public static string GetNextDoubleCharacters(this List<string> suffix)
        {
            return suffix.GetNextDoubleBySource(_characters);
        }

        public static string GetNextDoubleNumerics(this List<string> suffix)
        {
            return suffix.GetNextDoubleBySource(_onlyNumerics);
        }

        public static bool IsValidJson(this string jsonString)
        {
            try
            {
                JObject.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException ex)
            {
                // Optional: Log or handle parsing error
                return false;
            }
        }
    }
}
