using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace View.WpfLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.IsUnsavedToBrushConverterModule
{
    public class IsUnsavedToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool isUnsaved) == true)
                return isUnsaved == true ? Brushes.Red : Brushes.Gray;
            return Brushes.Gray;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
        
    }
}
