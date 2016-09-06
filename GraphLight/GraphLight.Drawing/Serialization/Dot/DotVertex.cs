namespace GraphLight.Serialization.Dot
{
    public class DotVertex : DotElement
    {
        public string Id { get; set; }

        public string Color
        {
            get { return GetValue<string>("color"); }
            set { SetValue("color", value); }
        }

        public string Style
        {
            get { return GetValue<string>("style"); }
            set { SetValue("style", value); }
        }

        public string Tooltip
        {
            get { return GetValue<string>("tooltip", true); }
            set { SetValue("tooltip", value, true); }
        }
    }
}