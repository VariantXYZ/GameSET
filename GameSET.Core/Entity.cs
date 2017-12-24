using System.Collections.Specialized;

namespace GameSET.Core
{
    /// <summary>
    /// An object to define an 'Entity'
    /// A dictionary of statistic names to values
    /// </summary>
    internal class Entity
    {
        private OrderedDictionary stats = new OrderedDictionary();

        public OrderedDictionary Stats { get => stats; set => stats = value; }
    }
}
