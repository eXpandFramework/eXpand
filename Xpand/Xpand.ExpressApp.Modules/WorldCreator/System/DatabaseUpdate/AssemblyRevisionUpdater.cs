using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.System.DatabaseUpdate {
    public class AssemblyRevisionUpdater {
        private readonly Dictionary<IObjectSpace, List<IPersistentAssemblyInfo>> _persistentAssemblyInfos=new Dictionary<IObjectSpace, List<IPersistentAssemblyInfo>>();

        public void Attach(IObjectSpace objectSpace){
            objectSpace.Committing+=ObjectSpaceOnCommitting;
            objectSpace.Disposed+=ObjectSpaceOnDisposed;
        }

        private void ObjectSpaceOnDisposed(object sender, EventArgs eventArgs){
            var objectSpace = ((IObjectSpace) sender);
            _persistentAssemblyInfos.Remove(objectSpace);
            objectSpace.Disposed-=ObjectSpaceOnDisposed;
            objectSpace.Committing-=ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            var objectSpace = ((IObjectSpace) sender);
            foreach (var result in GetModifiedObjectsForSave<IPersistentAssemblyInfo>(objectSpace)) {
                RaiseRevision(objectSpace,result);
            }
            foreach (var result in GetModifiedObjectsForSave<IPersistentClassInfo>(objectSpace)) {
                RaiseRevision(objectSpace, result);
            }
            foreach (var result in GetModifiedObjectsForSave<IPersistentMemberInfo>(objectSpace)) {
                RaiseRevision(objectSpace, result);
            }
        }

        private IEnumerable<T> GetModifiedObjectsForSave<T>(IObjectSpace objectSpace){
            return objectSpace.ModifiedObjects.OfType<T>().Where(info => !objectSpace.IsDeletedObject(info));
        }

        private void RaiseRevision(IObjectSpace objectSpace, object obj){
            var persistentAssemblyInfo = obj as IPersistentAssemblyInfo;
            if (persistentAssemblyInfo != null) {
                RaiseRevisionCore(objectSpace,persistentAssemblyInfo);
                return;
            }
            var persistentClassInfo = obj as IPersistentClassInfo;
            if (persistentClassInfo != null) {
                RaiseRevisionCore(objectSpace, persistentClassInfo.PersistentAssemblyInfo);
            }
            var persistentMemberInfo = obj as IPersistentMemberInfo;
            if (persistentMemberInfo != null && persistentMemberInfo.Owner != null)
                RaiseRevisionCore(objectSpace, persistentMemberInfo.Owner.PersistentAssemblyInfo);
        }

        private void RaiseRevisionCore(IObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo){
            if (persistentAssemblyInfo==null)
                return;
            if (!_persistentAssemblyInfos.ContainsKey(objectSpace)){
                _persistentAssemblyInfos.Add(objectSpace, new List<IPersistentAssemblyInfo>());
            }
            if (!_persistentAssemblyInfos[objectSpace].Contains(persistentAssemblyInfo))
                persistentAssemblyInfo.Revision++;
        }
    }
}
