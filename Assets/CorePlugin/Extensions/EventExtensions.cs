using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CorePlugin.Extensions
{
    public static class EventExtensions
    {
        public static T Combine<T>(this Delegate[] subscribers) where  T : Delegate
        {
            return subscribers.OfType<T>().Aggregate<T, T>(null, (current, dDelegate) => (T) Delegate.Combine(current, dDelegate));
        }
    }
}
