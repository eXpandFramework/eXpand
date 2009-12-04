using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator
{
    public class GenerateCodeController : ViewController
    {
        readonly List<IPersistentTypeInfo> _deletedPersistentTypeInfos=new List<IPersistentTypeInfo>();

        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
            ObjectSpace.ObjectDeleting+=ObjectSpaceOnObjectDeleted;
            ObjectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            foreach (IPersistentTypeInfo deletedPersistentTypeInfo in _deletedPersistentTypeInfos) {
                GenerateCode(deletedPersistentTypeInfo);
            }
            _deletedPersistentTypeInfos.Clear();
            if (ObjectSpace.IsModified)
                ObjectSpace.CommitChanges();
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {

            _deletedPersistentTypeInfos.AddRange(objectsManipulatingEventArgs.Objects.OfType<IPersistentAttributeInfo>().Select(attributeInfo => attributeInfo.Owner));
            _deletedPersistentTypeInfos.AddRange(objectsManipulatingEventArgs.Objects.OfType<IPersistentMemberInfo>().Select(memberInfo => memberInfo.Owner).Cast<IPersistentTypeInfo>());            
            _deletedPersistentTypeInfos.AddRange(objectsManipulatingEventArgs.Objects.OfType<IntermediateObject>().Select(memberInfo =>((IPersistentTypeInfo)memberInfo.RightIntermediateObjectField)).ToList());            
        }

        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs objectManipulatingEventArgs)
        {
            object o = objectManipulatingEventArgs.Object;
            if (((PersistentBase) o).IsDeleted)
                return;
            GenerateCode(o);
        }

        public void GenerateCode(object o) {

            if (o is IPersistentAttributeInfo) {
                attributeCodeGeneration(o);
            }
            else if (o is IPersistentMemberInfo) {
                memberCodeGeneration(o);
            }
            else if (o is IPersistentClassInfo) {
                classGeneration(o);
            }
            else if (o is IntermediateObject)
                classGeneration(((IntermediateObject) o).RightIntermediateObjectField);
        }

        void classGeneration(object o)
        {
            var persistentClassInfo = (IPersistentClassInfo)o;
            persistentClassInfo.GeneratedCode = CodeEngine.GenerateCode(persistentClassInfo);
        }

        void memberCodeGeneration(object o)
        {
            var persistentMemberInfo = (IPersistentMemberInfo)o;
            persistentMemberInfo.GeneratedCode = CodeEngine.GenerateCode(persistentMemberInfo);
            if (persistentMemberInfo.Owner != null)
                persistentMemberInfo.Owner.GeneratedCode = CodeEngine.GenerateCode(persistentMemberInfo.Owner);
        }

        void attributeCodeGeneration(object o)
        {
            IPersistentTypeInfo owner = ((IPersistentAttributeInfo)o).Owner;
            if (owner is IPersistentMemberInfo){
                IPersistentClassInfo persistentClassInfo = ((IPersistentMemberInfo)owner).Owner;
                if (persistentClassInfo != null)
                    persistentClassInfo.GeneratedCode = CodeEngine.GenerateCode(persistentClassInfo);
                owner.GeneratedCode = CodeEngine.GenerateCode((IPersistentMemberInfo)owner);
            }
            else 
                owner.GeneratedCode = CodeEngine.GenerateCode((IPersistentClassInfo)owner);
        }
    }
}