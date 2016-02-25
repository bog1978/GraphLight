using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace GraphLight.Controls
{
    public class ContainerBinding : DependencyObject
    {
        public string Property { get; set; }
        public Binding Binding { get; set; }

        internal DependencyProperty GetDependencyProperty(Type targetType)
        {
            var parts = Property.Split('.');
            var p = parts.Last();
            var field = targetType.GetField(p + "Property",
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            return field != null
                ? field.GetValue(null) as DependencyProperty
                : null;
        }
    }
}