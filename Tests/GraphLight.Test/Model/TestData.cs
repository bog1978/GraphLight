using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GraphLight.Model
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
    }
}
