using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GraphLight.Graph;
using GraphLight.Layout;
using LgmlVertexShape = GraphLight.Model.LGML.VertexShape;
using GraphVertexShape = GraphLight.Graph.VertexShape;

namespace GraphLight.Model.LGML
{
    public static class LgmlUtils
    {
        private static XmlReaderSettings? _readerSettings;

        public static Graph LoadLgmlGraph(Stream stream)
        {
            var type = typeof(Graph);
            var serializer = new XmlSerializer(type);

            using var reader = XmlReader.Create(stream, GetReaderSettings("GraphLight.Model.LGML.LGML.xsd"));
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

        public static Graph ToLgmlGraph(this IGraph graph)
        {
            var vertexList = new List<Vertex>();
            var edgeList = new List<Edge>();
            var vertexCatList = new List<VertexCategory>();
            var edgeCatList = new List<EdgeCategory>();

            foreach (var vertex in graph.Vertices)
            {
                var v = new Vertex
                {
                    Id = vertex.Data.Id,
                    Label = vertex.Data.Label,
                    Shape = MapShape(vertex.Data.Shape),
                    Category = vertex.Data.Category,
                    Background = vertex.Data.Background,
                    Foreground = vertex.Data.Foreground,
                    Stroke = vertex.Data.Stroke,
                    StrokeThickness = vertex.Data.StrokeThickness,
                };
                vertexList.Add(v);
                if (vertex.Data.Category != null && !vertexCatList.Any(x => x.Id == vertex.Data.Category))
                    vertexCatList.Add(new VertexCategory
                    {
                        Id = vertex.Data.Category
                    });
            }

            foreach (var edge in graph.Edges)
            {
                var e = new Edge
                {
                    Src = edge.Src.Data.Id,
                    Dst = edge.Dst.Data.Id,
                    Category = edge.Data.Category,
                    Weight = edge.Weight,
                    Stroke = edge.Data.Stroke,
                    StrokeThickness = edge.Data.StrokeThickness,
                    Label = edge.Data.Label,
                };
                edgeList.Add(e);
                if (edge.Data.Category != null && !edgeCatList.Any(x => x.Id == edge.Data.Category))
                    edgeCatList.Add(new EdgeCategory
                    {
                        Id = edge.Data.Category
                    });
            }

            var g = new Graph
            {
                Vertex = vertexList.ToArray(),
                Edge = edgeList.ToArray(),
                VertexCategory = vertexCatList.ToArray(),
                EdgeCategory = edgeCatList.ToArray(),
            };

            return g;
        }

        public static IGraph FromLgmlGraph(this Graph graph)
        {
            var vMap = new Dictionary<string, VertexData>();

            var vCategoryMap = graph.VertexCategory?.ToDictionary(x => x.Id);
            var eCategoryMap = graph.EdgeCategory?.ToDictionary(x => x.Id);

            var g = new LayoutGraphModel();

            foreach (var vertex in graph.Vertex)
            {
                var data = new VertexData(vertex.Id);
                if (vertex.Category != null && vCategoryMap != null)
                {
                    var cat = vCategoryMap[vertex.Category];
                    data.Category = cat.Id;
                    if (cat.ShapeSpecified)
                        data.Shape = MapShape(cat.Shape);
                    if (cat.Background != null)
                        data.Background = cat.Background;
                    if (cat.Foreground != null)
                        data.Foreground = cat.Foreground;
                    if (cat.Stroke != null)
                        data.Stroke = cat.Stroke;
                    if (cat.StrokeThicknessSpecified)
                        data.StrokeThickness = cat.StrokeThickness;
                    if (cat.FontSizeSpecified)
                        data.FontSize = cat.FontSize;
                }

                data.Label = vertex.Label ?? vertex.Id;
                if (vertex.ShapeSpecified)
                    data.Shape = MapShape(vertex.Shape);
                if (vertex.Background != null)
                    data.Background = vertex.Background;
                if (vertex.Foreground != null)
                    data.Foreground = vertex.Foreground;
                if (vertex.Stroke != null)
                    data.Stroke = vertex.Stroke;
                if (vertex.StrokeThicknessSpecified)
                    data.StrokeThickness = vertex.StrokeThickness;
                if (vertex.FontSizeSpecified)
                    data.FontSize = vertex.FontSize;

                vMap.Add(vertex.Id, data);
                g.AddVertex(data);
            }

            foreach (var edge in graph.Edge)
            {
                var data = new EdgeData();
                var weight = 1.0;
                if (edge.Category != null && eCategoryMap != null)
                {
                    var cat = eCategoryMap[edge.Category];
                    data.Category = cat.Id;
                    if (cat.Stroke != null)
                        data.Stroke = cat.Stroke;
                    if (cat.StrokeThicknessSpecified)
                        data.StrokeThickness = cat.StrokeThickness;
                    if (cat.WeightSpecified)
                        weight = cat.Weight;
                    if (cat.FontSizeSpecified)
                        data.FontSize = cat.FontSize;
                }

                if (edge.WeightSpecified)
                    weight = edge.Weight;
                if (edge.Stroke != null)
                    data.Stroke = edge.Stroke;
                if (edge.StrokeThicknessSpecified)
                    data.StrokeThickness = edge.StrokeThickness;
                if (edge.FontSizeSpecified)
                    data.FontSize = edge.FontSize;

                var src = vMap[edge.Src];
                var dst = vMap[edge.Dst];
                var e = g.AddEdge(src, dst, data);
                e.Weight = weight;
            }

            return g;
        }

        private static LgmlVertexShape MapShape(GraphVertexShape shape) =>
            shape switch
            {
                GraphVertexShape.None => LgmlVertexShape.None,
                GraphVertexShape.Ellipse => LgmlVertexShape.Ellipse,
                GraphVertexShape.Rectangle => LgmlVertexShape.Rectangle,
                GraphVertexShape.Diamond => LgmlVertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static GraphVertexShape MapShape(LgmlVertexShape shape) =>
            shape switch
            {
                LgmlVertexShape.None => GraphVertexShape.None,
                LgmlVertexShape.Ellipse => GraphVertexShape.Ellipse,
                LgmlVertexShape.Rectangle => GraphVertexShape.Rectangle,
                LgmlVertexShape.Diamond => GraphVertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
