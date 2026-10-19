using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.AppearanceSwitcher.Interface;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.DataModel;
using ViewModel.Contracts.ViewContracts.Switchers.LanguageSwitcher.Interface;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace ViewModel.ViewModelModules.MainWindowViewModelModule.Class
{
    public class MainWindowViewModel : ObservableObject
    {

        private IAppearanceSwitcher appearanceSwitcher;

        protected ILanguageSwitcher languageSwitcher;


        /////////////////////////////////////////////////////////////////////////////


        private string _statusMessage;


        /////////////////////////////////////////////////////////////////////////////


        public RssUserControlViewModel RssUserControlViewModel { get; }

        public string StatusMessage
        {
            get => this._statusMessage;
            set
            {
                /// دقت کنید که برای این پروپرتی ، نباید شرط بذاریم که اگه مقدارش تغییر نکرد ، رویداد OnPropertyChanged اجرا بشه .
                /// چون ممکنه دو پیام خطا که متن شون دقیقا مثل هم هستند ، اجرا بشه . که با شرطِ اینکه مقدارشون مساوی باشه یا نه گذاشتن ، دیگه مقدار پروپرتی ، ارسال نمیشه .
                /// متد SetProperty را هم فراخوانی نکردیم چون این متد ، شرط تغییر مقدار را هم اجرا میکنه . بجایش از OnPropertyChanged استفاده کردیم .
                this._statusMessage = value;
                this.OnPropertyChanged();
            }
        }


        /////////////////////////////////////////////////////////////////////////////


        public IAsyncRelayCommand ExitCommand { get; }

        public IRelayCommand ChangeThemeCommand { get; }

        public IRelayCommand ChangeLanguageCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public MainWindowViewModel(RssUserControlViewModel rssUserControlViewModel, IAppearanceSwitcher appearanceSwitcher, 
            ILanguageSwitcher languageSwitcher)
        {
            this.RssUserControlViewModel = rssUserControlViewModel;
            this.appearanceSwitcher = appearanceSwitcher;
            this.languageSwitcher = languageSwitcher;

            this.ExitCommand = new AsyncRelayCommand(this.Exit);
            this.ChangeThemeCommand = new RelayCommand<ThemeName>(this.ChangeTheme, this.CanChangeTheme);
            this.ChangeLanguageCommand = new RelayCommand<LanguageName>(this.ChangeLanguage, this.CanChangeLanguage);
            this.RegisterMessageHandler();
        }


        /////////////////////////////////////////////////////////////////////////////


        private void RegisterMessageHandler()
        {
            WeakReferenceMessenger.Default.Register<string, string>(this, SharedMessengerKey.String_GeneralMessageKey, (_, message) =>
            {
                this.StatusMessage = message;
            });
        }


        private async Task Exit()
        {
            // فقط بررسی بشه که آیا کش موقت ، داده و اطلاعاتی داره یا نه؟
            // و اگه داره ، اول ، به کاربر پیام بده که آیا میخواد در دیتابیس ذخیره کنه یا نه؟
            // اگه کاربر تایید کرد ، منتقل کنه و از کش موقت حذف کنه .

            // اگه کاربر تایید نکرد ، هیچ کار نکنه چون :
            // در اینجا ، لازم به Dispose کردن کش موقت نیست  . چون در رویداد OnExit در کلاس App ، این کار انجام میشه .

        }


        private bool CanChangeTheme(ThemeName themeName)
        {
            return this.appearanceSwitcher.GetCurrentTheme() != themeName;
        }


        private void ChangeTheme(ThemeName themeName)
        {
            this.appearanceSwitcher.ChangeTheme(themeName);
            /// پس از تغییر تم، وضعیت CanExecute کامند را بهروزرسانی میکنیم
            /// تا المنت و کنترل هایی که به این Command مون Binding هستند ، وضعیت فعال یا غیر فعال بودن شون را به روزرسانی کنند ..
            this.ChangeThemeCommand.NotifyCanExecuteChanged();
        }


        private bool CanChangeLanguage(LanguageName languageName)
        {
            return this.languageSwitcher.GetCurrentLanguage() != languageName;
        }


        private void ChangeLanguage(LanguageName languageName)
        {
            this.languageSwitcher.ChangeLanguage(languageName);
            /// پس از تغییر زبان ، وضعیت CanExecute کامند را بهروزرسانی میکنیم
            /// تا المنت و کنترل هایی که به این Command مون Binding هستند ، وضعیت فعال یا غیر فعال بودن شون را به روزرسانی کنند ..
            this.ChangeLanguageCommand.NotifyCanExecuteChanged();
        }

    }
}
