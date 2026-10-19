using System.Collections.ObjectModel;
using System.Windows;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class
{
    public class LanguageSwitcher : ResourceSwitcherBase<LanguageName>, ILanguageSwitcher
    {

        public LanguageSwitcher(Collection<ResourceDictionary> applicationMergedDictionary, 
            SwitchableResourceInfo themeResourceInfo, LanguageName defaultLanguage) : 
            base(applicationMergedDictionary, themeResourceInfo, defaultLanguage)
        {
        }


        ////////////////////////////////////////////////////////////////////


        public void ChangeLanguage(LanguageName languageName)
        {
            this.SwitchTo(languageName);
        }


        public LanguageName GetCurrentLanguage()
        {
            return this.GetCurrent();
        }

    }
}
