namespace eXpand.Persistent.Base.Taxonomies{
    public interface IValueTerm : IBaseTerm {
        IStructuralTerm StructuralTerm { get; set; }
    }
}