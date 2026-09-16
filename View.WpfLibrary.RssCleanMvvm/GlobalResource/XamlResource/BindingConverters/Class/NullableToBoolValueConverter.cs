using System.Globalization;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.XamlResource.BindingConverters.Class
{
    public class NullableToBoolValueConverter : IValueConverter
    {

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }


        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
            => throw new NotImplementedException();
        
    }
}
