namespace View.WinFormLibrary.RssCleanMvvm.GlobalResource.UiResource.Class
{
    /// <summary>
    /// معادلِ سبک‌ها/قالب‌های ظاهریِ نسخه‌ی WPF (پوشه‌ی GlobalResource) در نسخه‌ی WinForms .
    ///
    /// طبق درخواست، «تمپلیت» نسخه‌ی WPF بازسازی نشده است ؛ بلکه یک پالت ساده و روشن
    /// (هم‌خانواده با رنگ‌های تم روشنِ WPF) در یک جا متمرکز شده تا :
    ///   • رنگ‌ها و فونت‌ها پراکنده و سخت‌کد نشوند .
    ///   • تغییر ظاهر، فقط با تغییر این فایل ممکن باشد .
    ///
    /// چون تغییر تم در نسخه‌ی WinForms وجود ندارد ، این مقادیر ثابت (static readonly) هستند .
    /// </summary>
    public static class UiTheme
    {
        // ===== رنگ‌ها =====

        public static readonly Color WindowBackground = Color.FromArgb(255, 247, 249, 252);

        public static readonly Color Surface = Color.FromArgb(255, 255, 255, 255);

        public static readonly Color ContentBackground = Color.FromArgb(255, 247, 249, 252);

        public static readonly Color TextPrimary = Color.FromArgb(255, 31, 41, 55);

        public static readonly Color TextSecondary = Color.FromArgb(255, 107, 114, 128);

        /// <summary>رنگ تأکید (هم‌خانواده با DeepSkyBlue تم روشن WPF).</summary>
        public static readonly Color Accent = Color.FromArgb(255, 0, 145, 234);

        public static readonly Color AccentHover = Color.FromArgb(255, 0, 120, 212);

        public static readonly Color AccentPressed = Color.FromArgb(255, 0, 90, 170);

        public static readonly Color AccentSoft = Color.FromArgb(255, 232, 244, 255);

        public static readonly Color Border = Color.FromArgb(255, 226, 232, 240);

        public static readonly Color BorderStrong = Color.FromArgb(255, 203, 213, 225);

        public static readonly Color DisabledText = Color.FromArgb(255, 156, 163, 175);

        public static readonly Color DisabledBorder = Color.FromArgb(255, 213, 213, 224);

        public static readonly Color DisabledBackground = Color.FromArgb(255, 249, 250, 251);

        /// <summary>رنگ پیام وضعیت (معادل #D26900 در WPF).</summary>
        public static readonly Color StatusText = Color.FromArgb(255, 210, 105, 0);

        /// <summary>رنگ آیتمِ ذخیره‌نشده (معادل Brushes.Red در converter نسخه‌ی WPF).</summary>
        public static readonly Color UnsavedBorder = Color.FromArgb(255, 220, 53, 69);

        public static readonly Color SavedBorder = Color.FromArgb(255, 227, 230, 234);

        public static readonly Color ToggleCheckedBorder = Color.FromArgb(255, 255, 140, 0);

        public static readonly Color ToggleCheckedBackground = Color.FromArgb(255, 255, 247, 237);

        public static readonly Color ToggleCheckedText = Color.FromArgb(255, 179, 92, 0);

        public static readonly Color ImagePlaceholder = Color.FromArgb(255, 243, 245, 249);

        public static readonly Color Spinner = Color.FromArgb(255, 0, 145, 234);

        // ===== فونت‌ها =====

        public const string FontFamilyName = "Segoe UI";

        public static readonly Font BaseFont = new(FontFamilyName, 10F, FontStyle.Regular, GraphicsUnit.Point);

        public static readonly Font TitleFont = new(FontFamilyName, 12.5F, FontStyle.Bold, GraphicsUnit.Point);

        public static readonly Font CategoryFont = new(FontFamilyName, 9.5F, FontStyle.Regular, GraphicsUnit.Point);

        public static readonly Font DateFont = new(FontFamilyName, 9F, FontStyle.Regular, GraphicsUnit.Point);

        public static readonly Font ButtonFont = new(FontFamilyName, 9.5F, FontStyle.Regular, GraphicsUnit.Point);

        public static readonly Font HeaderFont = new(FontFamilyName, 10F, FontStyle.Bold, GraphicsUnit.Point);

        public static readonly Font StatusFont = new(FontFamilyName, 11F, FontStyle.Bold, GraphicsUnit.Point);

        // ===== ابعاد =====

        public const int CornerRadius = 6;

        public const int ButtonHeight = 30;

        public const int ItemImageWidth = 192;

        public const int ItemImageHeight = 108;

        public const int ItemActionColumnWidth = 168;
    }
}
