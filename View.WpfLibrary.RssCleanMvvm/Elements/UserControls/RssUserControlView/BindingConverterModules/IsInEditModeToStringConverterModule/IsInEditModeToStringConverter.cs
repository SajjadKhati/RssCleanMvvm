using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.Elements.UserControls.RssUserControlView.BindingConverterModules.IsInEditModeToStringConverterModule
{
    public class IsInEditModeToStringConverter : IValueConverter
    {

        private readonly ResourceDictionary languageResourceDictionary;

        private readonly string defaultEditModeString = "حالت ویرایش";

        private readonly string defaultReadModeString = "حالت خواندن";


        //////////////////////////////////////////////////////////////


        public string EditModeKey { get; set; }

        public string ReadModeKey { get; set; }


        //////////////////////////////////////////////////////////////


        public IsInEditModeToStringConverter()
        {
            /// اینجا که به شی Application.Current دسترسی پیدا کردیم ، یه کم کد کثیف میشه اما 
            /// چون خیلی کلاس تبدیل کننده اون هم برای مصرف فقط همین برنامه که میخوایم استفاده کنیم ، الویت اول برای کد تمیز در این کلاس را نداره .
            this.languageResourceDictionary = System.Windows.Application.Current.Resources;
        }


        //////////////////////////////////////////////////////////////


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isInEditMode || isInEditMode == false)
                return (this.languageResourceDictionary.Contains(this.EditModeKey) == true)
                    ? this.languageResourceDictionary[this.EditModeKey]
                    : this.defaultEditModeString;
            
            return (this.languageResourceDictionary.Contains(this.ReadModeKey) == true)
                ? this.languageResourceDictionary[this.ReadModeKey]
                : this.defaultReadModeString;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
        
    }
}
