using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;

namespace ViewModel.Contracts.ViewContracts.AppearanceSwitcher.Interface
{
    public interface IAppearanceSwitcher
    {

        void ChangeTheme(ThemeName themeName);

        ThemeName GetCurrentTheme();

    }
}
