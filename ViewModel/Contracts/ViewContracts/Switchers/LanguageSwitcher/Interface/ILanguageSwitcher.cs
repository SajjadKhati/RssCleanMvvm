using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.DataModel;

namespace ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface
{
    public interface ILanguageSwitcher
    {

        void ChangeLanguage(LanguageName languageName);

        LanguageName GetCurrentLanguage();

    }
}
