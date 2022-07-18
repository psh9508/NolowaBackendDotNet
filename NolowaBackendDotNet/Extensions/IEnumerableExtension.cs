using AutoMapper;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Extensions
{
    public static class IEnumerableExtension
    {
        public static void Foreach<T>(this IEnumerable<T> src, Action<T> action)
        {
            foreach (var item in src)
            {
                action(item);
            }
        }
    }
}
