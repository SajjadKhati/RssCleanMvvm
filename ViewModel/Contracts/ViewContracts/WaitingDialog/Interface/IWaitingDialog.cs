namespace ViewModel.Contracts.ViewContracts.WaitingDialog.Interface
{
    /// <summary>
    /// قرارداد سرویسی که منطق را در حین نمایش پنجره‌ی انتظار اجرا می‌کند.
    ///
    /// قرارداد: تا اتمام منطق (یا خطا)، پنجره‌ی انتظار نمایش داده می‌شود؛
    /// سپس بسته می‌شود. این عملیات، از دید caller، synchronous است
    /// (caller تا بسته‌شدن پنجره برنمی‌گردد) — چون ShowDialog بلاک‌کننده است.
    /// </summary>
    public interface IWaitingDialog
    {
        /// <summary>
        /// پنجره‌ی انتظار modal را نمایش می‌دهد و در حین آن، logicFuncAsync را اجرا می‌کند.
        /// پس از اتمام (یا خطای) منطق، پنجره بسته می‌شود.
        /// </summary>
        /// <param name="logicFuncAsync"></param>
        void RunLogicInWaitingDialog(Func<Task> logicFuncAsync);
    }
}
