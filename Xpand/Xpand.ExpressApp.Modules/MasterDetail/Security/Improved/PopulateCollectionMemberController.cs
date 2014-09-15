using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.MasterDetail.Security.Improved {
    public class PopulateCollectionMemberController : PopulateController<IContextMasterDetailRule> {
        protected override string GetPredefinedValues(IModelMember wrapper){
            var members = ((IContextMasterDetailRule) View.CurrentObject).TypeInfo.Members.Where(info 
                => info.IsList&&info.ListElementTypeInfo.IsDomainComponent).Select(info => info.Name);
            return string.Join(";", members);
        }

        protected override IEnumerable<string> RefreshingProperties() {
            return new[] { ((IContextMasterDetailRule)View.CurrentObject).GetPropertyName(rule => rule.TypeInfo) };
        }

        protected override Expression<Func<IContextMasterDetailRule, object>> GetPropertyName(){
            return rule => rule.CollectionMember;
        }
    }
}
