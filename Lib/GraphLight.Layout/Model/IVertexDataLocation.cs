using GraphLight.Geometry;

namespace GraphLight.Model
{
    public interface IVertexDataLocation
    {
        bool IsTmp { get; set; }
        Rect2D Rect { get; }
    }
}