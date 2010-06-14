using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensionMethods
    {
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var t in source ?? Enumerable.Empty<T>())
            {
                action(t);
            }
        }

        public static T Inject<T>(this IEnumerable<T> source, T seed, Func<T, T, T> block)
        {
            T first = seed;
            foreach (var t in source)
            {
                first = block(first, t);
            }
            return first;
        }

        public static T Inject<T>(this IEnumerable<T> source, Func<T, T, T> block)
        {
            return source.Skip(1).Inject(source.First(), block);
        }

        /// <summary>
        /// Returns collection without nulls. If source is null, returns an empty enumerable
        /// </summary>
        /// <typeparam name="T">Type of enumerable</typeparam>
        /// <param name="source">Source collection</param>
        /// <returns>Empty enumerable if source is null, otherwise, collection without nulls</returns>
        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> source)
        {
            return source == null ? Enumerable.Empty<T>() : source.Where(item => item != null);
        }
    }
}