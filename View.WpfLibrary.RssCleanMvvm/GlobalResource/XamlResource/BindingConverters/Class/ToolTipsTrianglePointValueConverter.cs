using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.XamlResource.BindingConverters.Class
{
    internal class ToolTipsTrianglePointValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double triangleX = System.Convert.ToDouble(parameter);
            double toolTipHeight = System.Convert.ToDouble(value);
            double pathHeight = toolTipHeight - 2;
            double triangleY = pathHeight * (25d / 100);
            return new Point(triangleX, triangleY);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
