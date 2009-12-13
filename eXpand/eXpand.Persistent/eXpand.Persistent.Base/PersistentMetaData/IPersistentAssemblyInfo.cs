using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssemblyInfo
    {
        string Name { get; set; }
        IList<IPersistentClassInfo> PersistentClassInfos { get; }
        string CompileErrors { get; set; }
        bool DoNotCompile { get; set; }
        CodeDomProvider CodeDomProvider { get; }
        Session Session { get; }
        IFileData FileData { get; set; }
        string Version { get; set; }
    }
    public enum CodeDomProvider {
        CSharp,
        VB
    }
}