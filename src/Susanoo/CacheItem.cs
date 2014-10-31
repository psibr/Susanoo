using System;

namespace Susanoo
{
    /// <summary>
    /// Class CacheItem.
    /// </summary>
    public class CacheItem
    {
        private readonly object _item;

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public object Item
        {
            get
            {
                CallCount++;
                return _item;
            }
        }

        /// <summary>
        /// Gets the caching mode.
        /// </summary>
        /// <value>The caching mode.</value>
        public CacheMode CachingMode { get; private set; }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public double Interval { get; private set; }

        /// <summary>
        /// Gets the call count.
        /// </summary>
        /// <value>The call count.</value>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentNullException">item</exception>
        public CacheItem(object item, CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
            CachingMode = mode;
            Interval = interval != null ? interval.Value : 0d;

            TimeStamp = DateTime.Now;

            if (Interval <= 0)
                CachingMode = CacheMode.Permanent;
        }

        /// <summary>
        /// Renews the intervals.
        /// </summary>
        public void RenewIntervals()
        {
            CallCount = 0;
            TimeStamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Caching modes supported by Susanoo
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        /// No caching
        /// </summary>
        None = 0,

        /// <summary>
        /// Do not expire cached items
        /// </summary>
        Permanent,

        /// <summary>
        /// Expire the cached items every second interval
        /// </summary>
        TimeSpan,

        /// <summary>
        /// Expire the cached items every interval of requests for the data
        /// </summary>
        RepeatedRequestLimit
    }
}