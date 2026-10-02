using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.IsInEditModeToVisibility
{
    public class IsInEditModeToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDefaultMode = parameter as string == "Invert";
            if (value is not bool isInEditMode || isInEditMode == false)
                return (isDefaultMode == true) ?  Visibility.Collapsed : Visibility.Visible;
            return (isDefaultMode == true) ? Visibility.Visible : Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
        
    }
}
