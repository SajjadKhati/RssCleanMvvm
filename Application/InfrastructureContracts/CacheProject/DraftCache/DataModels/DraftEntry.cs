namespace Application.InfrastructureContracts.CacheProject.DraftCache.DataModels
{
    public class DraftEntry<TItem>
    {

        public TItem Item { get; set; }

        public DraftState DraftState { get; set; }

        public object Tag {  get; set; }

    }
}
