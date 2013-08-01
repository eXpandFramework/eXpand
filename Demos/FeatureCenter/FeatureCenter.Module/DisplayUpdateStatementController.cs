using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Persistent.Base.General;
using Xpand.Xpo.DB;

namespace FeatureCenter.Module {
    public class DisplayUpdateStatementController : ViewController<DetailView> {
        DataStoreProxy _dataStoreProxy;
        SimpleAction _saveAction;

        public DisplayUpdateStatementController() {
            TargetObjectType = typeof(ISupportModificationStatements);
        }
        protected override void OnActivated() {
            base.OnActivated();
            _dataStoreProxy = ((IXpandObjectSpaceProvider)Application.ObjectSpaceProvider).DataStoreProvider.Proxy;
            _saveAction = Frame.GetController<ModificationsController>().SaveAction;
            _saveAction.Executing += SaveActionOnExecuting;
            _saveAction.Executed += SaveActionOnExecuted;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            _saveAction.Executing -= SaveActionOnExecuting;
            _saveAction.Executed -= SaveActionOnExecuted;
        }
        void SaveActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _dataStoreProxy.DataStoreModifyData -= DataStoreProxyOnDataStoreModifyData;
        }

        void SaveActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {

            _dataStoreProxy.DataStoreModifyData += DataStoreProxyOnDataStoreModifyData;
        }

        void DataStoreProxyOnDataStoreModifyData(object sender, DataStoreModifyDataEventArgs dataStoreModifyDataEventArgs) {
            var modificationStatement = dataStoreModifyDataEventArgs.ModificationStatements[0];
            List<QueryOperand> queryOperands = modificationStatement.Operands.OfType<QueryOperand>().Where(value => value.ColumnName != GCRecordField.StaticName && value.ColumnName != OptimisticLockingAttribute.DefaultFieldName).ToList();

            ((ISupportModificationStatements)View.CurrentObject).ModificationStatements = "THIS IS COMES FROM THE DATALAYER :---->" + (modificationStatement is UpdateStatement ? GetUpdateMessage((UpdateStatement)modificationStatement, queryOperands) : GetInsertMessage((InsertStatement)modificationStatement, queryOperands));
        }

        string GetInsertMessage(InsertStatement insertStatement, List<QueryOperand> queryOperands) {
            string s = "";
            for (int i = 0; i < queryOperands.Count; i++) {
                s += queryOperands[i].ColumnName + "=" + insertStatement.Parameters[i].Value + " AND ";
            }
            s = s.TrimEnd(" AND ".ToCharArray());
            return s;
        }

        string GetUpdateMessage(UpdateStatement updateStatement, List<QueryOperand> queryOperands) {
            string s = "";
            for (int i = 0; i < queryOperands.Count; i++) {
                s += queryOperands[i].ColumnName + "=" + updateStatement.Parameters[i].Value + " AND ";
            }
            s = s.TrimEnd(" AND ".ToCharArray());
            return s;
        }
    }
}