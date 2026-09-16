using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using ViewModel.DataModels;
using ViewModel.Services.SharedMessengerKeyService.Class;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace ViewModel.ViewModelModules.MainWindowViewModelModule.Class
{
    public class MainWindowViewModel : ObservableObject
    {
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


        public IAsyncRelayCommand ExitCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public MainWindowViewModel(RssUserControlViewModel rssUserControlViewModel)
        {
            this.RssUserControlViewModel = rssUserControlViewModel;
            this.ExitCommand = new AsyncRelayCommand(this.Exit);
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




        /////////////////////////////////////////////////////////////////////////////



    }
}
