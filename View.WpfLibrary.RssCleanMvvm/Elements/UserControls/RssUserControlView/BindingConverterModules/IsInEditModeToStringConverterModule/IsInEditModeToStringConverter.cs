using System.Globalization;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.IsInEditModeToStringConverterModule
{
    public class IsInEditModeToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isInEditMode || isInEditMode == false)
                return "حالت ویرایش";
            return "حالت خواندن";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
        
    }
}
