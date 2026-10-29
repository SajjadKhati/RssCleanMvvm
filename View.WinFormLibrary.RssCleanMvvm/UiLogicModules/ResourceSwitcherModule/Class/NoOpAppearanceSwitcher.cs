using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;

namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class
{
    /// <summary>
    /// معادلِ <c>AppearanceSwitcher</c> در نسخه‌ی WPF ، ولی به‌صورت NoOp .
    ///
    /// طبق درخواست، منوی «تنظیمات» و قابلیت تغییر تم در نسخه‌ی WinForms وجود ندارد .
    /// با این حال، <c>MainWindowViewModel</c> به یک <see cref="IAppearanceSwitcher"/> نیاز دارد
    /// و ما اجازه‌ی تغییر آن لایه را نداریم . پس این پیاده‌سازی، قرارداد را حفظ می‌کند
    /// ولی هیچ تغییری در ظاهر برنامه ایجاد نمی‌کند .
    /// </summary>
    public sealed class NoOpAppearanceSwitcher : IAppearanceSwitcher
    {
        /// <summary>ظاهر ثابتِ برنامه، همان تم روشن است .</summary>
        private readonly ThemeName currentTheme = ThemeName.Light;


        ///////////////////////////////////////////////////////////////////////


        public void ChangeTheme(ThemeName themeName)
        {
            /// تغییر تم در این نسخه پشتیبانی نمی‌شود .
        }


        public ThemeName GetCurrentTheme()
        {
            return this.currentTheme;
        }

    }
}
