using System;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public class VertexAttrs : BaseViewModel, IVertexAttrs
    {
        #region Private fields

        private static int _cnt;
        private string _category;
        private double _centerX;
        private double _height;
        private string _id;
        private bool _isHighlighted;
        private bool _isSelected;
        private bool _isTmp;
        private string _label;
        private double _left;
        private int _position;
        private int _rank;
        private string _shapeData;
        private double _top;
        private double _width;
        private int _zIndex;

        #endregion

        public VertexAttrs()
            : this("tmp" + _cnt++)
        {
        }

        public VertexAttrs(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            Id = id;
        }

        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                _zIndex = value;
                RaisePropertyChanged("ZIndex");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
                IsHighlighted = value;
            }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                RaisePropertyChanged("IsHighlighted");
            }
        }

        public string ShapeData
        {
            get { return _shapeData; }
            set
            {
                _shapeData = value;
                RaisePropertyChanged("ShapeData");
            }
        }

        #region IVertexAttrs Members

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged("Label");
            }
        }

        public int Rank
        {
            get { return _rank; }
            set
            {
                _rank = value;
                RaisePropertyChanged("Rank");
            }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged("Position");
            }
        }

        public bool IsTmp
        {
            get { return _isTmp; }
            set
            {
                _isTmp = value;
                RaisePropertyChanged("IsTmp");
            }
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (Label == null)
                    Label = value;
                RaisePropertyChanged("Id");
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged("Width");
                RaisePropertyChanged("Right");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged("Height");
                RaisePropertyChanged("Bottom");
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged("Left");
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged("Top");
            }
        }

        public double Right
        {
            get { return _left + _width; }
        }

        public double Bottom
        {
            get { return _top + _height; }
        }

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                _centerX = value;
                RaisePropertyChanged("CenterX");
            }
        }

        public VertexColor Color { get; set; }

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged("Category");
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as IVertexAttrs;
            if (other == null)
                return false;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public IVertexAttrs Clone()
        {
            var clone = new VertexAttrs
                {
                    Category = Category,
                    Color = Color,
                    Label = Label,
                    ShapeData = ShapeData,
                };
            return clone;
        }
    }
}