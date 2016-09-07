using System;
using GraphLight.ViewModel;

namespace GraphLight.Graph
{
    public class VertexAttrs : BaseViewModel
    {
        private static int _cnt;
        private string _id;

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

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as VertexAttrs;
            if (other == null)
                return false;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}