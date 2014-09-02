using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH
{
    public class NHNestedObjectSpace : NHObjectSpace, INestedObjectSpace
    {

        private readonly NHObjectSpace parentObjectSpace;

        internal NHNestedObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore, IPersistenceManager persistenceManager, 
            Dictionary<object, ObjectSpaceInstanceInfo> instances,  NHObjectSpace parentObjectSpace) : 
            base(typesInfo, entityStore, persistenceManager, instances, null)
        {

            this.parentObjectSpace = parentObjectSpace;
        }

        protected override void DoCommit()
        {
        }


        public override object ReloadObject(object obj)
        {
            return obj;
        }

        public IObjectSpace ParentObjectSpace
        {
            get { return parentObjectSpace; }
        }
    }
}
