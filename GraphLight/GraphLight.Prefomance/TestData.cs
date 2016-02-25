using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphLight.Graph;

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

        private static DrawingGraph _graphToTest;
        public static DrawingGraph GraphToTest
        {
            get
            {
                if(_graphToTest==null)
                {
                    var lazy = GraphStreams.First();
                    using (lazy.Value)
                        _graphToTest = DrawingGraph.ReadFromFile(lazy.Value);
                }
                return _graphToTest;
            }
        }
    }
}
