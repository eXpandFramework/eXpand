using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.MasterDetail.Security.Improved{
    public class PopulateChildListViewController:PopulateController<IContextMasterDetailRule>{
        protected override string GetPredefinedValues(IModelMember wrapper){
            var contextMasterDetailRule = ((IContextMasterDetailRule) View.CurrentObject);
            if (contextMasterDetailRule.CollectionMember != null){
                var listElementType = contextMasterDetailRule.CollectionMember.MemberInfo.ListElementType;
                var modelClass = Application.Model.BOModel.GetClass(listElementType);
                var modelListViews = Application.Model.Views.OfType<IModelListView>().Where(listView => listView.ModelClass==modelClass).Select(listView => listView.Id);
                return string.Join(";", modelListViews);
            }
            return null;
        }

        protected override IEnumerable<string> RefreshingProperties(){
            var contextMasterDetailRule = ((IContextMasterDetailRule)View.CurrentObject);
            return new[] { contextMasterDetailRule.GetPropertyName(rule => rule.TypeInfo),contextMasterDetailRule.GetPropertyName(rule => rule.CollectionMember) };
        }

        protected override Expression<Func<IContextMasterDetailRule, object>> GetPropertyName(){
            return rule => rule.ChildListView;
        }
    }
}