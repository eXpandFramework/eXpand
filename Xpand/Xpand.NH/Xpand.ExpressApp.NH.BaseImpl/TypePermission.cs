using DevExpress.ExpressApp.Security;
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
    public class TypePermission : ITypePermission
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
            get { return !string.IsNullOrWhiteSpace(TypeName) ? Type.GetType(TypeName) : null; }
            set { TypeName = value != null ? value.AssemblyQualifiedName : null; }
        }

        public IList<IOperationPermission> GetPermissions()
        {
            List<IOperationPermission> result = new List<IOperationPermission>();
            if (TargetType != null)
            {
                if (AllowRead)
                {
                    result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Read));
                }
                if (AllowWrite)
                {
                    result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Write));
                }
                if (AllowCreate)
                {
                    result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Create));
                }
                if (AllowDelete)
                {
                    result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Delete));
                }
                if (AllowNavigate)
                {
                    result.Add(new TypeOperationPermission(TargetType, SecurityOperations.Navigate));
                }
            }
            result.AddRange(ObjectPermissions.SelectMany(op => op.GetPermissions()));
            return result;
        }

        private List<ObjectPermission> objectPermissions;

        [DataMember]
        public IList<ObjectPermission> ObjectPermissions
        {
            get
            {
                if (objectPermissions == null)
                {
                    objectPermissions = new List<ObjectPermission>();
                }

                return objectPermissions;
            }
        }

        [DataMember]
        public Role Owner { get; set; }
    }
}
