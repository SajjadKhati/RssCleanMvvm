using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;

namespace CompositionRoot.WIndowsFormApp.UiLogicModules.ResourceSwitcherModule
{
    /// <summary>
    /// نسخه‌ی WinForms ، قابلیت تغییر Theme را ندارد .
    /// این کلاس ، قرارداد IAppearanceSwitcher را پیاده‌سازی می‌کند تا ViewModel بدون تغییر باقی بماند ، -
    /// ولی هیچ تغییری در ظاهر برنامه ایجاد نمی‌کند و همیشه تم ثابت (روشن) را برمی‌گرداند .
    /// </summary>
    internal sealed class NoOpAppearanceSwitcher : IAppearanceSwitcher
    {

        private readonly ThemeName currentTheme = ThemeName.Light;


        ///////////////////////////////////////////////////////////////////////


        public void ChangeTheme(ThemeName themeName)
        {
            /// تغییر Theme در نسخه‌ی WinForms پشتیبانی نمی‌شود .
        }


        public ThemeName GetCurrentTheme()
        {
            return this.currentTheme;
        }

    }
}
