using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using GameSET.Core;
using System.Collections.Generic;

namespace GameSET.Tests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void TestParseCSV()
        {
            List<Tuple<string, string, string>> testCsv = new List<Tuple<string, string, string>>();
            testCsv.Add(new Tuple<string, string, string>(@"asdf,12 34,0 x d e", "\"", ",")); //no quotes, comma delimited
            testCsv.Add(new Tuple<string, string, string>(@"""asdf"",""12 34"",""0 x d e""", "\"", ",")); // Single quotes, comma delimited
            testCsv.Add(new Tuple<string, string, string>(@"""""""asdf"""""",""""""12 34"""""",""""""0 x d e""""""", @"""""""", ",")); // Multi-quoted, comma delimited
            testCsv.Add(new Tuple<string, string, string>(@"***asdf***><***12 34***><***0 x d e***", @"***", "><")); // Multi-quoted, multi-delimited

            for (int i = 0; i < testCsv.Count; i++)
            {
                var csv = testCsv[i];
                List<string> tmp = Helper.ParseCSV(csv.Item1, csv.Item2, csv.Item3);
                Assert.IsTrue(tmp[0] == "asdf", $"Failed test {i}.0");
                Assert.IsTrue(tmp[1] == "12 34", $"Failed test {i}.1");
                Assert.IsTrue(tmp[2] == "0 x d e", $"Failed test {i}.2");
            }
        }
    }
}
