using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GraphLight.Layout;
using GraphLight.Model.LGML;

namespace GraphLight.Graph
{
    public static class GraphUtils
    {
        private static XmlReaderSettings? _readerSettings;

        public static IGraph LoadLgml(Stream stream)
        {
            var type = typeof(LgmlGraph);
            var serializer = new XmlSerializer(type);

            using var reader = XmlReader.Create(stream, GetReaderSettings("LGML.xsd"));
            var graph = (LgmlGraph)serializer.Deserialize(reader);
            return graph.FromLgmlGraph();
        }

        public static void SaveLgml(IGraph graph, Stream stream)
        {
            var lgmlGraph = graph.ToLgmlGraph();
            var serializer = new XmlSerializer(typeof(LgmlGraph));
            serializer.Serialize(stream, lgmlGraph);
        }

        private static XmlReaderSettings GetReaderSettings(string resourceName)
        {
            if (_readerSettings != null)
                return _readerSettings;

            using var xsdStream = FindResourceStream(resourceName);
            if (xsdStream == null)
                throw new MissingManifestResourceException($"Embedded resource not found: {resourceName}");

            var errors = new List<Exception>();
            var schema = XmlSchema.Read(xsdStream, (s, e) => errors.Add(e.Exception));
            if (errors.Any())
                throw new AggregateException(errors);

            var schemaSet = new XmlSchemaSet();
            _ = schemaSet.Add(schema);
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

        private static Stream? FindResourceStream(string fileName)
        {
            var type = typeof(LgmlGraph);
            var resourceNames = type.Assembly.GetManifestResourceNames();
            var resourceName = resourceNames.FirstOrDefault(x => x.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
            return resourceName != null
                ? type.Assembly.GetManifestResourceStream(resourceName)
                : null;
        }

        private static LgmlGraph ToLgmlGraph(this IGraph graph) =>
            new LgmlGraph
            {
                Vertex = graph.Vertices.Select(vertex => new LgmlVertex
                {
                    Id = vertex.Data.Id,
                    Label = vertex.Data.Label,
                    //Category = vertex.Data.Category,
                    Background = vertex.Data.Background,
                    Foreground = vertex.Data.Foreground,
                    Stroke = vertex.Data.Stroke,
                    StrokeThickness = vertex.Data.StrokeThickness,
                    StrokeThicknessSpecified = true,
                    FontSize = vertex.Data.FontSize,
                    FontSizeSpecified = true,
                    Margin = vertex.Data.Margin,
                    MarginSpecified = true,
                    Shape = MapVertexShape(vertex.Data.Shape),
                    ShapeSpecified = true,
                    StrokeStyle = MapEdgeStyle(vertex.Data.Style),
                    StrokeStyleSpecified = true,
                }).ToArray(),
                Edge = graph.Edges.Select(edge => new LgmlEdge
                {
                    Label = edge.Data.Label,
                    Src = edge.Src.Data.Id,
                    Dst = edge.Dst.Data.Id,
                    //Category = edge.Data.Category,
                    Stroke = edge.Data.Stroke,
                    StrokeThickness = edge.Data.StrokeThickness,
                    StrokeThicknessSpecified = true,
                    FontSize = edge.Data.FontSize,
                    FontSizeSpecified = true,
                    Weight = edge.Weight,
                    WeightSpecified = true,
                    StrokeStyle = MapEdgeStyle(edge.Data.Style),
                    StrokeStyleSpecified = true,
                }).ToArray(),
            };

        private static IGraph FromLgmlGraph(this LgmlGraph lgmlGraph)
        {
            var vMap = new Dictionary<string, VertexData>();

            var vCategoryMap = lgmlGraph.VertexCategory?.ToDictionary(x => x.Id);
            var eCategoryMap = lgmlGraph.EdgeCategory?.ToDictionary(x => x.Id);

            var g = new LayoutGraphModel();

            foreach (var vertex in lgmlGraph.Vertex)
            {
                var data = new VertexData(vertex.Id);
                if (vertex.Category != null && vCategoryMap != null)
                {
                    var cat = vCategoryMap[vertex.Category];
                    data.Category = cat.Id;
                    if (cat.ShapeSpecified)
                        data.Shape = MapVertexShape(cat.Shape);
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
                    if (cat.MarginSpecified)
                        data.Margin = cat.Margin;
                    if (cat.StrokeStyleSpecified)
                        data.Style = MapEdgeStyle(cat.StrokeStyle);
                }

                data.Label = vertex.Label ?? vertex.Id;
                if (vertex.ShapeSpecified)
                    data.Shape = MapVertexShape(vertex.Shape);
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
                if (vertex.MarginSpecified)
                    data.Margin = vertex.Margin;
                if (vertex.StrokeStyleSpecified)
                    data.Style = MapEdgeStyle(vertex.StrokeStyle);

                vMap.Add(vertex.Id, data);
                g.AddVertex(data);
            }

            foreach (var edge in lgmlGraph.Edge)
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
                    if (cat.StrokeStyleSpecified)
                        data.Style = MapEdgeStyle(cat.StrokeStyle);
                    if (cat.Background != null)
                        data.Background = cat.Background;
                    if (cat.Foreground != null)
                        data.Foreground = cat.Foreground;
                }

                if (edge.WeightSpecified)
                    weight = edge.Weight;
                if (edge.Label != null)
                    data.Label = edge.Label;
                if (edge.Stroke != null)
                    data.Stroke = edge.Stroke;
                if (edge.StrokeThicknessSpecified)
                    data.StrokeThickness = edge.StrokeThickness;
                if (edge.FontSizeSpecified)
                    data.FontSize = edge.FontSize;
                if (edge.StrokeStyleSpecified)
                    data.Style = MapEdgeStyle(edge.StrokeStyle);
                if (edge.Background != null)
                    data.Background = edge.Background;
                if (edge.Foreground != null)
                    data.Foreground = edge.Foreground;

                var src = vMap[edge.Src];
                var dst = vMap[edge.Dst];
                var e = g.AddEdge(src, dst, data);
                e.Weight = weight;
            }

            return g;
        }

        private static LgmlVertexShape MapVertexShape(VertexShape shape) =>
            shape switch
            {
                VertexShape.None => LgmlVertexShape.None,
                VertexShape.Ellipse => LgmlVertexShape.Ellipse,
                VertexShape.Rectangle => LgmlVertexShape.Rectangle,
                VertexShape.Diamond => LgmlVertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static VertexShape MapVertexShape(LgmlVertexShape shape) =>
            shape switch
            {
                LgmlVertexShape.None => VertexShape.None,
                LgmlVertexShape.Ellipse => VertexShape.Ellipse,
                LgmlVertexShape.Rectangle => VertexShape.Rectangle,
                LgmlVertexShape.Diamond => VertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static StrokeStyle MapEdgeStyle(LgmlStrokeStyle style) =>
            style switch
            {
                LgmlStrokeStyle.Solid => StrokeStyle.Solid,
                LgmlStrokeStyle.Dash => StrokeStyle.Dash,
                LgmlStrokeStyle.DashDot => StrokeStyle.DashDot,
                LgmlStrokeStyle.Dot => StrokeStyle.Dot,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static LgmlStrokeStyle MapEdgeStyle(StrokeStyle style) =>
            style switch
            {
                StrokeStyle.Solid => LgmlStrokeStyle.Solid,
                StrokeStyle.Dash => LgmlStrokeStyle.Dash,
                StrokeStyle.DashDot => LgmlStrokeStyle.DashDot,
                StrokeStyle.Dot => LgmlStrokeStyle.Dot,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}