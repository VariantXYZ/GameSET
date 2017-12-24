using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameSET.Core
{
    public static class Helper
    {
        /// <summary>
        /// Generate a unique random string given excluded characters (excluded will override characters in included)
        /// Only really 'good enough'
        /// </summary>
        /// <returns></returns>
        #region Cache Results
        static Random GenerateRandomString_Random = new Random();
        static string GenerateRandomString_included = "";
        static string GenerateRandomString_excluded = "";
        static char[] GenerateRandomString_chars = new char[1];
        #endregion Cache Results
        public static string GenerateRandomString(string excluded = "", uint length = 8, string included = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var stringChar = new char[length];

            if (GenerateRandomString_excluded != excluded ||
                GenerateRandomString_included != included)
            {
                GenerateRandomString_chars = included.ToCharArray().Except(excluded.ToCharArray()).ToArray();
            }

            for (int i = 0; i < length; i++)
            {
                stringChar[i] = GenerateRandomString_chars[GenerateRandomString_Random.Next(GenerateRandomString_chars.Length)];
            }

            return new String(stringChar);
        }


        #region Cache Results
        static string ParseCSV_prevQuoteChar = "";
        //static string ParseCSV_prevDelimiter = "";
        static Regex ParseCSV_rgxQuoted;
        #endregion Cache Results
        public static List<string> ParseCSV(in string csv, string quoteChar = "\"", string delimiter = ",")
        {
            #region Preprocessing
            const string specialQuoteChars = @"\/*.[]>"; //Backslash has to be first

            if (ParseCSV_prevQuoteChar != quoteChar)
            {
                string escapedQuoteChar = quoteChar;

                foreach (char c in specialQuoteChars)
                    escapedQuoteChar = escapedQuoteChar.Replace(c.ToString(), $"\\{c}");

                ParseCSV_rgxQuoted = new Regex($"{escapedQuoteChar}(.*?){escapedQuoteChar}");
                ParseCSV_prevQuoteChar = quoteChar;
            }
            #endregion Preprocessing

            Dictionary<string, string> quoteCharMapping = new Dictionary<string, string>();
            MatchEvaluator m = (Match match) => { var guid = GenerateRandomString(quoteChar + delimiter); quoteCharMapping[guid] = match.Groups[1].Value; return guid; };
            string quotelessCsv = ParseCSV_rgxQuoted.Replace(csv, m);

            return quotelessCsv.Split(new string[] { delimiter }, StringSplitOptions.None)
                    .Select((s) => quoteCharMapping.ContainsKey(s) ? quoteCharMapping[s] : s)
                    .ToList();
        }
    }
}
