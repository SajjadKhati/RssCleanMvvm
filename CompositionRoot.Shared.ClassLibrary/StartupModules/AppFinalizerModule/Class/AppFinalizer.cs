namespace CompositionRoot.Shared.ClassLibrary.StartupModules.AppFinalizerModule.Class
{
    public class AppFinalizer : IDisposable
    {

        private IList<IDisposable> disposables;


        ////////////////////////////////////////////////////////////////////////


        public AppFinalizer(IList<IDisposable> disposables)
        {
            this.disposables = disposables;
        }


        ////////////////////////////////////////////////////////////////////////


        public void Dispose()
        {
            foreach (IDisposable disposable in this.disposables)
            {
                disposable.Dispose();
            }
            this.disposables = null;
        }

    }
}
