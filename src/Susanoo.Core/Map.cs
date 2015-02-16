using System.Collections.Generic;

namespace Susanoo
{
    /// <summary>
    /// A simple bidirectional Dictionary that allows efficient lookup on either key or value.
    /// </summary>
    /// <typeparam name="T1">The type of the t1.</typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    internal class Map<T1, T2>
    {
        private readonly IDictionary<T1, T2> _forward;
        private readonly IDictionary<T2, T1> _reverse;

        public Map()
            : this(new Dictionary<T1, T2>(), new Dictionary<T2, T1>()) { }

        public Map(IDictionary<T1, T2> one, IDictionary<T2, T1> two)
        {
            this._forward = one;
            this._reverse = two;

            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private readonly IDictionary<T3, T4> _dictionary;
            public Indexer(IDictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            public bool TryGetValue(T3 key, out T4 value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            public Dictionary<T3, T4> ToDictionary()
            {
                return new Dictionary<T3, T4>(_dictionary);
            }

            public int Count
            {
                get { return _dictionary.Count; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Add(T2 t1, T1 t2)
        {
            _forward.Add(t2, t1);
            _reverse.Add(t1, t2);
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return Forward.TryGetValue(key, out value);
        }

        public bool TryGetValue(T2 key, out T1 value)
        {
            return Reverse.TryGetValue(key, out value);
        }

        public Indexer<T1, T2> Forward { get; set; }
        public Indexer<T2, T1> Reverse { get; set; }
    }
}
