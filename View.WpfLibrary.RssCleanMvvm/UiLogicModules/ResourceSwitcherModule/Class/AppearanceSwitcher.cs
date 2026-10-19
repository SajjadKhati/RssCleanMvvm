using System.Collections.ObjectModel;
using System.Windows;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class
{
    public class AppearanceSwitcher : ResourceSwitcherBase<ThemeName>, IAppearanceSwitcher
    {

        public AppearanceSwitcher(Collection<ResourceDictionary> applicationMergedDictionary, 
            SwitchableResourceInfo themeResourceInfo, ThemeName defaultTheme) : 
            base(applicationMergedDictionary, themeResourceInfo, defaultTheme)
        {
        }


        ////////////////////////////////////////////////////////////////////


        public void ChangeTheme(ThemeName themeName)
        {
            this.SwitchTo(themeName);
        }


        public ThemeName GetCurrentTheme()
        {
            return this.GetCurrent();
        }

    }
}
