using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.SystemModule.Search;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;
using FilterEditorControl = Xpand.ExpressApp.Win.Editors.FilterEditorControl;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof(string), EditorAliases.CriteriaPropertyEditorEx,false)]
    public class CriteriaPropertyEditorEx : CriteriaPropertyEditor,IComplexViewItem {
        private XafApplication _application;
        private IObjectSpace _objectSpace;

        public CriteriaPropertyEditorEx(Type objectType, IModelMemberViewItem model) : base(objectType, model){

        }

        protected override object CreateControlCore(){
            var helper=new CriteriaPropertyEditorHelper(MemberInfo);
            

            var controlHelper = new FilterEditorControlHelper(_application, _objectSpace);
            var filterControl = new FilterEditorControl(() =>{
                var criteriaObjectType = helper.GetCriteriaObjectType(CurrentObject);
                return Model.Application.BOModel.GetClass(criteriaObjectType).AllMembers.Cast<IModelMemberFullTextContains>();
            });
            controlHelper.Attach(filterControl);
            filterControl.AllowCreateDefaultClause = false;
            filterControl.FilterChanged += (sender, args) => OnControlValueChanged();
            filterControl.FilterTextChanged += (sender, args) => OnControlValueChanged();
            return filterControl;
        }

        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application){
            Setup(objectSpace, application);
            _application = application;
            _objectSpace = objectSpace;
        }
    }
}
