using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Validation;
using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
    public class HCategory : IHCategory {
		public HCategory() {
			Children = new List<HCategory>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		public HCategory Parent { get; set; }
		public virtual IList<HCategory> Children { get; set; }

		[NotMapped, Browsable(false), RuleFromBoolProperty("HCategoryCircularReferences", DefaultContexts.Save, "Circular refrerence detected. To correct this error, set the Parent property to another value.", UsedProperties = "Parent")]
        public Boolean IsValid {
            get {
                HCategory currentObj = Parent;
                while(currentObj != null) {
                    if(currentObj == this) {
                        return false;
                    }
                    currentObj = currentObj.Parent;
                }
                return true;
            }
        }

        IBindingList ITreeNode.Children {
            get { return ((IListSource)Children).GetList() as IBindingList; }
        }
        ITreeNode IHCategory.Parent {
            get { return Parent as IHCategory; }
            set { Parent = value as HCategory; }
        }
        ITreeNode ITreeNode.Parent {
            get { return Parent as ITreeNode; }
        }
    }
}
