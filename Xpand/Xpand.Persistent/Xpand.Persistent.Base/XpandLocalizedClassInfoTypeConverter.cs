using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base {
    public class XpandLocalizedClassInfoTypeConverter : LocalizedClassInfoTypeConverter {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            var typeInfos = XafTypesInfo.Instance.FindTypeInfo(typeof(PersistentBase)).Descendants;
            var list = (from typeInfo in typeInfos where typeInfo.Type != null select typeInfo.Type).ToList();
            list.Sort(this);
            list.Insert(0, null);
            return new StandardValuesCollection(list);
        }
    }
}