using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH
{
    public class NHObjectSpaceProvider : IObjectSpaceProvider
    {
        private readonly ITypesInfo typesInfo;
        private readonly IPersistenceManager persistenceManager;
        public NHObjectSpaceProvider(ITypesInfo typesInfo, IPersistenceManager persistenceManager)
        {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            Guard.ArgumentNotNull(persistenceManager, "persistenceManager");
            this.typesInfo = typesInfo;
            this.persistenceManager = persistenceManager;
        }

        public string ConnectionString { get; set; }
        public IObjectSpace CreateObjectSpace()
        {
            return new NHObjectSpace(typesInfo, EntityStore, persistenceManager);
        }

        public IObjectSpace CreateUpdatingObjectSpace(bool allowUpdateSchema)
        {
            throw new NotImplementedException();
        }

        private IEntityStore entityStore;
        public DevExpress.ExpressApp.DC.IEntityStore EntityStore
        {
            get
            {
                if (entityStore == null)
                    entityStore = new NHEntityStore(typesInfo, persistenceManager);

                return entityStore;
            }
        }

        public Type ModuleInfoType
        {
            get { return null; }
        }

        public DevExpress.ExpressApp.DC.ITypesInfo TypesInfo
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateSchema()
        {
            throw new NotImplementedException();
        }
    }
}
