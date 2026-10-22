using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace View.WpfLibrary.RssCleanMvvm.GlobalResource.XamlResource.Styles
{
    public partial class MenuStyleResource : ResourceDictionary
    {

        public MenuStyleResource()
        {
            InitializeComponent();
        }


        ///////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// وقتی یک Submenu باز می‌شود، محتوای Popup آن را مجبور به measure مجدد می‌کند.
        ///
        /// چرا این لازم است؟
        /// Popup در WPF، اندازه‌ی محتوایش را فقط یک بار (اولین باز شدن) محاسبه
        /// و cache می‌کند. وقتی زبان برنامه عوض می‌شود و Header منوآیتم‌ها (که با
        /// DynamicResource تعریف شده‌اند) متن بلندتری می‌گیرند، Popup با عرض
        /// قدیمی رندر می‌شود و متن بلندتر از دو طرف قطع می‌شود.
        ///
        /// چرا InvalidateMeasure روی MenuItem کافی نیست؟
        /// چون Popup در یک پنجره‌ی جداگانه (یا overlay) رندر می‌شود، نه در
        /// visual tree اصلی. UpdateLayout روی MenuItem به Popup.Child نمی‌رسد.
        /// باید مستقیماً محتوای Popup را invalidate کنیم.
        ///
        /// چرا کل زیردرخت (recursive)؟
        /// چون فقط Border (لایه‌ی اول) کافی نیست. ScrollViewer و StackPanel
        /// و هر MenuItem زیرمنو، اندازه‌ی قبلی‌شان را cache کرده‌اند. برای
        /// این‌که تغییر زبان واقعاً روی اندازه اثر بگذارد، همه‌ی این لایه‌ها
        /// باید از نو measure شوند.
        ///
        /// چرا DispatcherPriority.Background؟
        /// چون این کار باید بعد از انیمیشن باز شدن Popup (Slide یا Fade)
        /// اجرا شود. اگر با Loaded اجرا شود، انیمیشن را قفل می‌کند و لگ
        /// محسوسی در باز شدن منو ایجاد می‌شود.
        /// </summary>
        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            /// پیدا کردن Popupِ متعلق به همین Submenu.
            /// در تمپلیت MenuItem، Popup یک فرزند از نوع Popup است که داخل
            /// visual tree قرار دارد. با VisualTreeHelper آن را پیدا می‌کنیم
            /// (بدون وابستگی به نامش در XAML).
            Popup? popup = this.FindPopupInVisualTree(menuItem);

            if (popup?.Child is not FrameworkElement popupContent)
                return;

            /// اجرا در Background: بعد از این‌که Popup کاملاً رندر و انیمیشنش
            /// تمام شد، حالا محتوایش را invalidate می‌کنیم تا با اندازه‌ی
            /// جدید محتوا (متن زبان جاری) هم‌گام شود.
            popup.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.InvalidateMeasureRecursively(popupContent);
                popupContent.UpdateLayout();
            }), DispatcherPriority.Background);
        }


        ///////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// اولین Popup موجود در زیردرخت بصریِ یک عنصر را برمی‌گرداند.
        ///
        /// چرا VisualTreeHelper و نه FindName؟
        /// FindName به «نام» عنصر در ControlTemplate وابسته است. اگر فردا
        /// نام Popup در XAML عوض شود، FindName می‌شکند. VisualTreeHelper
        /// فقط به «نوع» (Popup) تکیه می‌کند، نه به نام.
        ///
        /// چرا recursive؟
        /// چون Popup در عمق دلخواهی از درخت می‌تواند قرار بگیرد (بسته به
        /// ساختار ControlTemplate). ما با پیمایش کامل درخت، مطمئن می‌شویم
        /// که هر Popupی را پیدا می‌کنیم.
        /// </summary>
        private Popup? FindPopupInVisualTree(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is Popup popup)
                    return popup;

                Popup? foundInSubtree = this.FindPopupInVisualTree(child);
                if (foundInSubtree is not null)
                    return foundInSubtree;
            }

            return null;
        }


        /// <summary>
        /// کل زیردرخت بصری یک عنصر را invalidateMeasure می‌کند.
        ///
        /// چرا recursive؟
        /// چون فقط invalidate کردن عنصر ریشه کافی نیست. WPF هر عنصر را
        /// جداگانه measure می‌کند و اگر فرزندی valid باشد، اندازه‌ی قبلی‌اش
        /// را دوباره استفاده می‌کند. برای این‌که تغییر زبان (متن بلندتر)
        /// واقعاً روی اندازه‌ی نهایی اثر بگذارد، همه‌ی لایه‌ها (ScrollViewer،
        /// StackPanel، MenuItemها، ContentPresenterها، TextBlockها) باید
        /// از نو measure شوند.
        ///
        /// این کار در لحظه‌ی باز شدن Popup انجام می‌شود (نه یک بار برای همیشه)،
        /// چون هر Popup اندازه‌ی خودش را جداگانه cache می‌کند و باید همان
        /// لحظه که می‌خواهد رندر شود، اندازه‌اش تازه محاسبه شود.
        /// </summary>
        private void InvalidateMeasureRecursively(DependencyObject element)
        {
            if (element is UIElement uiElement)
                uiElement.InvalidateMeasure();

            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
                this.InvalidateMeasureRecursively(VisualTreeHelper.GetChild(element, i));
        }

    }
}
