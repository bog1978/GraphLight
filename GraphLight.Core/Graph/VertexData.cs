using System;

namespace GraphLight.Graph
{
    public class VertexData : IVertexData
    {
        public VertexData(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; }

        public bool Equals(IVertexData other) => other?.Id == Id;

        public int CompareTo(IVertexData other) => StringComparer.Ordinal.Compare(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IVertexData);

        public override string ToString() => Id;
    }
}