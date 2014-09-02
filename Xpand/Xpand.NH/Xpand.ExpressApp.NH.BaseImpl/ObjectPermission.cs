using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    [DataContract]
    public class ObjectPermission
    {

        [DataMember]
        public Guid Id { get; set; }

        public IList<IOperationPermission> GetPermissions()
        {
            IList<IOperationPermission> result = new List<IOperationPermission>();
            if (Owner != null & Owner.TargetType != null)
            {
                if (AllowRead)
                {
                    result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Read));
                }
                if (AllowWrite)
                {
                    result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Write));
                }
                if (AllowDelete)
                {
                    result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Delete));
                }
                if (AllowNavigate)
                {
                    result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Navigate));
                }
            }
            return result;
        }

        [DataMember]
        [CriteriaOptions("Owner.TargetType")]
        [VisibleInListView(true)]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        public string Criteria
        {
            get;
            set;

        }

        private bool allowRead;

        [DataMember]
        public bool AllowRead
        {
            get { return allowRead; }
            set { allowRead = value; }
        }
        
        [DataMember]
        public bool AllowWrite
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
        public TypePermission Owner { get; set; }
    }
}
