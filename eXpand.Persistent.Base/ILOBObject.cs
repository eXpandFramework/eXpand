namespace eXpand.Persistent.Base.Interfaces {
    public interface ILOBObject {
        bool IsImportedInLOB { get; set; }
        bool IsUsedByLOB { get; set; }
    }
}
