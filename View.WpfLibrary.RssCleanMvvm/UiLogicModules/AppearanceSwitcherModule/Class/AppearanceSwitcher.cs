using System.Collections.ObjectModel;
using System.Windows;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.Interface;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.AppearanceSwitcherModule.Class
{
    public class AppearanceSwitcher : IAppearanceSwitcher
    {

        private readonly Collection<ResourceDictionary> applicationMergedDictionary;

        private readonly SwitchableResourceInfo themeResourceInfo;

        private ThemeName currentTheme;


        ////////////////////////////////////////////////////////////////////


        public AppearanceSwitcher(Collection<ResourceDictionary> applicationMergedDictionary,
            SwitchableResourceInfo themeResourceInfo, ThemeName defaultTheme)
        {
            this.applicationMergedDictionary = applicationMergedDictionary;
            this.themeResourceInfo = themeResourceInfo;
            this.currentTheme = defaultTheme;
            this.Validate();
        }


        ////////////////////////////////////////////////////////////////////


        public void ChangeTheme(ThemeName themeName)
        {
            if(themeName == this.currentTheme)
                return;

            this.applicationMergedDictionary[this.themeResourceInfo.Index] = new ResourceDictionary
            {
                Source = new Uri($"{this.themeResourceInfo.BasePath}{themeName}{this.themeResourceInfo.FileSuffix}", UriKind.RelativeOrAbsolute),
            };
            this.currentTheme = themeName;
        }


        public ThemeName GetCurrentTheme()
        {
            return this.currentTheme;
        }


        ////////////////////////////////////////////////////////////////////


        private void Validate()
        {
            if (this.applicationMergedDictionary == null)
                throw new ArgumentNullException(nameof(applicationMergedDictionary));

            if (this.themeResourceInfo.Index < 0 || this.themeResourceInfo.Index >= applicationMergedDictionary.Count)
                throw new ArgumentOutOfRangeException(nameof(this.themeResourceInfo.Index),
                    $"تعدادِ ایندکسِ تم ، خارج از محدوده است.\nتعداد MergedDictionaries: {applicationMergedDictionary.Count}");

            if (this.applicationMergedDictionary[this.themeResourceInfo.Index]?.Source == null)
                throw new InvalidOperationException($"ResourceDictionary در ایندکس {this.themeResourceInfo.Index} هیچ Source ندارد.");
        }

    }
}
