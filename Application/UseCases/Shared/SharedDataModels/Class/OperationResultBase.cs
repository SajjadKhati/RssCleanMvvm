namespace Application.UseCases.Shared.SharedDataModels.Class
{
    public class OperationResultBase
    {

        public IList<Exception> CreationErrors { get; init; }

        public bool WasUserCancelled { get; init; }


        ////////////////////////////////////////////////////////////////


        public OperationResultBase()
        {

        }


        public OperationResultBase(IList<Exception> creationErrors, bool wasUserCancelled)
        {
            this.CreationErrors = creationErrors;
            this.WasUserCancelled = wasUserCancelled;
        }

    }
}
