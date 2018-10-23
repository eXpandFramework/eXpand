using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class ExcelColumnMapMemberTypeValueController:ObjectViewController<ListView,ExcelColumnMapMemberTypeValue> {

        public ExcelColumnMapMemberTypeValueController(){
            TargetViewNesting=Nesting.Nested;
        }

        protected IEnumerable<Type> GetTypes(){
            if (ExcelColumnMap != null){
                var propertyType = ExcelColumnMap.PropertyType;
                var types = new[]{propertyType}
                    .Concat(propertyType.GetTypeInfo().Descendants.Select(info => info.Type))
                    .Where(type => !type.IsAbstract);
                return types;
            }
            return Enumerable.Empty<Type>();
        }

        protected override void OnActivated() {
            base.OnActivated();
            ((PropertyCollectionSource) View.CollectionSource).MasterObjectChanged+=OnMasterObjectChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ((PropertyCollectionSource) View.CollectionSource).MasterObjectChanged-=OnMasterObjectChanged;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            PopulateTypes();
        }

        protected virtual void PopulateTypes() {
            
        }

        protected ExcelColumnMap ExcelColumnMap => ((ExcelColumnMap) ((PropertyCollectionSource) View.CollectionSource).MasterObject);

        protected virtual void OnMasterObjectChanged(object sender, EventArgs e) {
            View.AllowEdit[nameof(ExcelColumnMapMemberTypeValueController)] = ExcelColumnMap.IsPersistentBO;
            View.AllowDelete[nameof(ExcelColumnMapMemberTypeValueController)] = ExcelColumnMap.IsPersistentBO;
            View.AllowNew[nameof(ExcelColumnMapMemberTypeValueController)] = ExcelColumnMap.IsPersistentBO;
            PopulateTypes();
        }

    }
}