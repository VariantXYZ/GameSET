using System.Collections;
using System.Collections.Generic;

namespace GameSET.Core
{
    /// <summary>
    /// An object to define an 'Entity'
    /// A dictionary of statistic names to values
    /// </summary>
    internal class Entity
    {
        private Dictionary<string, object> stats = new Dictionary<string, object>();

        public Dictionary<string, object> Stats { get => stats; set => stats = value; }
    }
}
