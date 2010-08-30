using DevExpress.Xpo.Metadata.Helpers;

namespace eXpand.Xpo
{
    public interface ISupportChangedMembers
    {
        ChangedMemberCollector ChangedMemberCollector { get; }
    }
}
