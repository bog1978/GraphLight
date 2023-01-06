using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GraphLight.Drawing
{
    public class TextOnPathElement : FrameworkElement
    {
        #region dependency properties

        #region FontFamily

        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            nameof(FontFamily), typeof(FontFamily), typeof(TextOnPathElement), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily));

        #endregion

        #region FontStyle

        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            nameof(FontStyle), typeof(FontStyle), typeof(TextOnPathElement));

        #endregion

        #region FontWeight

        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            nameof(FontWeight), typeof(FontWeight), typeof(TextOnPathElement));

        #endregion

        #region FontStretch

        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }

        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register(
            nameof(FontStretch), typeof(FontStretch), typeof(TextOnPathElement));

        #endregion

        #region FontSize

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            nameof(FontSize), typeof(double), typeof(TextOnPathElement));

        #endregion

        #region Foreground

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            nameof(Foreground), typeof(Brush), typeof(TextOnPathElement));

        #endregion

        #region Text

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(TextOnPathElement));

        #endregion

        #region PathFigure

        public PathFigure? PathFigure
        {
            get => (PathFigure?)GetValue(PathFigureProperty);
            set => SetValue(PathFigureProperty, value);
        }

        public static readonly DependencyProperty PathFigureProperty = DependencyProperty.Register(
            nameof(PathFigure), typeof(PathFigure), typeof(TextOnPathElement));

        #endregion

        #region ContentAlignment

        public HorizontalAlignment ContentAlignment
        {
            get => (HorizontalAlignment)GetValue(ContentAlignmentProperty);
            set => SetValue(ContentAlignmentProperty, value);
        }

        public static readonly DependencyProperty ContentAlignmentProperty = DependencyProperty.Register(
            nameof(ContentAlignment), typeof(HorizontalAlignment), typeof(TextOnPathElement));

        #endregion

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (string.IsNullOrEmpty(Text) || PathFigure == null)
                return;

            var pathLength = GetPathFigureLength(PathFigure);
            if (pathLength == 0)
                return;

            var textLength = 0.0;
            var formattedChars = new List<FormattedText>();
            var fontSize = ContentAlignment == HorizontalAlignment.Stretch ? 100 : FontSize;
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            foreach (var ch in Text)
            {
                var formattedText =
                    new FormattedText(ch.ToString(), CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, Foreground);
                formattedChars.Add(formattedText);
                textLength += formattedText.WidthIncludingTrailingWhitespace;
            }
            if (textLength == 0)
                return;

            var scalingFactor = ContentAlignment == HorizontalAlignment.Stretch ? pathLength / textLength : 1;

            var progress = 0.0;
            switch (ContentAlignment)
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                    progress = 0;
                    break;
                case HorizontalAlignment.Center:
                    progress = Math.Abs(pathLength - textLength) / 2 / pathLength;
                    break;
                case HorizontalAlignment.Right:
                    progress = Math.Abs(pathLength - textLength) / pathLength;
                    break;
            }

            var pathGeometry = new PathGeometry(new[] { PathFigure });

            foreach (var formText in formattedChars)
            {
                var width = scalingFactor * formText.WidthIncludingTrailingWhitespace;
                var baseline = scalingFactor * formText.Baseline;

                progress += width / 2 / pathLength;

                pathGeometry.GetPointAtFractionLength(progress, out var point, out var tangent);
                var angle = Math.Atan2(tangent.Y, tangent.X) * 180 / Math.PI;

                var matrix = new Matrix();
                matrix.Scale(scalingFactor, scalingFactor);
                matrix.RotateAt(angle, width / 2, baseline);
                matrix.Translate(point.X - width / 2, point.Y - baseline);

                progress += width / 2 / pathLength;
                dc.PushTransform(new MatrixTransform(matrix));
                dc.DrawText(formText, new Point(0, 0));
                dc.Pop();
            }
        }

        #region private methods

        private static double GetPathFigureLength(PathFigure pathFigure)
        {
            if (pathFigure == null)
                return 0;

            var isAlreadyFlattened = pathFigure.Segments
                .All(s => s is PolyLineSegment || s is LineSegment);

            var pathFigureFlattened = isAlreadyFlattened
                ? pathFigure
                : pathFigure.GetFlattenedPathFigure();

            var length = 0.0;
            var pt1 = pathFigureFlattened.StartPoint;

            foreach (var pathSegment in pathFigureFlattened.Segments)
            {
                if (pathSegment is LineSegment lineSegment)
                {
                    var pt2 = lineSegment.Point;
                    length += (pt2 - pt1).Length;
                    pt1 = pt2;
                }
                else if (pathSegment is PolyLineSegment polyLineSegment)
                {
                    var pointCollection = polyLineSegment.Points;
                    foreach (var pt2 in pointCollection)
                    {
                        length += (pt2 - pt1).Length;
                        pt1 = pt2;
                    }
                }
            }
            return length;
        }

        #endregion
    }
}
