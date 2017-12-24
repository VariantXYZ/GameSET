﻿using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace GameSET.Core
{
    /// <summary>
    /// An object that represents the result of provided accumulated events.
    /// Contains several entities, and dictionary of statistic names and their handlers
    /// </summary>
    public class State
    {
        private static State currentState; // Maintain a reference to the current state so we can have C exports modify it
        private Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

        private OrderedDictionary statistics = new OrderedDictionary();

        public static void SetCurrentState(State st) => currentState = st;

        State(bool setCurrentState = true)
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
        public static void LogEventCurrent(string entityName, string csvData)
        {
            currentState.LogEvent(entityName, csvData);
        }

        [DllExport("AddStatistic", CallingConvention = CallingConvention.Cdecl)]
        public static void AddStatisticCurrent(string name, string alias, Func<object, object> statisticHandler)
        {
            currentState.AddStatistic(name, alias, statisticHandler);
        }

        public void LogEvent(string entityName, string csvData)
        {
            if (!entities.ContainsKey(entityName))
                entities[entityName] = new Entity();
            
        }

        public void AddStatistic(string name, string alias, Func<object, object> statisticHandler)
        {
            if (statistics.Contains(name))
                throw new Exception($"AddStatistic: {name} is already a known statistic for this state");
            statistics.Add(name, new Tuple<string, Func<object, object>>(alias, statisticHandler));
        }
    }
}