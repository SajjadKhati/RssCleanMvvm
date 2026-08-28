namespace Application.InfrastructureContracts.CacheProject.CacheBase.DraftCache.DataModels
{
    public class DraftEntry<TItem>
    {

        public TItem Item { get; set; }

        public DraftState DraftState { get; set; }

        public object Tag {  get; set; }


        ///////////////////////////////////////////////////////////////


        public DraftEntry()
        {
        }

        public DraftEntry(TItem item)
        {
            this.Item = item;
        }

        public DraftEntry(TItem item, DraftState draftState) : this(item)
        {
            this.Item = item;
            this.DraftState = draftState;
        }

        public DraftEntry(TItem item, DraftState draftState, object tag) : this(item, draftState)
        {
            this.Tag = tag;
        }

    }
}
