﻿using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Observers {
    public class SerializationConfigurationObserver : ObjectObserver<ISerializationConfiguration> {
        bool _serializing;

        public SerializationConfigurationObserver(IObjectSpace objectSpace)
            : base(objectSpace) {
        }

        protected override void OnChanged(ObjectChangedEventArgs<ISerializationConfiguration> e) {
            base.OnChanged(e);
            ISerializationConfiguration serializationConfiguration = e.Object;
            if (e.PropertyName == serializationConfiguration.GetPropertyName(x => x.TypeToSerialize) && e.NewValue != null) {
                if (!_serializing) {
                    _serializing = true;
                    ObjectSpace.Delete(serializationConfiguration.SerializationGraph);
                    //serializationConfiguration.
                        //.Session.Delete(serializationConfiguration.SerializationGraph);
                    new ClassInfoGraphNodeBuilder().Generate(serializationConfiguration);
                    _serializing = false;
                }
            }
        }
    }
}