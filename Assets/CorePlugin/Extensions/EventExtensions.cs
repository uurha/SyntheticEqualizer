﻿using System;
using System.Linq;

namespace CorePlugin.Extensions
{
    public static class EventExtensions
    {
        [Obsolete("Use <i>EventExtensions.Subscribe</i> or <i>EventExtensions.Unsubscribe</i>", true)]
        public static T Combine<T>(this Delegate[] subscribers) where T : Delegate
        {
            return subscribers.OfType<T>()
                              .Aggregate<T, T>(null, (current, dDelegate) => (T) Delegate.Combine(current, dDelegate));
        }

        /// <summary>
        /// Aggregating delegates
        /// </summary>
        /// <param name="dDelegate"></param>
        /// <param name="aggregateMethod"></param>
        /// <param name="delegates"></param>
        /// <typeparam name="T"></typeparam>
        public static void Aggregate<T>(ref T dDelegate, Func<Delegate, Delegate, Delegate> aggregateMethod, params Delegate[] delegates) where T : Delegate
        {
            dDelegate = delegates.OfType<T>().Aggregate(dDelegate,
                                                        (current, delegateItem) =>
                                                            (T) aggregateMethod.Invoke(current, delegateItem));
        }

        /// <summary>
        /// Subscribing delegates
        /// </summary>
        /// <param name="dDelegate"></param>
        /// <param name="delegates"></param>
        /// <typeparam name="T"></typeparam>
        public static void Subscribe<T>(ref T dDelegate, params Delegate[] delegates) where T : Delegate
        {
            Aggregate(ref dDelegate, Delegate.Combine, delegates);
        }

        /// <summary>
        /// Unsubscribes delegates
        /// </summary>
        /// <param name="dDelegate"></param>
        /// <param name="delegates"></param>
        /// <typeparam name="T"></typeparam>
        public static void Unsubscribe<T>(ref T dDelegate, params Delegate[] delegates) where T : Delegate
        {
            Aggregate(ref dDelegate, Delegate.Remove, delegates);
        }

        /// <summary>
        /// Unsubscribes all delegates
        /// </summary>
        /// <param name="dDelegate"></param>
        /// <param name="delegates"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnsubscribeAll<T>(ref T dDelegate, params Delegate[] delegates) where T : Delegate
        {
            Aggregate(ref dDelegate, Delegate.RemoveAll, delegates);
        }
    }
}
