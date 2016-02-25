using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphLight;
using GraphLight.Algorithm;
using GraphLight.Geometry;
using GraphLight.Collections;

namespace SplitPolygon
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private readonly ViewModel _vm = new ViewModel();
        public Window1()
        {
            DataContext = _vm;
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            _vm.Polygon.Points.Add(new Point2D(position));
            refresh();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
            _vm.Polygon.Points.Clear();
            _vm.SplitCount = 0;
        }

        private void btnSimplify_Click(object sender, RoutedEventArgs e)
        {
            _vm.Simplify();
            refresh();
        }

        private void refresh()
        {
            clear();
            foreach (var p in _vm.Polygon.Points)
                polygon.Points.Add(p);

            _vm.CalcConvexTypes();
            var grpConcave = new GeometryGroup();
            var grpConvex = new GeometryGroup();
            foreach (var p in _vm.Polygon.Points)
            {
                var ell = new EllipseGeometry(p, 5, 5);
                if (p.Sgn >= 0)
                    grpConvex.Children.Add(ell);
                else
                    grpConcave.Children.Add(ell);
            }
            convexPoints.Data = grpConvex;
            concavePoints.Data = grpConcave;
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            Point2D p;
            var pts = _vm.Split(out p);
            var grpSplit = new GeometryGroup();
            foreach (var q in pts)
            {
                var line = new LineGeometry(p, q);
                grpSplit.Children.Add(line);
            }
            splitLines.Data = grpSplit;
        }

        private void btnConvexLines_Click(object sender, RoutedEventArgs e)
        {
            var shortestPath = _vm.GetPiecewiseCurve();
            
            var approx = new Approximation();
            shortestPath.Iter(approx.AddPoint);

            var points = new PointCollection(shortestPath.Select(x => (Point)x));
            var points2 = new PointCollection(approx.GeneratePoints(100).Select(x => (Point)x));

            shortestPathLines.Points = points;
            shortestPathLines2.Points = points2;
        }

        private void clear()
        {
            polygon.Points.Clear();
            concavePoints.Data = null;
            convexPoints.Data = null;
            splitLines.Data = null;
            pathLines.Data = null;
            shortestPathLines.Points = null;
            shortestPathLines2.Points = null;
            _vm.ConvexIndex = 1;
        }
    }
}
