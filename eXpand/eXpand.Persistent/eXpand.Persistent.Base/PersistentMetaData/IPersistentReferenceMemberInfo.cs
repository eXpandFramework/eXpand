<<<<<<< HEAD
using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentMemberInfo
    {
        Type ReferenceType { get; set; }
        string ReferenceTypeAssemblyQualifiedName { get; set; }
=======
namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentReferenceMemberInfo : IPersistentMemberInfo
    {
        string ReferenceTypeFullName { get; set; }
>>>>>>> CodeDomApproachForWorldCreator
    }
}