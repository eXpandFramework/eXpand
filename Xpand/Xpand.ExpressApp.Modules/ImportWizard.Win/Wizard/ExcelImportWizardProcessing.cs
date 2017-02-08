using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ImportWizard.Core;
using Xpand.ExpressApp.ImportWizard.Win.Properties;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Threading;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard{
    public partial class ExcelImportWizard{
        public static int TransactionSize = 100;
        public void ProccesExcellRows(XPObjectSpace objectSpace, Row[] rows, Type type, int? headerRows,  CancellationToken token, IProgress<string> progress) {
            
            var records = rows;
            int i = 0;
            var props = objectSpace.FindXPClassInfo(type).PersistentProperties.OfType<XPMemberInfo>().ToList();
            string keyPropertyName = objectSpace.GetKeyPropertyName(type);
            foreach (Row excelRow in records){
                ++i;
                if (i <= headerRows) continue;
                token.ThrowIfCancellationRequested();
                string message;
                ProcessSingleRow(objectSpace, type, keyPropertyName, excelRow, props, i, out message,progress.Report);
                if (i% TransactionSize==0) {
                    objectSpace.CommitChanges();
                    progress.Report(string.Format(Resources.SuccessProcessingRecord, i - 1));
                }
            }
            progress.Report(string.Format(Resources.SuccessProcessingRecord, i - 1));
            objectSpace.CommitChanges();
        }

        private void ProcessSingleRow(XPObjectSpace objectSpace, Type type, string keyPropertyName, Row excelRow, List<XPMemberInfo> props, int i, out string message, Action<string> notify){
            IXPSimpleObject newObj = GetExistingOrCreateNewObject(objectSpace, keyPropertyName, excelRow, type);
            message = null;
            if (newObj == null){
                message = string.Format(Resources.newObjectError, i);
                return;
            }
            foreach (Mapping mapping in ImportMap.Mappings){

                XPMemberInfo prop = props.Single(p => p.Name == mapping.MapedTo);

                try{
                    Cell val = excelRow[mapping.Column];

                    if (val != null)
                        _propertyValueMapper(objectSpace, prop, val.Value, ref newObj);
                }

                catch (Exception ee){
                    message = string.Format(Resources.ErrorProcessingRecord,
                        i - 1, ee);
                    notify(message);
                }

            }

            objectSpace.Session.Save(newObj);
            
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