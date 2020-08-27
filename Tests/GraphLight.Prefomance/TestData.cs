using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphLight.Graph;
using GraphLight.Parser;

namespace GraphLight.Prefomance
{
    public static class TestData
    {
        public static IEnumerable<Lazy<Stream>> GraphStreams
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resources =
                    from resName in assembly.GetManifestResourceNames()
                    where resName.EndsWith(".graph")
                    let stream = new Lazy<Stream>(() => assembly.GetManifestResourceStream(resName))
                    select stream;
                return resources;
            }
        }

        private static IGraph _graphToTest;
        public static IGraph GraphToTest
        {
            get
            {
                if(_graphToTest==null)
                {
                    var lazy = GraphStreams.First();
                    using (lazy.Value)
                        _graphToTest = GraphHelper.ReadFromFile(lazy.Value);
                }
                return _graphToTest;
            }
        }
    }
}
