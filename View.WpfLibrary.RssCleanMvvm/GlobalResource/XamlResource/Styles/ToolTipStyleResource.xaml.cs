using System.Windows;
using System.Windows.Controls;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.XamlResource.Styles
{
    public partial class ToolTipStyleResource : ResourceDictionary
    {

        public ToolTipStyleResource()
        {
            InitializeComponent();
        }


        /////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// با هر بار باز شدن ToolTip، اندازه‌اش را دوباره محاسبه می‌کند.
        ///
        /// چرا لازم است؟
        /// ToolTip داخل یک Popup رندر می‌شود و Popup فقط یک بار محتوایش را measure می‌کند.
        /// اگر محتوای ToolTip بعد از اولین نمایش تغییر کند (مثلاً با DynamicResource
        /// هنگام تغییر زبان یا تم)، اندازه‌ی Popup روی مقدار قبلی می‌ماند و متن
        /// ممکن است بریده یا با فضای خالی نمایش داده شود.
        ///
        /// InvalidateMeasure + UpdateLayout باعث می‌شود Popup اندازه‌ی جدید را
        /// دوباره محاسبه کند. InvalidateMeasure تنها کافی نیست، چون فقط یک flag
        /// می‌گذارد و layout pass را فوراً اجرا نمی‌کند.
        /// </summary>
        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ToolTip toolTip)
            {
                toolTip.InvalidateMeasure();
                toolTip.UpdateLayout();
            }
        }

    }
}
