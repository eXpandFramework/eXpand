using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentTypeInfo : IXPSimpleObject, INotifyPropertyChanged{
        string Name { get; set; }
        IList<IPersistentAttributeInfo> TypeAttributes { get; }
        IList<ITemplateInfo> TemplateInfos { get; }
    }
}