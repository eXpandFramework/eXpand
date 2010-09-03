using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata.Helpers;
using eXpand.ExpressApp;
using eXpand.Xpo.DB;
using System.Linq;

namespace FeatureCenter.Module.Miscellaneous.UpdateOnlyChangeFields {
    public class UpdateStatementController:ViewController<DetailView> {
        SqlDataStoreProxy _sqlDataStoreProxy;
        SimpleAction _saveAction;

        public UpdateStatementController() {
            TargetObjectType = typeof (UOCFCustomer);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            _sqlDataStoreProxy = ((IXpandObjectSpaceProvider)Application.ObjectSpaceProvider).DataStoreProvider.Proxy;
            _saveAction = Frame.GetController<DetailViewController>().SaveAction;
            _saveAction.Executing+=SaveActionOnExecuting;
            _saveAction.Executed+=SaveActionOnExecuted;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            _saveAction.Executing -= SaveActionOnExecuting;
            _saveAction.Executed -= SaveActionOnExecuted;
        }
        void SaveActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _sqlDataStoreProxy.DataStoreModifyData -= SqlDataStoreProxyOnDataStoreModifyData;
        }

        void SaveActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {

            _sqlDataStoreProxy.DataStoreModifyData += SqlDataStoreProxyOnDataStoreModifyData;
        }

        void SqlDataStoreProxyOnDataStoreModifyData(object sender, DataStoreModifyDataEventArgs dataStoreModifyDataEventArgs) {
            var updateStatement = (UpdateStatement) dataStoreModifyDataEventArgs.ModificationStatements[0];
            List<QueryOperand> queryOperands = updateStatement.Operands.OfType<QueryOperand>().Where(value => value.ColumnName!=GCRecordField.StaticName&&value.ColumnName!=OptimisticLockingAttribute.DefaultFieldName).ToList();
            string s = "";
            for (int i = 0; i < queryOperands.Count; i++) {
                s += queryOperands[i].ColumnName+"="+ updateStatement.Parameters[i].Value +" AND ";
            }
            s = s.TrimEnd(" AND ".ToCharArray());
            
            ((UOCFCustomer)View.CurrentObject).ModificationStatements = "THIS IS COMES FROM THE DATALAYER :---->"+s;
        }
    }
}