using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentAssemblyInfo {
        Guid Oid { get; set; }
        string Name { get; set; }
        IList<IPersistentClassInfo> PersistentClassInfos { get; }
        string Errors { get; set; }
        bool DoNotCompile { get; set; }
        CodeDomProvider CodeDomProvider { get; }
        Session Session { get; }
        IFileData StrongKeyFileData { get; set; }
        int CompileOrder { get; set; }
        IList<IPersistentAssemblyAttributeInfo> Attributes { get; }
        int Revision { get; set; }
        string GeneratedCode { get; }
    }
    public enum CodeDomProvider {
        CSharp
    }
}