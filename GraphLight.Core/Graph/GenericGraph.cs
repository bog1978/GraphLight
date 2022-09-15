using System;

namespace GraphLight.Graph
{
    public class GenericGraph<V, E> : BaseGraph<V, E>
    {
        public GenericGraph() { }

        protected override IEdge<V, E> CreateEdge(E data) => new BaseEdge<V, E>(data);

        protected override IVertex<V, E> CreateVertex(V data) => new BaseVertex<V, E>(data);

        public override V CreateVertexData(object id) => throw new NotImplementedException("Необходимо реализовать метод CreateVertexData.");

        public override E CreateEdgeData() => throw new NotImplementedException("Необходимо реализовать метод CreateEdgeData.");
    }
}