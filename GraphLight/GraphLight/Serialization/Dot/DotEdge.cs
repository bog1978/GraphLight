namespace GraphLight.Serialization.Dot
{
    public class DotEdge : DotElement
    {
        public string Dst;
        public string Src;

        public string Style
        {
            get { return GetValue<string>("style"); }
            set { SetValue("style", value); }
        }

        public double Weight
        {
            get { return GetValue<double>("weight"); }
            set { SetValue("weight", value); }
        }

        public string Dir
        {
            get { return GetValue<string>("dir"); }
            set { SetValue("dir", value); }
        }

        public double PenWidth
        {
            get { return GetValue<double>("penwidth"); }
            set { SetValue("penwidth", value); }
        }

        public string Color
        {
            get { return GetValue<string>("color"); }
            set { SetValue("color", value); }
        }

        public string Tooltip
        {
            get { return GetValue<string>("tooltip", true); }
            set { SetValue("tooltip", value, true); }
        }

        public bool Constraint
        {
            get { return GetValue<bool>("constraint"); }
            set { SetValue("constraint", value); }
        }

        public string HeadLabel
        {
            get { return GetValue<string>("headlabel", true); }
            set { SetValue("headlabel", value, true); }
        }

        public string TailLabel
        {
            get { return GetValue<string>("taillabel", true); }
            set { SetValue("taillabel", value, true); }
        }
    }
}