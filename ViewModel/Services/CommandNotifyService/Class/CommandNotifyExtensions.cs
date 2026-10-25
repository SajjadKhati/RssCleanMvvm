using CommunityToolkit.Mvvm.Input;

namespace ViewModel.Services.CommandNotifyService.Class
{
    /// <summary>
    /// Extension methodهایی برای زنجیره‌کردن CanExecute کامندهای async.
    ///
    /// مسئله‌ای که حل می‌کند:
    /// وقتی CanExecute یک کامند (مثلاً «لغو دانلود») به وضعیت کامندهای دیگر
    /// (مثلاً «دریافت اخبار جدید» و «بارگذاری») وابسته است، تغییر IsRunning یا
    /// IsCancellationRequested در آن کامندها باید باعث ارزیابی مجدد CanExecute
    /// در کامند وابسته شود. بدون این، دکمه‌ها در وضعیت قدیمی (فعال/غیرفعال)
    /// باقی می‌مانند.
    ///
    /// چرا Extension Method؟
    /// برای این‌که در Constructor هر ViewModel، وابستگی‌ها به‌صورت fluent و
    /// خوانا بیان شوند:
    ///   this.LoadRssItemsCommand.NotifyCanExecuteChangedOf(
    ///       this.DownLoadRssItemsCommand,
    ///       this.CancelCurrentOperationCommand);
    /// بدون نیاز به handlerهای جداگانه در ViewModel و بدون تکرار boilerplate.
    /// </summary>
    public static class CommandNotifyExtensions
    {

        /// <summary>
        /// مجموعه‌ی استاندارد propertyهایی که وضعیت اجرای یک کامند async را نشان می‌دهند.
        /// (IsRunning: در حال اجرا؟ / IsCancellationRequested: قبلاً لغو شده؟ /
        /// CanBeCanceled: delegate توکن می‌گیرد؟)
        ///
        /// private است، چون مصرف‌کننده‌های بیرونی نیازی به دسترسی مستقیم ندارند؛
        /// فقط از overload ساده استفاده می‌کنند.
        /// </summary>
        private static readonly IReadOnlyCollection<string> StandardStatePropertyNames = new[]
        {
            nameof(IAsyncRelayCommand.IsRunning),
            nameof(IAsyncRelayCommand.IsCancellationRequested),
            nameof(IAsyncRelayCommand.CanBeCanceled),
        };


        /// <summary>
        /// نسخه‌ی ساده: با هر تغییر در هر سه property استاندارد
        /// (IsRunning, IsCancellationRequested, CanBeCanceled) کامند منبع،
        /// کامندهای وابسته را وادار به ارزیابی مجدد CanExecute می‌کند.
        /// </summary>
        /// <param name="source">
        /// کامند منبع (watch‌شده). وضعیت این کامند، تغییرات را trigger می‌کند.
        /// </param>
        /// <param name="commandsToNotify">
        /// کامندهایی که باید با تغییر وضعیت source، CanExecute‌شان دوباره
        /// ارزیابی شود. اگر خالی یا null باشد، هیچ کاری انجام نمی‌شود.
        /// </param>
        /// <returns>
        /// همان source (برای امکان زنجیره‌کردن fluent).
        /// </returns>
        public static IAsyncRelayCommand PropagateNotifyCanExecuteChangedToCommands(this IAsyncRelayCommand source, params IRelayCommand[] commandsToNotify)
        {
            return source.PropagateNotifyCanExecuteChangedToCommands(StandardStatePropertyNames, commandsToNotify);
        }


        /// <summary>
        /// نسخه‌ی پیشرفته: فقط propertyهای مشخص‌شده در observedProperties را
        /// watch می‌کند. برای مواقعی که فقط برخی propertyها (مثلاً فقط IsRunning)
        /// برای مصرف‌کننده مهم‌اند.
        /// </summary>
        /// <param name="source">
        /// کامند منبع (watch‌شده).
        /// </param>
        /// <param name="observedProperties">
        /// نام propertyهایی از source که تغییرشان باید trigger باشد
        /// (مثلاً nameof(IAsyncRelayCommand.IsRunning)).
        /// اگر null باشد، ArgumentNullException پرتاب می‌شود.
        /// </param>
        /// <param name="commandsForExecuteNotify">
        /// کامندهایی که باید با تغییر propertyهای watch‌شده، CanExecute‌شان
        /// دوباره ارزیابی شود. اگر خالی یا null باشد، هیچ‌چیز subscribe نمی‌شود.
        /// </param>
        /// <returns>
        /// همان source (برای امکان زنجیره‌کردن fluent).
        /// </returns>
        public static IAsyncRelayCommand PropagateNotifyCanExecuteChangedToCommands(this IAsyncRelayCommand source, 
            IReadOnlyCollection<string> observedProperties, params IRelayCommand[] commandsForExecuteNotify)
        {
            if (observedProperties is null) 
                throw new ArgumentNullException(nameof(observedProperties));
            if (commandsForExecuteNotify is null || commandsForExecuteNotify.Length == 0)
                return source;

            source.PropertyChanged += (_, e) =>
            {
                /// اگر propertyName خالی یا خارج از مجموعه‌ی watch‌شده باشد،
                /// notify لازم نیست (بهینه‌سازی: جلوگیری از NotifyCanExecuteChangedهای بی‌مورد).
                if (e.PropertyName is null || observedProperties.Contains(e.PropertyName) == false)
                    return;

                foreach (IRelayCommand command in commandsForExecuteNotify)
                    command.NotifyCanExecuteChanged();
            };

            return source;
        }

    }
}
