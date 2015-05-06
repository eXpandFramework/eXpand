using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;

namespace XpandAddins{
    [Guid("F71B6036-80F1-4F08-BC59-B5D92865F629")]
    public interface IFullReference {
        // Reference        
        [DispId(1)]
        DTE DTE { get; }
        [DispId(2)]
        References Collection { get; }
        [DispId(3)]
        Project ContainingProject { get; }
        [DispId(4)]
        void Remove();
        [DispId(5)]
        string Name { get; }
        [DispId(6)]
        prjReferenceType Type { get; }
        [DispId(7)]
        string Identity { get; }
        [DispId(8)]
        string Path { get; }
        [DispId(9)]
        string Description { get; }
        [DispId(10)]
        string Culture { get; }
        [DispId(11)]
        int MajorVersion { get; }
        [DispId(12)]
        int MinorVersion { get; }
        [DispId(13)]
        int RevisionNumber { get; }
        [DispId(14)]
        int BuildNumber { get; }
        [DispId(15)]
        bool StrongName { get; }
        [DispId(16)]
        Project SourceProject { get; }
        [DispId(17)]
        bool CopyLocal { get; set; }
        [DispId(18), TypeLibFunc(1088)]
        dynamic get_Extender(string extenderName);
        [DispId(19)]
        dynamic ExtenderNames { get; }
        [DispId(20)]
        string ExtenderCATID { get; }
        [DispId(21)]
        string PublicKeyToken { get; }
        [DispId(22)]
        string Version { get; }
        // Reference2        
        [DispId(100)]
        string RuntimeVersion { get; }
        // Reference3       
        [DispId(120)]
        bool SpecificVersion { get; set; }
        [DispId(121)]
        string SubType { get; set; }
        [DispId(122)]
        bool Isolated { get; set; }
        [DispId(123)]
        string Aliases { get; set; }
        [DispId(124)]
        uint RefType { get; }
        [DispId(125)]
        bool AutoReferenced { get; }
        [DispId(126)]
        bool Resolved { get; }
        // Reference4       
        [DispId(127)]
        bool EmbedInteropTypes { get; set; }
    }
}