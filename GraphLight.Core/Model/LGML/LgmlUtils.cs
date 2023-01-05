using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GraphLight.Model.LGML
{
    public static class LgmlUtils
    {
        private static XmlReaderSettings? _readerSettings;

        public static Graph LoadLgmlGraph(Stream stream)
        {
            var type = typeof(Graph);
            var serializer = new XmlSerializer(type);

            using var reader = XmlReader.Create(stream, GetReaderSettings("GraphLight.Model.LGML.xsd"));
            return (Graph)serializer.Deserialize(reader);
        }

        public static void Save(Graph graph, Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Graph));
            serializer.Serialize(stream, graph);
        }

        private static XmlReaderSettings GetReaderSettings(string resourceName)
        {
            var errors = new List<Exception>();

            if (_readerSettings != null)
                return _readerSettings;

            var type = typeof(Graph);
            using var xsdStream = type.Assembly.GetManifestResourceStream(resourceName);
            if (xsdStream == null)
                throw new MissingManifestResourceException($"Embedded resource not found: {resourceName}");
            var schema = XmlSchema.Read(xsdStream, (s, e) => errors.Add(e.Exception));
            if (errors.Any())
                throw new AggregateException(errors);
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);
            schemaSet.Compile();
            _readerSettings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints
                                  | XmlSchemaValidationFlags.ReportValidationWarnings
            };

            return _readerSettings;
        }
    }
}
