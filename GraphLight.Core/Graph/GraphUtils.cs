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
                    Shape = vertex.Data.Shape.Map(),
                    ShapeSpecified = true,
                    StrokeStyle = vertex.Data.StrokeStyle.Map(),
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
                    StrokeStyle = Map(edge.Data.StrokeStyle),
                    StrokeStyleSpecified = true,
                }).ToArray(),
            };

        private static IGraph FromLgmlGraph(this LgmlGraph lgmlGraph)
        {
            var vMap = new Dictionary<string, VertexData>();

            var vertexStyles = lgmlGraph.VertexCategory?.ToDictionary(x => x.Id, x => x as LgmlVertexStyle);
            var edgeStyles = lgmlGraph.EdgeCategory?.ToDictionary(x => x.Id, x => x as LgmlEdgeStyle);

            var g = new LayoutGraphModel();

            foreach (var vertex in lgmlGraph.Vertex)
            {
                var style = vertex.Category != null && vertexStyles != null
                    ? vertexStyles[vertex.Category]
                    : null;

                var data = new VertexData(vertex.Id, vertex.Label, vertex.Category)
                    .ApplyVertexStyle(style)
                    .ApplyVertexStyle(vertex);

                vMap.Add(vertex.Id, data);
                g.AddVertex(data);
            }

            foreach (var edge in lgmlGraph.Edge)
            {
                var data = new EdgeData(edge.Label, edge.Category);
                var weight = 1.0;
                if (edge.Category != null && edgeStyles != null)
                {
                    var style = edgeStyles[edge.Category];
                    data.ApplyEdgeStyle(style);
                    if (style.WeightSpecified)
                        weight = style.Weight;
                }

                data.ApplyEdgeStyle(edge);
                if (edge.WeightSpecified)
                    weight = edge.Weight;

                var src = vMap[edge.Src];
                var dst = vMap[edge.Dst];
                var e = g.AddEdge(src, dst, data);
                e.Weight = weight;
            }

            return g;
        }

        private static T ApplyVertexStyle<T>(this T data, LgmlVertexStyle? style)
        where T : IVertexData
        {
            if (style == null)
                return data;
            if (style.ShapeSpecified)
                data.Shape = style.Shape.Map();
            if (style.MarginSpecified)
                data.Margin = style.Margin;
            return data.ApplyBaseStyle(style);
        }

        private static T ApplyEdgeStyle<T>(this T data, LgmlEdgeStyle? style)
            where T : IEdgeData
        {
            if (style == null)
                return data;
            //if (style.WeightSpecified)
            //    data.Weight = style.Weight);
            return data.ApplyBaseStyle(style);
        }

        private static T ApplyBaseStyle<T>(this T data, LgmlBaseStyle? style)
            where T : ICommonData
        {
            if (style == null)
                return data;

            if (style.Background != null)
                data.Background = style.Background;
            if (style.Foreground != null)
                data.Foreground = style.Foreground;

            if (style.Stroke != null)
                data.Stroke = style.Stroke;
            if (style.StrokeStyleSpecified)
                data.StrokeStyle = style.StrokeStyle.Map();
            if (style.StrokeThicknessSpecified)
                data.StrokeThickness = style.StrokeThickness;

            if (style.FontSizeSpecified)
                data.FontSize = style.FontSize;
            if (style.FontStyleSpecified)
                data.FontStyle = style.FontStyle.Map();
            if (style.FontWeightSpecified)
                data.FontWeight = style.FontWeight.Map();

            if (style.TextAlignmentSpecified)
                data.TextAlignment = style.TextAlignment.Map();
            if (style.TextWrappingSpecified)
                data.TextWrapping = style.TextWrapping.Map();

            return data;
        }

        private static TextWrapping Map(this LgmlTextWrapping textWrapping) =>
            textWrapping switch
            {
                LgmlTextWrapping.NoWrap => TextWrapping.NoWrap,
                LgmlTextWrapping.Wrap => TextWrapping.Wrap,
                _ => throw new ArgumentOutOfRangeException(nameof(textWrapping), textWrapping, null)
            };

        private static TextAlignment Map(this LgmlTextAlignment textAlignment) =>
            textAlignment switch
            {
                LgmlTextAlignment.Center => TextAlignment.Center,
                LgmlTextAlignment.Left => TextAlignment.Left,
                LgmlTextAlignment.Right => TextAlignment.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(textAlignment), textAlignment, null)
            };

        private static FontWeight Map(this LgmlFontWeight fontWeight) =>
            fontWeight switch
            {
                LgmlFontWeight.Normal => FontWeight.Normal,
                LgmlFontWeight.Bold => FontWeight.Bold,
                _ => throw new ArgumentOutOfRangeException(nameof(fontWeight), fontWeight, null)
            };

        private static FontStyle Map(this LgmlFontStyle fontStyle) =>
            fontStyle switch
            {
                LgmlFontStyle.Normal => FontStyle.Normal,
                LgmlFontStyle.Italic => FontStyle.Italic,
                _ => throw new ArgumentOutOfRangeException(nameof(fontStyle), fontStyle, null)
            };

        private static VertexShape Map(this LgmlVertexShape shape) =>
            shape switch
            {
                LgmlVertexShape.None => VertexShape.None,
                LgmlVertexShape.Ellipse => VertexShape.Ellipse,
                LgmlVertexShape.Rectangle => VertexShape.Rectangle,
                LgmlVertexShape.Diamond => VertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static StrokeStyle Map(this LgmlStrokeStyle style) =>
            style switch
            {
                LgmlStrokeStyle.Solid => StrokeStyle.Solid,
                LgmlStrokeStyle.Dash => StrokeStyle.Dash,
                LgmlStrokeStyle.DashDot => StrokeStyle.DashDot,
                LgmlStrokeStyle.Dot => StrokeStyle.Dot,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static LgmlVertexShape Map(this VertexShape shape) =>
            shape switch
            {
                VertexShape.None => LgmlVertexShape.None,
                VertexShape.Ellipse => LgmlVertexShape.Ellipse,
                VertexShape.Rectangle => LgmlVertexShape.Rectangle,
                VertexShape.Diamond => LgmlVertexShape.Diamond,
                _ => throw new ArgumentOutOfRangeException()
            };

        private static LgmlStrokeStyle Map(this StrokeStyle style) =>
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