using System.Collections.Generic;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssemblyInfo
    {
        string Name { get; set; }
        IList<IPersistentClassInfo> PersistentClassInfos { get; }
        string CompileErrors { get; set; }
        bool DoNotCompile { get; set; }
        CodeDomProvider CodeDomProvider { get; }
    }
    public enum CodeDomProvider {
        CSharp,
        VB,
        JScript
    }
}