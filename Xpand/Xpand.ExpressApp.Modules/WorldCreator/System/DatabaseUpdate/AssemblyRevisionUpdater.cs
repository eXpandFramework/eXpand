using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            foreach (var persistentAssemblyInfo in objectSpace.GetModifiedPersistentAssemblies()){
                RaiseRevision(objectSpace, persistentAssemblyInfo);
            }
        }

        private void RaiseRevision(IObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo){
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
