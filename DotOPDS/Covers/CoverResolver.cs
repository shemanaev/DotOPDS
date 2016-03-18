using DotOPDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotOPDS.Covers
{
    public class CoverResolver
    {
        private static CoverResolver instance = new CoverResolver();
        public static CoverResolver Instance { get { return instance; } }

        private Dictionary<string, ICoverResolver> resolvers = new Dictionary<string, ICoverResolver>();

        private CoverResolver()
        {
            var type = typeof(ICoverResolver);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass);

            foreach (var t in types)
            {
                var resolver = (ICoverResolver)Activator.CreateInstance(t);
                resolvers.Add(resolver.Name, resolver);
            }
        }

        public string Get(Book book)
        {
            var type = Settings.Instance.Libraries[book.LibraryId].Covers;
            if (!resolvers.ContainsKey(type)) return null;
            var resolver = resolvers[type];
            return resolver.Get(book);
        }
    }
}
