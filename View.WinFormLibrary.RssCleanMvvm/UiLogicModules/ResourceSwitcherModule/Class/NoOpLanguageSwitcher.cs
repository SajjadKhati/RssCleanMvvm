using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;

namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class
{
    /// <summary>
    /// معادلِ <c>LanguageSwitcher</c> در نسخه‌ی WPF ، ولی به‌صورت NoOp .
    ///
    /// زبان برنامه، فارسیِ ثابت است (طبق درخواست، تغییر زبان لازم نیست) .
    /// این کلاس فقط برای ارضای قرارداد <see cref="ILanguageSwitcher"/> در ViewModel است .
    /// </summary>
    public sealed class NoOpLanguageSwitcher : ILanguageSwitcher
    {
        private readonly LanguageName currentLanguage = LanguageName.Persian;


        ///////////////////////////////////////////////////////////////////////


        public void ChangeLanguage(LanguageName languageName)
        {
            /// تغییر زبان در این نسخه پشتیبانی نمی‌شود .
        }


        public LanguageName GetCurrentLanguage()
        {
            return this.currentLanguage;
        }

    }
}
