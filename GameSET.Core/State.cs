using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameSET.Core
{
    /// <summary>
    /// An object that represents the result of provided accumulated events.
    /// Contains several entities, and dictionary of statistic names and their handlers
    /// </summary>
    public class State
    {
        private struct Statistic
        {
            public readonly string Name; 
            public readonly string Alias;
            public readonly dynamic DefaultValue;
            public readonly Type Type;
            public readonly Func<string, dynamic, dynamic, dynamic> StatisticHandler;

            public Statistic(string name, string alias, dynamic defaultValue, string type, Func<string, dynamic, dynamic, dynamic> func)
            {
                Name = name;
                Alias = alias;
                DefaultValue = defaultValue;
                Type = Type.GetType(type);
                StatisticHandler = func;
            }
        }

        private static State currentState; // Maintain a reference to the current state so we can have C exports modify it
        private Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

        private Dictionary<string, Statistic> statistics = new Dictionary<string, Statistic>();

        public static void SetCurrentState(State st) => currentState = st;

        public State(bool setCurrentState = true)
        {
            if (setCurrentState)
            {
                SetCurrentState(this);
            }
        }

        ~State()
        {
        }

        [DllExport("LogEvent", CallingConvention = CallingConvention.Cdecl)]
        public static void LogEventSingleCurrent(string entityName, string statisticName, string statisticValue)
        {
            currentState.LogEventSingle(entityName, statisticName, statisticValue);
        }

        [DllExport("LogEventMulti", CallingConvention = CallingConvention.Cdecl)]
        public static void LogEventMultiCurrent(string entityName, string csvHeader, string csvData)
        {
            currentState.LogEventMulti(entityName, csvHeader, csvData);
        }

        [DllExport("AddStatistic", CallingConvention = CallingConvention.Cdecl)]
        public static void AddStatisticCurrent(string name, string alias, object defaultValue, string type, Func<string, dynamic, dynamic, dynamic> statisticHandler)
        {
            currentState.AddStatistic(name, alias, defaultValue, type, statisticHandler);
        }

        [DllExport("GetStatistic", CallingConvention = CallingConvention.Cdecl)]
        public static dynamic GetStatisticCurrent(string entityName, string statisticName)
        {
            return currentState.GetStatistic(entityName, statisticName);
        }

        public dynamic GetStatistic(string entityName, string statisticName)
        {
            return entities[entityName].Stats.ContainsKey(statisticName) ? Convert.ChangeType(entities[entityName].Stats[statisticName], statistics[statisticName].Type) : statistics[statisticName].DefaultValue;
        }

        public void LogEventSingle(string entityName, string statisticName, dynamic statisticValue)
        {
            if (!entities.ContainsKey(entityName))
                entities[entityName] = new Entity();

            if (!entities[entityName].Stats.ContainsKey(statisticName))
                entities[entityName].Stats[statisticName] = statistics[statisticName].DefaultValue;

            dynamic value = Convert.ChangeType(statisticValue, statistics[statisticName].Type);

            entities[entityName].Stats[statisticName] = statistics[statisticName]
                .StatisticHandler(entityName, entities[entityName].Stats[statisticName], value);
        }

        public void LogEventMulti(string entityName, string csvHeader, string csvData)
        {
            if (!entities.ContainsKey(entityName))
                entities[entityName] = new Entity();

            var header = Helper.ParseCSV(csvHeader);
            var stats = Helper.ParseCSV(csvData);

            for(int i = 0; i < header.Count && i < stats.Count(); i++)
            {
                LogEventSingle(entityName, header[i], stats[i]);
            }
        }

        public void AddStatistic(string name, string alias, object defaultValue, string type, Func<string, dynamic, dynamic, dynamic> statisticHandler)
        {
            if (statistics.ContainsKey(name))
                throw new Exception($"AddStatistic: {name} is already a known statistic for this state");

            statistics.Add(name, new Statistic(name, alias, defaultValue, type, statisticHandler));
        }
    }
}
