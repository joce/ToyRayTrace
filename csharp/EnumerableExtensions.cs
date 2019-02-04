using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyRayTrace
{
    static class EnumerableExtensions
    {
        public static long Median(this IEnumerable<long> source)
        {
            // Create a copy of the input, and sort the copy
            var temp = source.OrderBy(v => v).ToArray();

            var count = temp.Length;
            if (count == 0)
                throw new InvalidOperationException("Empty collection");

            if (count % 2 == 0)
            {
                // count is even, average two middle elements
                var a = temp[count / 2 - 1];
                var b = temp[count / 2];
                return (a + b) / 2;
            }

            // count is odd, return the middle element
            return temp[count / 2];
        }
    }
}