using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssemblyInfo
    {
        string Name { get; set; }
        IList<IPersistentClassInfo> PersistentClassInfos { get; }
        string CompileErrors { get; set; }
        bool DoNotCompile { get; set; }
        CodeDomProvider CodeDomProvider { get; }
        Session Session { get; }
        IFileData FileData { get; set; }
        int CompileOrder { get; set; }
        IList<IPersistentAssemblyAttributeInfo> Attributes { get; }
    }
    public enum CodeDomProvider {
        CSharp,
        VB
    }
}