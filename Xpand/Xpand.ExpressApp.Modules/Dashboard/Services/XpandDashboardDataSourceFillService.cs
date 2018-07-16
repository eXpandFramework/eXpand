using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard.Services{
    public class XpandDashboardDataSourceFillService{
        public event EventHandler<ObjectDataSourceCreatingArgs> DatasourceCreating; 
        public event EventHandler<HandledEventArgs> LoadBeforeParameters;

        public  object CreateDataSource(IObjectSpace objectSpace, Type targetType, ObjectDataSourceFillParameters objectDataSourceFillParameters,Func<(IObjectSpace objectSpace,Type targetType,ObjectDataSourceFillParameters objectDataSourceFillParameters),object> baseCall) {
            var args = new HandledEventArgs();
            OnLoadBeforeParameters(args);
            if (args.Handled){
                if (objectDataSourceFillParameters.Parameters.Any()){
                    OnDatasourceCreating(new ObjectDataSourceCreatingArgs(targetType));
                    var dataSource = baseCall((objectSpace, targetType, objectDataSourceFillParameters));
                    return dataSource;                    
                }

                return null;
            }
            OnDatasourceCreating(new ObjectDataSourceCreatingArgs(targetType));
            return baseCall((objectSpace, targetType, objectDataSourceFillParameters));
        }

        protected virtual void OnDatasourceCreating(ObjectDataSourceCreatingArgs e){
            DatasourceCreating?.Invoke(this, e);
        }

        protected virtual void OnLoadBeforeParameters(HandledEventArgs e){
            LoadBeforeParameters?.Invoke(this, e);
        }
    }
}