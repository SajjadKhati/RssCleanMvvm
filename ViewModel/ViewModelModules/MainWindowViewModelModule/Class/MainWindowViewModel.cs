using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.DataModels;
using ViewModel.ViewModelModules.RssUserControlViewModelModule.Class;

namespace ViewModel.ViewModelModules.MainWindowViewModelModule.Class
{
    public class MainWindowViewModel : ObservableObject
    {

        public RssUserControlViewModel RssUserControlViewModel { get; }


        public IAsyncRelayCommand ExitCommand { get; }


        /////////////////////////////////////////////////////////////////////////////


        public MainWindowViewModel(RssUserControlViewModel rssUserControlViewModel)
        {
            this.RssUserControlViewModel = rssUserControlViewModel;
            this.ExitCommand = new AsyncRelayCommand(this.Exit);
        }


        /////////////////////////////////////////////////////////////////////////////


        private async Task Exit()
        {
            // فقط بررسی بشه که آیا کش موقت ، داده و اطلاعاتی داره یا نه؟
            // و اگه داره ، اول ، به کاربر پیام بده که آیا میخواد در دیتابیس ذخیره کنه یا نه؟
            // اگه کاربر تایید کرد ، منتقل کنه و از کش موقت حذف کنه .

            // اگه کاربر تایید نکرد ، هیچ کار نکنه چون :
            // در اینجا ، لازم به Dispose کردن کش موقت نیست  . چون در رویداد OnExit در کلاس App ، این کار انجام میشه .

        }

    }
}
