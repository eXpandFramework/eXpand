using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.MemberLevelSecurity.Win.PropertyEditors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Controllers
{
    public partial class ChangePropertyEditorsController : ViewController
    {
        public ChangePropertyEditorsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        
        public override void UpdateModel(IModelApplication applicationModel)
        {
            base.UpdateModel(applicationModel);
            return;
            IEnumerable<IModelClass> modelClasses =
                applicationModel.BOModel.Where(cls => typeof(XPBaseObject).IsAssignableFrom(cls.TypeInfo.Type));

            foreach (IModelClass modelClass in modelClasses){
                foreach (IModelMember modelMember in from property in modelClass.AllMembers
                                                                       let isSimpleStringEdit = (property.Type != typeof(string) || (property.RowCount == 0))
                                                                       let isComboStringEdit = (isSimpleStringEdit && !string.IsNullOrEmpty(property.PredefinedValues))
                                                                       where property.PropertyEditorType == typeof(StringPropertyEditor) && isSimpleStringEdit && !isComboStringEdit
                                                                       select property)
                    modelMember.PropertyEditorType = typeof(MemberLevelSecurityStringPropertyEditor);
            }
        }

    }
}


