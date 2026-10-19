using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.DataModel;

namespace ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface
{
    public interface IAppearanceSwitcher
    {

        void ChangeTheme(ThemeName themeName);

        ThemeName GetCurrentTheme();

    }
}
