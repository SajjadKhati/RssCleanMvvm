using System.Collections.ObjectModel;
using System.Windows;
using ViewModel.Contracts.ViewContracts.AppearanceSwitcher.DataModel;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.ResourceSwitcherModule.Class
{
    public abstract class ResourceSwitcherBase<TSwitchTarget> where TSwitchTarget : struct, Enum
    {

        private readonly Collection<ResourceDictionary> applicationMergedDictionary;

        private readonly SwitchableResourceInfo themeResourceInfo;

        private TSwitchTarget currentSwitchTarget;


        ////////////////////////////////////////////////////////////////////


        public ResourceSwitcherBase(Collection<ResourceDictionary> applicationMergedDictionary,
            SwitchableResourceInfo themeResourceInfo, TSwitchTarget defaultSwitchTarget)
        {
            this.applicationMergedDictionary = applicationMergedDictionary;
            this.themeResourceInfo = themeResourceInfo;
            this.currentSwitchTarget = defaultSwitchTarget;
            this.Validate();
        }


        ////////////////////////////////////////////////////////////////////


        protected void SwitchTo(TSwitchTarget newSwitchTarget)
        {
            if (EqualityComparer<TSwitchTarget>.Default.Equals(newSwitchTarget, this.currentSwitchTarget))
                return;

            this.applicationMergedDictionary[this.themeResourceInfo.Index] = new ResourceDictionary
            {
                Source = new Uri($"{this.themeResourceInfo.BasePath}{newSwitchTarget}{this.themeResourceInfo.FileSuffix}", UriKind.RelativeOrAbsolute),
            };
            this.currentSwitchTarget = newSwitchTarget;
        }


        public TSwitchTarget GetCurrent()
        {
            return this.currentSwitchTarget;
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
