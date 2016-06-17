using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation {
    [CodeRule]
    public class RuleClassInfoMerge:RuleBase<IPersistentClassInfo>{
        public const string ErrorMessage = "Please use a PersistentMapInheritance attribute to map with the parent table";

        public RuleClassInfoMerge() : base("", "Save") { }

        public RuleClassInfoMerge(IRuleBaseProperties properties) : base(properties){
            properties.CustomMessageTemplate = ErrorMessage;
        }

        public override ReadOnlyCollection<string> UsedProperties{
            get{
                IPersistentClassInfo persistentClassInfo;
                return new ReadOnlyCollection<string>(new[] { nameof(persistentClassInfo.TypeAttributes) });
            }
        }

        protected override bool IsValidInternal(IPersistentClassInfo target, out string errorMessageTemplate){
            errorMessageTemplate = ErrorMessage;
            return string.IsNullOrEmpty(target.MergedObjectFullName) ||
                   target.TypeAttributes.OfType<IPersistentMapInheritanceAttribute>()
                       .Any(attribute => attribute.MapInheritanceType == MapInheritanceType.ParentTable);
        }
    }
}
