using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class ICollectionExtension
    {
        public static void AddRnage<T>(this ICollection<T> src, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                src.Add(item);
            }
        }    
    }
}
