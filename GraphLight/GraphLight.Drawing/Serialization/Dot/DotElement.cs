using System.Collections.Generic;

namespace GraphLight.Serialization.Dot
{
    public abstract class DotElement
    {
        private readonly IDictionary<string, object> _attrs = new Dictionary<string, object>();

        public IDictionary<string, object> Attrs
        {
            get { return _attrs; }
        }

        public string Label
        {
            get { return GetValue<string>("label", true); }
            set { SetValue("label", value, true); }
        }

        protected T GetValue<T>(string key, bool quote = false)
        {
            object value;
            _attrs.TryGetValue(key, out value);
            if (value is string && quote)
            {
                var str = (string) value;
                value = str.Trim('"');
            }
            if (value is T)
            {
                return (T) value;
            }
            return default(T);
        }

        protected void SetValue(string key, object value, bool quote = false)
        {
            if (value is string && quote)
                _attrs[key] = "\"" + value + "\"";
            else
                _attrs[key] = value;
        }
    }
}