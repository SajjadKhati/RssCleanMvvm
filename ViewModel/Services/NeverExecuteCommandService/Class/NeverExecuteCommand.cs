using System.Windows.Input;

namespace ViewModel.Services.NeverExecuteCommandService.Class
{
    public class NeverExecuteCommand : ICommand
    {

        public static ICommand Instance { get; } = new NeverExecuteCommand();


        ///////////////////////////////////////////////////////


        /// <summary>
        /// متد سازنده اش private هست چون لازم نیست کسی شی ای ازش بسازه .
        /// شی ، از طریق پروپرتیِ استاتیکِ Instance دریافت میشه .
        /// </summary>
        private NeverExecuteCommand() { }

        /// ← هیچ‌وقت fire نمی‌شه. accessors خالی، بدون warning و با قصد صریح.
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => false;

        /// <summary>
        /// از اونجایی که متد CanExecute ، همیشه false را برمیگردونه ، پس هیچ وقت قرار نیست که این متدِ Execute اجرا بشه که نیاز به بدنه داشته باشه .
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(object? parameter) => 
            throw new NotImplementedException("NeverExecuteCommand هرگز نباید اجرا شود؛ CanExecute آن همیشه false است.");

    }
}
