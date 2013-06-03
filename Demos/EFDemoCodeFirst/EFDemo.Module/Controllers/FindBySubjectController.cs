using System;

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace EFDemo.Module.Controllers {
	public partial class FindBySubjectController : ViewController {
        private void FindBySubjectAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            string paramValue = e.ParameterCurrentValue as string;
            if(!string.IsNullOrEmpty(paramValue)) {
                paramValue = "%" + paramValue + "%";
            }
            object obj = objectSpace.FindObject(((ListView)View).ObjectTypeInfo.Type,
                new BinaryOperator("Subject", paramValue, BinaryOperatorType.Like));
            if(obj != null) {
                e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, obj);
            }
        }
        public FindBySubjectController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
