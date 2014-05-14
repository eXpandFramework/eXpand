using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{

    [DataContract]
    public class TypePermission : ITypePermissionOperations
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public bool AllowCreate
        {
            get;
            set;
        }

        [DataMember]
        public bool AllowDelete
        {
            get;
            set;
        }

        [DataMember]
        public bool AllowNavigate
        {
            get;
            set;
        }

        [DataMember]
        public bool AllowRead
        {
            get;
            set;
        }

        [DataMember]
        public bool AllowWrite
        {
            get;
            set;
        }


        [DataMember]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public string TypeName { get; set; }

        public Type TargetType
        {
            get { return !string.IsNullOrWhiteSpace(TypeName) ?  Type.GetType(TypeName) : null; }
            set { TypeName = value !=null ? value.AssemblyQualifiedName : null;}
        }

    }
}
