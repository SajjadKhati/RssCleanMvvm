using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace View.WpfLibrary.RssCleanMvvm.UiLogicModules.RoutedEventTriggerModule.Class
{
    /// <summary>
    /// تریگری که به هر RoutedEvent (شامل Attached Routed Eventهایی مثل
    /// Binding.SourceUpdatedEvent) وصل می‌شود و Actionهای داخلی‌اش را اجرا می‌کند.
    ///
    /// برخلاف EventTrigger پیش‌فرضِ Microsoft.Xaml.Behaviors که فقط CLR Eventها را
    /// با Reflection پیدا می‌کند، این کلاس مستقیماً از AddHandler استفاده می‌کند.
    /// </summary>
    public class RoutedEventTrigger : EventTriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty RoutedEventProperty = DependencyProperty.Register(
            nameof(RoutedEvent), typeof(RoutedEvent), typeof(RoutedEventTrigger), new PropertyMetadata(null, OnRoutedEventChanged));

        public static readonly DependencyProperty MarkAsHandledProperty = DependencyProperty.Register(
                nameof(MarkAsHandled), typeof(bool), typeof(RoutedEventTrigger), new PropertyMetadata(false));

        public static readonly DependencyProperty HandledEventsTooProperty = DependencyProperty.Register(
                nameof(HandledEventsToo), typeof(bool), typeof(RoutedEventTrigger), new PropertyMetadata(false, OnHandledEventsTooChanged));


        /////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// رویداد مسیریابی که می‌خواهیم به آن گوش بدهیم.
        /// می‌توان با {x:Static} یا با نام رشته‌ای (مثل "MouseDown") مقدار داد.
        /// </summary>
        [TypeConverter(typeof(RoutedEventConverter))]
        public RoutedEvent? RoutedEvent
        {
            get => (RoutedEvent?)GetValue(RoutedEventProperty);
            set => SetValue(RoutedEventProperty, value);
        }


        /// <summary>
        /// اگر true باشد، حتی اگر رویداد توسط کنترل دیگری Handle شده باشد،
        /// باز هم دریافت می‌شود.
        /// </summary>
        public bool HandledEventsToo
        {
            get => (bool)GetValue(HandledEventsTooProperty);
            set => SetValue(HandledEventsTooProperty, value);
        }


        /// <summary>
        /// این پروپرتی ، همون معادل e.Handled در متغییرهای رویدادی از نوع RoutedEventArgs عمل میکنه -
        /// و اگه true باشه ، اون رویداد در درخت بصری المنت ها ، هندلر در درخت  بصری در المنت ها را فراخوانی نمیکنه 
        /// </summary>
        public bool MarkAsHandled
        {
            get => (bool)GetValue(MarkAsHandledProperty);
            set => SetValue(MarkAsHandledProperty, value);
        }


        /////////////////////////////////////////////////////////////////////////////


        private Delegate? handler;

        /// <summary>
        /// چون در نسخه‌ی Microsoft.Xaml.Behaviors که استفاده می‌کنیم
        /// IsAttached به‌صورت public در دسترس نیست، خودمان یک فیلد نگه می‌داریم.
        /// </summary>
        private bool isAttached;


        /////////////////////////////////////////////////////////////////////////////


        protected override void OnAttached()
        {
            base.OnAttached();

            this.isAttached = true;
            this.AttachHandler();
        }


        protected override void OnDetaching()
        {
            this.isAttached = false;
            this.DetachHandler();

            base.OnDetaching();
        }


        /////////////////////////////////////////////////////////////////////////////


        private static void OnRoutedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RoutedEventTrigger trigger || trigger.isAttached == false)
                return;

            trigger.DetachHandler();
            trigger.AttachHandler();
        }


        private static void OnHandledEventsTooChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RoutedEventTrigger trigger || trigger.isAttached == false) 
                return;

            trigger.DetachHandler();
            trigger.AttachHandler();
        }


        /////////////////////////////////////////////////////////////////////////////


        private void AttachHandler()
        {
            /// هدف این متد:
            ///   اتصال یک هندلر (delegate) به رویدادِ RoutedEvent روی عنصر Source.
            ///   یعنی وقتی این رویداد روی Source اجرا شد، متد OnRoutedEvent ما هم صدا زده شود.
            ///
            /// سه حالت زیر باعث می‌شوند که این متد هیچ کاری انجام ندهد:
            ///
            ///   ۱) RoutedEvent == null → هنوز کاربر توی XAML مقدارش را مشخص نکرده.
            ///   ۲) Source == null → این Behavior هنوز به هیچ عنصری متصل نشده.
            ///   ۳) _handler != null → قبلاً یک بار هندلر ساخته و متصل شده.
            ///      این شرط جلوی subscribe تکراری را می‌گیرد.
            ///      اگر دو بار subscribe بشویم، رویداد دو بار fire می‌شود و
            ///      کامند داخل Behavior هم دو بار اجرا می‌شود.
            if (this.RoutedEvent is null || this.Source is null || this.handler is not null)
                return;

            /// مرحله ۱: گرفتن اطلاعات متد OnRoutedEvent از طریق Reflection
            ///
            /// چرا Reflection؟ چون می‌خواهیم اطلاعات متد (MethodInfo) را در زمان اجرا
            /// به دست بیاوریم تا بعداً به Delegate.CreateDelegate تحویلش بدهیم.
            /// بدون Reflection، فقط می‌توانستیم مستقیم بنویسیم this.OnRoutedEvent که
            /// نوع delegate را کامپایلر انتخاب می‌کرد (مشکلش را در پایین توضیح می‌دهم).
            ///
            /// پارامترهای GetMethod:
            ///
            ///   • nameof(OnRoutedEvent) → نام متد به صورت رشته، ولی کامپایل‌تایم.
            ///     یعنی اگر فردا اسم متد را عوض کنی، این هم خودکار عوض می‌شود و
            ///     خطای runtime نمی‌گیری (برخلاف وقتی که رشته‌ی خام "OnRoutedEvent" بنویسی).
            ///
            ///   • BindingFlags.NonPublic → متد OnRoutedEvent ما private است، پس باید
            ///     صریحاً بگوییم که در متدهای غیرعمومی (private/protected) هم بگردد.
            ///     اگر این flag را ندهیم، GetMethod مقدار null برمی‌گرداند چون
            ///     به‌صورت پیش‌فرض فقط متدهای public را جستجو می‌کند.
            ///
            ///   • BindingFlags.Instance → متد OnRoutedEvent یک متد instance است
            ///     (نه static). پس باید بگوییم که دنبال متدهای instance بگردد.
            ///     ترکیب این دو flag یعنی: «دنبال یک متد instance و غیرعمومی با نام
            ///     OnRoutedEvent بگرد».
            ///
            ///   • ! در آخر → عملگر null-forgiving. به کامپایلر می‌گوییم
            ///     «می‌دانم که این ممکن است از نظر کامپایلر null باشد، ولی من
            ///     مطمئنم که null نمی‌شود چون خودم این متد را در همین کلاس نوشته‌ام».
            ///     این عملگر روی رفتار runtime هیچ تأثیری ندارد و صرفاً برای
            ///     ساکت کردن هشدار nullable است.
            ///
            /// نتیجه: methodInfo یک شیء از نوع MethodInfo است که اطلاعات متد ما را
            ///        در خود دارد (اسم، پارامترها، نوع return، و ...). توجه کن که
            ///        این «خودِ متد» نیست، بلکه یک توضیح‌دهنده است که بعداً
            ///        Delegate.CreateDelegate ازش استفاده می‌کند.
            MethodInfo methodInfo = typeof(RoutedEventTrigger).GetMethod(nameof(OnRoutedEvent), BindingFlags.NonPublic | BindingFlags.Instance)!;
            try
            {
                /// مرحله ۲: ساختن یک delegate از نوع دقیقِ موردنیاز رویداد
                ///
                /// چرا این مرحله لازم است؟
                ///
                /// هر RoutedEvent در WPF یک نوع delegate مخصوص به خود دارد که در
                /// خصیصه HandlerType آن ذخیره شده است. مثلاً:
                ///
                ///   • Binding.SourceUpdatedEvent    → DataTransferEventHandler
                ///   • Mouse.MouseDownEvent          → MouseButtonEventHandler
                ///   • Keyboard.KeyDownEvent         → KeyEventHandler
                ///   • ButtonBase.ClickEvent         → RoutedEventHandler
                ///
                /// متد AddHandler (که در انتها صدا می‌زنیم) در زمان اجرا چک می‌کند
                /// که نوع delegate ورودی، دقیقاً همان HandlerType رویداد باشد.
                /// اگر نباشد، ArgumentException می‌دهد.
                ///
                /// حالا اگر بخواهیم به‌جای Delegate.CreateDelegate از روش ساده زیر
                /// استفاده کنیم:
                ///
                ///     Source.AddHandler(RoutedEvent, this.OnRoutedEvent, false);
                ///
                /// کامپایلر C# از روی امضای متد OnRoutedEvent (که پارامتر دومش
                /// RoutedEventArgs است) یک RoutedEventHandler می‌سازد. این
                /// RoutedEventHandler فقط با رویدادهایی سازگار است که HandlerType
                /// آن‌ها RoutedEventHandler باشد (مثل Click). برای SourceUpdated
                /// که HandlerType آن DataTransferEventHandler است، این کار
                /// ArgumentException می‌دهد.
                ///
                /// راه‌حل: به‌جای اینکه نوع delegate را به کامپایلر بسپاریم، خودمان
                /// در زمان اجرا نوع دقیق را از خاصیت HandlerType می‌خوانیم و
                /// با Delegate.CreateDelegate یک delegate دقیقاً از همان نوع
                /// می‌سازیم که به متد OnRoutedEvent ما اشاره کند.
                ///
                /// پارامترهای Delegate.CreateDelegate:
                ///
                ///   • RoutedEvent.HandlerType
                ///     → نوع delegate موردنیاز. مثلاً برای SourceUpdatedEvent
                ///       برابر است با typeof(DataTransferEventHandler).
                ///
                ///   • this
                ///     → شیء هدف. یعنی delegate ساخته‌شده وقتی اجرا شود، متد
                ///       را روی همین شیء (this = RoutedEventTrigger) صدا می‌زند.
                ///       به این کار در اصطلاح «target» کردن delegate می‌گویند.
                ///
                ///   • methodInfo
                ///     → اطلاعات متدی که delegate باید صدا بزند (مرحله قبل
                ///       به دستش آوردیم).
                ///
                /// ═══════════════════════════════════════════════════════════════════
                /// نکته‌ی ظریف: Contravariance پارامترها
                /// ═══════════════════════════════════════════════════════════════════
                ///
                /// امضای متد OnRoutedEvent ما این است:
                ///
                ///     private void OnRoutedEvent(object? sender, RoutedEventArgs args)
                ///
                /// ولی delegate نوع DataTransferEventHandler انتظار پارامتر دومی
                /// از نوع DataTransferEventArgs دارد:
                ///
                ///     public delegate void DataTransferEventHandler(
                ///         object sender, DataTransferEventArgs e);
                ///
                /// از آنجایی که DataTransferEventArgs از RoutedEventArgs ارث‌بری
                /// می‌کند (DataTransferEventArgs : RoutedEventArgs)، پس هر
                /// DataTransferEventArgs یک RoutedEventArgs هم هست. بنابراین متد
                /// ما می‌تواند DataTransferEventArgs را هم بگیرد (چون آن را
                /// به‌عنوان RoutedEventArgs می‌بیند).
                ///
                /// در delegateها، پارامترها می‌توانند contravariant باشند؛ یعنی
                /// متدی که پارامتر عمومی‌تر (پایه‌تر) می‌گیرد، می‌تواند به delegateای
                /// که پارامتر خاص‌تر (مشتق‌تر) می‌گیرد متصل شود.
                ///
                /// دقیقاً به همین دلیل است که Delegate.CreateDelegate بدون خطا
                /// کار می‌کند و یک delegate از نوع DataTransferEventHandler می‌سازد
                /// که پشت صحنه متد ما را که پارامتر RoutedEventArgs می‌گیرد
                /// صدا می‌زند.
                ///
                /// نتیجه‌ی نهایی: یک متد واحد (OnRoutedEvent) به هر تعداد رویداد
                /// Routed با هر نوع delegateی وصل می‌شود، بدون نیاز به نوشتن
                /// ده‌ها overload جداگانه.
                this.handler = Delegate.CreateDelegate(this.RoutedEvent.HandlerType, this, methodInfo);
            }
            catch (ArgumentException ex)
            {
                /// اگر Delegate.CreateDelegate به هر دلیلی شکست بخورد، یک
                /// ArgumentException پرتاب می‌کند. دلایل احتمالی:
                ///
                ///   • امضای متد OnRoutedEvent با نوع delegate سازگار نبوده
                ///     (مثلاً نوع پارامترها یا تعدادشان با هم نمی‌خواند).
                ///
                ///   • پارامتر دوم متد ما (RoutedEventArgs) از نوعی نیست که
                ///     delegate انتظار دارد یا قابل تبدیل به آن نباشد
                ///     (مثلاً اگر رویداد args کاملاً متفاوتی داشته باشد).
                ///
                ///   • متد OnRoutedEvent یک متد instance است ولی delegate
                ///     احتیاج به static دارد (که در اینجا این مورد نیست).
                ///
                /// در اینجا ما ArgumentException را در یک InvalidOperationException
                /// با پیام فارسی و واضح‌تر می‌پیچیم، چون پیام پیش‌فرض
                /// ArgumentException خیلی فنی و غیرقابل‌فهم است. با این کار،
                /// اگر فردا رویداد جدیدی وصل کردی و سازگار نبود، سریع می‌فهمی
                /// که مشکل کجاست. همچنین ex اصلی (InnerException) هم حفظ می‌شود
                /// تا stack trace اصلی از بین نرود.
                string errorString = $"امکان اتصال به RoutedEvent «{RoutedEvent.Name}» با نوع Handler " +
                                     $"«{RoutedEvent.HandlerType.FullName}» وجود ندارد. \nمطمئن شوید که امضای OnRoutedEvent با پارامترهای رویداد سازگار است.";
                throw new InvalidOperationException(errorString, ex);
            }
            /// مرحله ۳: اتصال هندلر به رویداد روی عنصر Source
            ///
            /// حالا که _handler را از نوع دقیقاً درست ساختیم، آن را به رویداد
            /// متصل می‌کنیم:
            ///
            ///   • RoutedEvent → رویدادی که می‌خواهیم به آن گوش بدهیم.
            ///   • _handler    → delegateی که ساخته‌ایم (از نوع HandlerType رویداد).
            ///   • HandledEventsToo → یک خصیصه که می‌توان از XAML ست کرد.
            ///     اگر false باشد (پیش‌فرض):
            ///       اگر رویداد قبلاً توسط یک کنترل دیگر (مثلاً داخل خود TextBox)
            ///       با Handled = true علامت‌گذاری شده باشد، دیگر هندلر ما
            ///       اجرا نمی‌شود.
            ///     اگر true باشد:
            ///       حتی اگر رویداد Handled شده باشد، هندلر ما هم اجرا می‌شود.
            ///
            /// این خط، در واقع «قلب» کلاس RoutedEventTrigger است. بقیه‌ی کد
            /// فقط برای این بوده که به اینجا برسیم و _handler درست را بسازیم.
            Source.AddHandler(this.RoutedEvent, this.handler, this.HandledEventsToo);
        }


        private void DetachHandler()
        {
            if (this.handler is null || this.RoutedEvent is null) 
                return;

            /// Source ممکن است در زمان OnDetaching هنوز مقدار داشته باشد.
            /// اگر null باشد، RemoveHandler خطا می‌دهد؛ پس guard می‌کنیم.
            if (this.Source is not null)
            {
                this.Source.RemoveHandler(this.RoutedEvent, this.handler);
            }
            this.handler = null;
        }


        /// <summary>
        /// امضای این متد باید با تمام RoutedEventHandlerهای WPF سازگار باشد.
        /// همه‌ی آن‌ها پارامتر دومشان از RoutedEventArgs مشتق می‌شود.
        /// Delegate.CreateDelegate این را با contravariance قبول می‌کند.
        /// </summary>
        private void OnRoutedEvent(object? sender, RoutedEventArgs args)
        {
            if (this.MarkAsHandled == true)
                args.Handled = true;

            base.OnEvent(args);
        }


        protected override string GetEventName()
        {
            /// EventTriggerBase از این متد برای پیدا کردن CLR Event با Reflection
            /// استفاده می‌کند. چون خودمان مستقیم AddHandler می‌زنیم، نیازی به آن نیست.
            /// برگرداندن رشته‌ی خالی باعث می‌شود base از این کار صرف‌نظر کند.
            return string.Empty;
        }

    }
}
