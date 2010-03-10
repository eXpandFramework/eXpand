using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.MemberLevelSecurity.Win.PropertyEditors;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Controllers
{
    public partial class ChangePropertyEditorsController : ViewController
    {
        public ChangePropertyEditorsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            var applicationNodeWrapper = new ApplicationNodeWrapper(dictionary);
            IEnumerable<ClassInfoNodeWrapper> classInfoNodeWrappers =
                applicationNodeWrapper.BOModel.Classes.Where(cls =>typeof (XPBaseObject).IsAssignableFrom(cls.ClassTypeInfo.Type));
            foreach (ClassInfoNodeWrapper classInfoNodeWrapper in classInfoNodeWrappers){
                foreach (PropertyInfoNodeWrapper propertyInfoNodeWrapper in from property in classInfoNodeWrapper.Properties
                                                                       let detailViewItemInfoNodeWrapper =new DetailViewItemInfoNodeWrapper(property.Node)
                                                                       let isSimpleStringEdit =(property.Type != typeof(string).FullName ||(detailViewItemInfoNodeWrapper.RowCount == 0))
                                                                       let isComboStringEdit =(isSimpleStringEdit &&!string.IsNullOrEmpty(detailViewItemInfoNodeWrapper.PredefindedValues))
                                                                       where property.PropertyEditorType ==typeof(StringPropertyEditor).FullName &&isSimpleStringEdit && !isComboStringEdit
                                                                       select property)
                    propertyInfoNodeWrapper.PropertyEditorType = typeof(MemberLevelSecurityStringPropertyEditor).FullName;
            }
        }

    }
}


