using System;
using GameSET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameSET.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void StateTest()
        {
            State state = new State();

            Func<string, dynamic, dynamic, dynamic> funcUpdateTimestamp =
                (entityName, oldObj, newObj) => { state.LogEventSingle(entityName, "Time", (newObj - oldObj) / 1000); return newObj; };

            Func<string, dynamic, dynamic, dynamic> funcUpdateTime =
                (entityName, oldObj, newObj) =>
                {
                    if (oldObj + newObj != 0)
                        state.LogEventSingle(entityName, "DPS", state.GetStatistic(entityName, "Damage") / (oldObj + newObj));
                    return oldObj + newObj;
                };

            Func<string, dynamic, dynamic, dynamic> funcUpdateDamage =
                (entityName, oldObj, newObj) =>
                {
                    dynamic t = state.GetStatistic(entityName, "Time");
                    if (t != 0)
                        state.LogEventSingle(entityName, "DPS", (oldObj + newObj) / t);
                    return oldObj + newObj;
                };

            Func<string, dynamic, dynamic, dynamic> funcReplace =
                (entityName, oldObj, newObj) => newObj;

            string entityName1 = "Test1";
            string csvHeader1 = "Timestamp,Damage";

            //5 states
            for (int j = 0; j < 5; j++)
            {
                state.AddStatistic("Timestamp", "Timestamp", 0, "System.UInt32", funcUpdateTimestamp);
                state.AddStatistic("Damage", "Damage", 0, "System.UInt32", funcUpdateDamage);
                state.AddStatistic("Time", "Total Time (s)", 0, "System.UInt32", funcUpdateTime);
                state.AddStatistic("DPS", "DPS", 0, "System.UInt32", funcReplace);

                //10000 logs
                for (int i = 1; i < 10000; i++)
                {
                    string csvData1 = $"{1000 * i},100";
                    State.LogEventMultiCurrent(entityName1, csvHeader1, csvData1);
                    Assert.IsTrue((uint)State.GetStatisticCurrent(entityName1, "Timestamp") == 1000 * i);
                    Assert.IsTrue((uint)State.GetStatisticCurrent(entityName1, "Damage") == 100 * i);
                    Assert.IsTrue((uint)State.GetStatisticCurrent(entityName1, "Time") == 1 * i);
                    Assert.IsTrue((uint)State.GetStatisticCurrent(entityName1, "DPS") == 100);
                }
                state = new State();
            }
        }
    }
}
