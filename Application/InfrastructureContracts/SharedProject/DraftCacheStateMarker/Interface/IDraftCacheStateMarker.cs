namespace Application.InfrastructureContracts.SharedProject.DraftCacheStateMarker.Interface
{
    public interface IDraftCacheStateMarker<in TKey, in TItem>
    {

        void MarkAsAddedInDraftCache(TKey key, TItem item);

        void MarkAsModifiedInDraftCache(TKey key, TItem item);

        void MarkAsDeletedInDraftCache(TKey key, TItem item);

    }
}
