namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class
{
    /// <summary>
    /// ظرفِ اشتراک‌های یک View (معادلِ <c>CompositeDisposable</c> در ReactiveUI).
    ///
    /// چرا لازم است؟
    /// در WPF، Binding ها با Dispose شدنِ کنترل، خودبه‌خود قطع می‌شوند . ولی در WinForms،
    /// اشتراک در رویدادهای <see cref="System.ComponentModel.INotifyPropertyChanged"/> و
    /// <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/> باید دستی قطع شود ؛
    /// وگرنه ViewModel (که عمرش از View بیشتر است) به View مرده ارجاع می‌دهد و نشتی حافظه رخ می‌دهد .
    ///
    /// هر View یک نمونه از این کلاس دارد و همه‌ی اشتراک‌ها را در آن ثبت می‌کند . هنگام Dispose
    /// شدنِ View، فقط کافی است همان یک نمونه Dispose شود .
    /// </summary>
    public sealed class BindingScope : IDisposable
    {
        private readonly List<Action> unsubscribers = new();

        private bool isDisposed;


        ///////////////////////////////////////////////////////////////////////


        /// <summary>
        /// یک «لغو اشتراک» ثبت می‌کند . اگر scope قبلاً Dispose شده باشد، همان لحظه لغو می‌شود .
        /// </summary>
        public void Add(Action unsubscriber)
        {
            if (unsubscriber == null)
                throw new ArgumentNullException(nameof(unsubscriber));

            if (this.isDisposed == true)
            {
                unsubscriber();
                return;
            }

            this.unsubscribers.Add(unsubscriber);
        }


        public void Add(IDisposable disposable)
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            this.Add(disposable.Dispose);
        }


        /// <summary>
        /// یک <see cref="Binding"/> استاندارد WinForms را به کنترل اضافه می‌کند و
        /// حذفِ آن را هم در همین scope ثبت می‌کند .
        /// </summary>
        public void AddBinding(Control control, Binding binding)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            control.DataBindings.Add(binding);
            this.Add(() => control.DataBindings.Remove(binding));
        }


        public void Dispose()
        {
            if (this.isDisposed == true)
                return;

            this.isDisposed = true;

            /// برعکسِ ترتیبِ ثبت، لغو می‌کنیم (مثل اصل LIFO در CompositeDisposable).
            for (int index = this.unsubscribers.Count - 1; index >= 0; index--)
            {
                try
                {
                    this.unsubscribers[index]();
                }
                catch (Exception)
                {
                    /// Dispose نباید به‌خاطر یک لغو اشتراکِ خطادار، بقیه را رها کند .
                }
            }

            this.unsubscribers.Clear();
        }

    }
}
