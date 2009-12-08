namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentClassInfoScript {
        IPersistentClassInfo PersistentClassInfo { get; set; }
        ScriptType ScriptType { get; set; }
        string Script { get; set; }
    }
}