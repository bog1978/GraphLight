using System;
using System.Collections.Generic;
using System.Linq;
using GraphLight.Graph;
using GraphLight.Layout;
using LgmlVertexShape = GraphLight.Model.LGML.VertexShape;
using GraphVertexShape = GraphLight.Graph.VertexShape;

namespace GraphLight.Model.LGML
{
    public static class LgmlConverter
    {
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
                    data.Shape = MapShape(cat.Shape);
                }

                data.Shape = MapShape(vertex.Shape);
                data.Label = vertex.Label;
                vMap.Add(vertex.Id, data);
                g.AddVertex(data);
            }

            foreach (var edge in graph.Edge)
            {
                var data = new EdgeData();
                if (edge.Category != null && eCategoryMap != null)
                {
                    var cat = eCategoryMap[edge.Category];
                    data.Category = cat.Id;
                }

                var src = vMap[edge.Src];
                var dst = vMap[edge.Dst];
                g.AddEdge(src, dst, data);
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