using GraphLight.Geometry;

namespace GraphLight.Graph
{
    public interface IVertexDataLocation
    {
        bool IsTmp { get; set; }
        Rect2D Rect { get; }
    }
}