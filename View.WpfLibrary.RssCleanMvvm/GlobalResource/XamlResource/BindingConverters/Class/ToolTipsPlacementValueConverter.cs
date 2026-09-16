using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.XamlResource.BindingConverters.Class
{
    internal class ToolTipsPlacementValueConverter : IValueConverter
    {

        /// <summary>
        /// مقداری از نوع PlacementMode در شیِ ToolTip را میگیره و اگه این مقدار ، با یکی از مقادیر PlacementMode.Right یا PlacementMode.Left یا -
        /// PlacementMode.Center یا PlacementMode.MousePoint برابر بود ، مقدار true را برمیگردونه 
        /// تا در DataTrigger ئه مورد نظر ، حالت بالون برای ToolTip ئه مورد نظر ، مخفی بشه و حالتِ Border فقط داشته باشه وگرنه مقدار false را برمیگردونه-
        /// تا ToolTip مون ، حالت عادی که شکل بالون هست را داشته باشه .
        /// </summary>
        /// <param name="value">
        /// مقداری از نوع اینامِ PlacementMode در شیِ ToolTip</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>
        /// مقدار بولین را برمیگردونه .
        /// در بالا توضیح داده شد .
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PlacementMode toolTipPlacementMode = (PlacementMode)value;
            PlacementMode[] checkPlacementModes = new PlacementMode[] { PlacementMode.Right, PlacementMode.Left, PlacementMode.Center, PlacementMode.MousePoint };
            foreach (PlacementMode checkPlacementMode in checkPlacementModes)
            {
                if (toolTipPlacementMode == checkPlacementMode)
                    return true;
            }

            return false;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
