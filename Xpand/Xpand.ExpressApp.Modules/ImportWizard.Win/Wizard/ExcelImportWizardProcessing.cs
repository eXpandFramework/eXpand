using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ImportWizard.Core;
using Xpand.ExpressApp.ImportWizard.Win.Properties;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard{
    public partial class ExcelImportWizard{
        public void ProccesExcellRows( XPObjectSpace objectSpace, DoWorkEventArgs e, Type type){
            var workerArgs = ((WorkerArgs) e.Argument);
            var records = workerArgs.Rows;
            int i = 0;
            var props = objectSpace.FindXPClassInfo(type).PersistentProperties.OfType<XPMemberInfo>().ToList();
            string keyPropertyName = objectSpace.GetKeyPropertyName(type);
            foreach (Row excelRow in records){
                ++i;
                if (i <= workerArgs.HeaderRows) continue;
                if (_bgWorker.CancellationPending){
                    e.Cancel = true;
                    break;
                }


                string message;

                ProcessSingleRow(objectSpace, e, type, keyPropertyName, excelRow, props, i, out message);

                if (i%50 == 0)
                    objectSpace.CommitChanges();

                _bgWorker.ReportProgress(1, message);
                Application.DoEvents();
            }
            objectSpace.CommitChanges();
        }

        private void ProcessSingleRow(XPObjectSpace objectSpace, DoWorkEventArgs e, Type type, string keyPropertyName,
            Row excelRow, List<XPMemberInfo> props, int i, out string message){
            IXPSimpleObject newObj = GetExistingOrCreateNewObject(objectSpace, keyPropertyName, excelRow, type);

            if (newObj == null){
                message = string.Format(Resources.newObjectError, i);
                return;
            }
            foreach (Mapping mapping in ImportMap.Mappings){
                if (_bgWorker.CancellationPending){
                    e.Cancel = true;
                    break;
                }
                Application.DoEvents();

                XPMemberInfo prop = props.Single(p => p.Name == mapping.MapedTo);

                try{
                    Cell val = excelRow[mapping.Column];

                    if (val != null)
                        _propertyValueMapper(objectSpace, prop, val.Value, ref newObj);
                }

                catch (Exception ee){
                    message = string.Format(Resources.ErrorProcessingRecord,
                        i - 1, ee);
                    _bgWorker.ReportProgress(0, message);
                }

                if (CurrentCollectionSource != null)
                    AddNewObjectToCollectionSource(CurrentCollectionSource, newObj, ObjectSpace);
            }

            objectSpace.Session.Save(newObj);
            message = string.Format(Resources.SuccessProcessingRecord, i - 1);
        }

        private IXPSimpleObject GetExistingOrCreateNewObject(XPObjectSpace objectSpace, string keyPropertyName,
            Row excelRow, Type type){
            Mapping idMapping = ImportMap.Mappings.SingleOrDefault(p => p.MapedTo == keyPropertyName);
            IXPSimpleObject newObj = null;
            if (idMapping != null && ImportUtils.GetQString(excelRow[idMapping.Column].Value) != string.Empty){
                try{
                    //find existing object
                    Cell val = excelRow[idMapping.Column];
                    var gwid = new Guid(ImportUtils.GetQString(val.Value));
                    newObj =
                        objectSpace.FindObject(type, new BinaryOperator(keyPropertyName, gwid), true) as IXPSimpleObject;
                }
                catch{
                }
            }
            return newObj ?? (objectSpace.CreateObject(type) as IXPSimpleObject);
        }
    }
}