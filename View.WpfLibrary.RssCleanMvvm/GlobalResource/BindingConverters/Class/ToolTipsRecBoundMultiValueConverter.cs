using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.BindingConverters.Class
{
    public class ToolTipsRecBoundMultiValueConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double toolTipWidth = System.Convert.ToDouble(values[0]);
            double toolTipHeight = System.Convert.ToDouble(values[1]);
            /// چون ویژال استودیو 2022 در xaml designer گیر میده که width و height باید اعداد مثبت باشن ، براشون متد قدر مطلق را توسط متدِ Math.Abs در نظر گرفتیم .
            double rectangleWidth = Math.Abs(toolTipWidth - 2);
            double pathHeight = toolTipHeight - 2;
            /// برای خروجیِ double گرفتن ، حداقل ، یک طرفِ معادله باید از نوع اعشاری باشه . واسه ی همین عدد 20 در پایین را از نوع double گرفتیم .
            /// چون ویژال استودیو 2022 در xaml designer گیر میده که width و height باید اعداد مثبت باشن ، براشون متد قدر مطلق را توسط متدِ Math.Abs در نظر گرفتیم .
            double rectangleHeight = Math.Abs(pathHeight * (80d / 100));
            double rectangleY = pathHeight - rectangleHeight;
            return new Rect(1, rectangleY, rectangleWidth, rectangleHeight);
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
