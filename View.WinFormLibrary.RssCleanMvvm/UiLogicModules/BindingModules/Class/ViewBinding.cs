using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace View.WinFormLibrary.RssCleanMvvm.UiLogicModules.BindingModules.Class
{
    /// <summary>
    /// لایه‌ی بایندینگِ کمکی برای WinForms .
    ///
    /// ────────────────────────────────────────────────────────────────────────────────
    /// چرا این کلاس وجود دارد؟
    /// ────────────────────────────────────────────────────────────────────────────────
    /// WPF این‌ها را به‌صورت داخلی دارد :
    ///   • Binding به یک Property از ViewModel (با INotifyPropertyChanged)
    ///   • Command Binding (اجرای Command + هم‌گام‌سازی Enabled با CanExecute)
    ///   • Binding به Collection (با INotifyCollectionChanged)
    ///
    /// WinForms فقط مورد اول را (آن هم با DataBindings و برای propertyهای ساده) دارد .
    /// Command Binding و Collection Binding و «بایندینگ به یک مقدار محاسبه‌شده» را ندارد .
    ///
    /// پس به‌جای اتصال مستقیمِ View به ViewModel (مثل فراخوانی دستیِ متدها)، این چند متد
    /// همان کارِ Binding را انجام می‌دهند و View هیچ منطقی از ViewModel را نمی‌شناسد :
    /// فقط نام Property/Command را می‌دهد و واکنش UI را تعریف می‌کند .
    ///
    /// نکته‌ی مهم: هیچ‌کدام از این متدها به ReactiveUI نیاز ندارند . ViewModel های پروژه
    /// از CommunityToolkit.Mvvm مشتق شده‌اند و <c>IReactiveObject</c> را پیاده‌سازی نمی‌کنند ؛
    /// بنابراین Binding های ReactiveUI به آداپتر نیاز پیدا می‌کردند و سود واقعی نداشتند .
    /// ────────────────────────────────────────────────────────────────────────────────
    /// </summary>
    public static class ViewBinding
    {
        /// <summary>
        /// معادلِ <c>Binding Mode=OneWay</c> در WPF .
        ///
        /// مقدار اولیه را همان لحظه اعمال می‌کند و سپس با هر بار تغییر همان Property، دوباره اعمال می‌کند .
        /// عملِ اعمال، روی نخ UI اجرا می‌شود (چون ممکن است PropertyChanged از نخ دیگری برسد) .
        /// </summary>
        /// <param name="scope">ظرفِ اشتراک‌ها (برای لغو خودکار هنگام Dispose شدن View).</param>
        /// <param name="uiTarget">کنترلی که عمل روی آن اجرا می‌شود (برای مارشال کردن به نخ UI).</param>
        /// <param name="source">ViewModel منبع .</param>
        /// <param name="propertyName">نام دقیق Property (همان چیزی که در OnPropertyChanged فرستاده می‌شود).</param>
        /// <param name="apply">عملی که مقدار را روی کنترل‌ها می‌نویسد .</param>
        public static void BindOneWay<TSource>(this BindingScope scope, Control uiTarget, TSource source,
            string propertyName, Action apply)
            where TSource : INotifyPropertyChanged
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (uiTarget == null)
                throw new ArgumentNullException(nameof(uiTarget));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(propertyName) == true)
                throw new ArgumentException("نام Property نمی‌تواند خالی باشد .", nameof(propertyName));
            if (apply == null)
                throw new ArgumentNullException(nameof(apply));

            UiThread.Post(uiTarget, apply);

            PropertyChangedEventHandler handler = (object? sender, PropertyChangedEventArgs eventArgs) =>
            {
                /// بعضی ViewModel ها (مثل MainWindowViewModel) برای بعضی Propertyها
                /// عمداً همیشه OnPropertyChanged را صدا می‌زنند تا مقدارِ تکراری هم به UI برسد .
                /// پس اینجا مقایسه‌ی مقدار انجام نمی‌دهیم و فقط نام Property را بررسی می‌کنیم .
                if (eventArgs.PropertyName == propertyName)
                    UiThread.Post(uiTarget, apply);
            };

            source.PropertyChanged += handler;
            scope.Add(() => source.PropertyChanged -= handler);
        }


        /// <summary>
        /// معادلِ <c>Binding Mode=OneWay</c> روی یک Collection در WPF (ItemsSource) .
        /// مقدار اولیه را اعمال و سپس با هر تغییر (Add/Remove/Reset) دوباره اعمال می‌کند .
        /// </summary>
        public static void BindCollection(this BindingScope scope, Control uiTarget,
            INotifyCollectionChanged collection, Action apply)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (uiTarget == null)
                throw new ArgumentNullException(nameof(uiTarget));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (apply == null)
                throw new ArgumentNullException(nameof(apply));

            UiThread.Post(uiTarget, apply);

            NotifyCollectionChangedEventHandler handler = (object? sender, NotifyCollectionChangedEventArgs eventArgs) =>
            {
                UiThread.Post(uiTarget, apply);
            };

            collection.CollectionChanged += handler;
            scope.Add(() => collection.CollectionChanged -= handler);
        }


        /// <summary>
        /// معادلِ <c>Binding Mode=OneWay</c> یا <c>TwoWay</c> برای متن (Label/TextBox) .
        ///
        /// این متد از DataBindings خودِ WinForms استفاده می‌کند (نه مکانیزم دستی) چون :
        ///   • WinForms برای propertyهای ساده‌ی متنی، Binding کامل و بالغی دارد .
        ///   • با INotifyPropertyChanged کاملاً کار می‌کند .
        ///   • اعمالِ تغییرات از TextBox به ViewModel را هم خودش انجام می‌دهد .
        /// </summary>
        public static void BindText(this BindingScope scope, Control control, object source, string sourceProperty,
            string formatString = "", bool twoWay = false)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataSourceUpdateMode updateMode = twoWay == true
                ? DataSourceUpdateMode.OnPropertyChanged
                : DataSourceUpdateMode.Never;

            Binding binding = new(nameof(Control.Text), source, sourceProperty, true, updateMode);

            if (string.IsNullOrEmpty(formatString) == false)
                binding.FormatString = formatString;

            scope.AddBinding(control, binding);
        }


        /// <summary>
        /// معادلِ <c>Command Binding</c> در WPF برای دکمه‌ها :
        ///   • Enabled با <see cref="ICommand.CanExecute"/> هم‌گام می‌شود .
        ///   • با تغییر وضعیت CanExecute، Enabled دوباره ارزیابی می‌شود .
        ///   • کلیک، Command را (در صورت CanExecute) اجرا می‌کند .
        /// </summary>
        public static void BindCommand(this BindingScope scope, ButtonBase button, ICommand? command,
            Func<object?>? parameterFactory = null)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (button == null)
                throw new ArgumentNullException(nameof(button));

            void UpdateEnabled()
            {
                bool canExecute = command != null && command.CanExecute(parameterFactory?.Invoke());
                UiThread.Post(button, () => button.Enabled = canExecute);
            }

            EventHandler canExecuteChangedHandler = (object? sender, EventArgs eventArgs) => UpdateEnabled();
            if (command != null)
                command.CanExecuteChanged += canExecuteChangedHandler;

            scope.Add(() =>
            {
                if (command != null)
                    command.CanExecuteChanged -= canExecuteChangedHandler;
            });

            EventHandler clickHandler = (object? sender, EventArgs eventArgs) =>
            {
                object? parameter = parameterFactory?.Invoke();
                if (command != null && command.CanExecute(parameter) == true)
                    command.Execute(parameter);
            };

            /// در WPF اجرای Command را خودِ CommandManager انجام می‌دهد ؛ در WinForms باید
            /// رویداد Click را به Command وصل کنیم . ولی این «اتصال مستقیم» نیست :
            /// View فقط می‌گوید «این دکمه به این Command بایند است» .
            button.Click += clickHandler;
            scope.Add(() => button.Click -= clickHandler);

            UpdateEnabled();
        }


        /// <summary>
        /// همان Command Binding ، ولی برای آیتم‌های MenuStrip (که <see cref="Control"/> نیستند) .
        /// </summary>
        public static void BindCommand(this BindingScope scope, ToolStripItem item, ICommand? command,
            Func<object?>? parameterFactory = null)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            void UpdateEnabled()
            {
                bool canExecute = command != null && command.CanExecute(parameterFactory?.Invoke());

                void Apply()
                {
                    if (item.IsDisposed == false)
                        item.Enabled = canExecute;
                }

                ToolStrip? owner = item.Owner;
                if (owner != null)
                    UiThread.Post(owner, Apply);
                else
                    Apply();
            }

            EventHandler canExecuteChangedHandler = (object? sender, EventArgs eventArgs) => UpdateEnabled();
            if (command != null)
                command.CanExecuteChanged += canExecuteChangedHandler;

            scope.Add(() =>
            {
                if (command != null)
                    command.CanExecuteChanged -= canExecuteChangedHandler;
            });

            EventHandler clickHandler = (object? sender, EventArgs eventArgs) =>
            {
                object? parameter = parameterFactory?.Invoke();
                if (command != null && command.CanExecute(parameter) == true)
                    command.Execute(parameter);
            };

            item.Click += clickHandler;
            scope.Add(() => item.Click -= clickHandler);

            UpdateEnabled();
        }


        /// <summary>
        /// یک <see cref="Binding"/> خام می‌سازد و به scope اضافه می‌کند .
        /// برای موارد خاصی که به Format/Parse سفارشی نیاز دارند .
        /// </summary>
        public static Binding Bind(this BindingScope scope, Control control, string targetProperty, object source,
            string sourceProperty, DataSourceUpdateMode updateMode = DataSourceUpdateMode.Never)
        {
            Binding binding = new(targetProperty, source, sourceProperty, true, updateMode);
            scope.AddBinding(control, binding);
            return binding;
        }

    }
}
