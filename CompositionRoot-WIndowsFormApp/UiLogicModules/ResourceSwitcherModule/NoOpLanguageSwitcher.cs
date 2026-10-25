using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;

namespace CompositionRoot.WIndowsFormApp.UiLogicModules.ResourceSwitcherModule
{
    /// <summary>
    /// نسخه‌ی WinForms ، قابلیت تغییر Language را ندارد .
    /// این کلاس ، قرارداد ILanguageSwitcher را پیاده‌سازی می‌کند تا ViewModel بدون تغییر باقی بماند ، -
    /// ولی همیشه زبان ثابت (فارسی) را برمی‌گرداند .
    /// </summary>
    internal sealed class NoOpLanguageSwitcher : ILanguageSwitcher
    {

        private readonly LanguageName currentLanguage = LanguageName.Persian;


        ///////////////////////////////////////////////////////////////////////


        public void ChangeLanguage(LanguageName languageName)
        {
            /// تغییر Language در نسخه‌ی WinForms پشتیبانی نمی‌شود .
        }


        public LanguageName GetCurrentLanguage()
        {
            return this.currentLanguage;
        }

    }
}
