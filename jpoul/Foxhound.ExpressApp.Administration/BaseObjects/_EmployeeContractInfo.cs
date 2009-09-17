using System.Reflection;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace Foxhound.ExpressApp.Administration.BaseObjects{
    public class _EmployeeContractInfo : BaseObjectInfo{

        private string contractTerminationReason;
        [Size(SizeAttribute.Unlimited)]
        public string ContractTerminationReason
        {
            get
            {
                return contractTerminationReason;
            }
            set
            {
                SetPropertyValue("ContractTerminationReason", ref contractTerminationReason, value);
            }
        }
    }
}