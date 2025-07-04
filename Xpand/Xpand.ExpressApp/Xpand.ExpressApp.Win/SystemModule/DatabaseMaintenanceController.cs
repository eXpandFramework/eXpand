using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomFunctions;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Helpers;
using Timer = System.Timers.Timer;
#pragma warning disable CS0618 // Type or member is obsolete

namespace Xpand.ExpressApp.Win.SystemModule {

    public enum FileGrowthMode{
        Megabytes,
        Percent
    }

    public class ModelAutogrowthReadOnlyCalculator: IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName){
            return IsReadOnly(node);
        }

        public bool IsReadOnly(IModelNode node, IModelNode childNode){
            return IsReadOnly(node);
        }

        private static bool IsReadOnly(IModelNode node){
            var enabled = ((IModelAutogrowth) node).Enabled;
            return enabled.HasValue && !enabled.Value;
        }
    }

    public interface IModelAutogrowth:IModelNode{
        [Description("Disable AutoGrowth to gain in performance, but make sure you adjust your initial size")]
        [RefreshProperties(RefreshProperties.All)]
        bool? Enabled{ get; set; }
        [Category("FileGrowth")]
        [ModelReadOnly(typeof(ModelAutogrowthReadOnlyCalculator))]
        FileGrowthMode FileGrowthMode { get; set; }
        [RuleRange(0, 100, TargetCriteria = nameof(FileGrowthMode) + "='" + nameof(FileGrowthMode.Percent) + "'")]
        [Category("FileGrowth")]
        [ModelReadOnly(typeof(ModelAutogrowthReadOnlyCalculator))]
        int FileGrowth { get; set; }
        [Category("Maximum File Size")]
        [ModelReadOnly(typeof(ModelAutogrowthReadOnlyCalculator))]
        MaximumFileSizeMode MaximumFileSizeMode{ get; set; }
        [Description("In MB")]
        [ModelReadOnly(typeof(ModelAutogrowthReadOnlyCalculator))]
        [Category("Maximum File Size")]
        int MaximumFileSize{ get; set; }
    }

    public enum MaximumFileSizeMode{
        Unlimited,
        Limited
    }

    public interface IModelDatabaseMaintanance:IModelNodeEnabled{
        [Description("In MB")]
        int? InitialSize { get; set; }
        IModelAutogrowth Autogrowth{ get; }
        [DefaultValue(10)]
        int BackupsToKeep{ get; set; }
        [Description("Backups minimize the transaction log size and imporoves performance")]
        TimeSpan? BackupInterval{ get; set; }
        [Description("Use SIMPLE if you do not care about a time specific recovery, also this mode will keep your database size relatively small")]
        RecoveryModel? RecoveryModel{ get; set; }
        [DataSourceProperty(nameof(UserClasses))]
        [Category("User")]
        IModelClass UserModelClass { get; set; }

        [CriteriaOptions(nameof(UserModelClass)+".TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        [DefaultValue(EvaluateCSharpOperator.OperatorName +"('new Regex(\".*\").IsMatch(Environment.MachineName)')")]
        [Category("User")]
        string UserCriteria { get; set; }

        [Browsable(false)]
        IModelList<IModelClass> UserClasses { get; }
        [DefaultValue(600)]
        [Description("SQL Command timeout in sec")]
        int BackupTimeout{ get; set; }
        string BackupDirectory{ get; set; }
        
    }

    [DomainLogic(typeof(IModelDatabaseMaintanance))]
    public static class DatabaseMaintenanceDomainLogic{
        public static IModelClass Get_UserModelClass(IModelDatabaseMaintanance databaseMaintanance) {
            return databaseMaintanance.ModelClasses(typeof(ISecurityUser)).FirstOrDefault();
        }
        public static IModelList<IModelClass> Get_UserClasses(IModelDatabaseMaintanance databaseMaintanance){
            return databaseMaintanance.ModelClasses(typeof(ISecurityUser));
        }

    }
    public interface IModelOptionsDatabaseMaintanence{
        IModelDatabaseMaintanance DatabaseMaintanance{ get; }
    }
    public enum RecoveryModel{
        FULL,
        SIMPLE
    }

    public class DatabaseMaintenanceController:WindowController,IModelExtender {
        Timer _timer;

        public DatabaseMaintenanceController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated(){
            base.OnActivated();
            var databaseMaintanance = ((IModelOptionsDatabaseMaintanence) Application.Model.Options).DatabaseMaintanance;
            var supported = Application.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>().Any(provider =>
                provider.DataLayer is BaseDataLayer baseDataLayer &&baseDataLayer.ConnectionProvider is ConnectionProviderSql);
            if (!supported) {
                return;
            }
            Task.Factory.StartNew(() => {
                try{
                    Execute(databaseMaintanance, (session, database) => {
                        Update(databaseMaintanance.RecoveryModel, database);
                        Update(databaseMaintanance.Autogrowth, database);
                        Update(databaseMaintanance.InitialSize, session, database);
                    });
                }
                catch (Exception e){
                    Tracing.Tracer.LogError(e);
                }
            },TaskCreationOptions.LongRunning);
        }

        private void Update(int? initialSize, Session session, string database){
            if (initialSize.HasValue){
                string text= "sp_spaceused";
                var result = session.ExecuteSproc(text).ResultSet[0].Rows.First().Values[1].ToString();
                var size = Convert.ToDouble(result.Substring(0, result.IndexOf(" ", StringComparison.Ordinal)));
                if (size<initialSize.Value){
                    text = $"ALTER DATABASE [{database}] MODIFY FILE ( NAME = N'{database}', SIZE = {initialSize.Value.Convert(SystemExtensions.SizeDefinition.Megabyte,SystemExtensions.SizeDefinition.Kilobyte )}KB )";
                    ExecuteNonQuery(text);
                }
            }
        }

        private void ExecuteNonQuery(string text,int timeout=30){
            var parser = new ConnectionStringParser(XpandModuleBase.ConnectionString);
            parser.RemovePartByName("xpodatastorepool");
            using (var connection = new SqlConnection(parser.GetConnectionString())){
                connection.Open();
                using (var dbCommand = connection.CreateCommand()){
                    dbCommand.CommandTimeout = timeout;
                    dbCommand.CommandText = text;
                    dbCommand.ExecuteNonQuery();
                }
            }

            Tracing.Tracer.LogValue(GetType().Namespace,text);
        }

        private void Update(IModelAutogrowth autogrowth, string database){
            var autogrowthEnabled = autogrowth.Enabled;
            if (autogrowthEnabled.HasValue){
                string fileGrowth = ", FILEGROWTH =";
                if (!autogrowthEnabled.Value)
                    fileGrowth += "0";
                else{
                    fileGrowth += autogrowth.FileGrowth.ToString();
                    if (autogrowth.FileGrowthMode == FileGrowthMode.Percent)
                        fileGrowth += "%";
                    if (autogrowth.MaximumFileSizeMode == MaximumFileSizeMode.Limited)
                        fileGrowth +=$", MAXSIZE = {autogrowth.MaximumFileSize.Convert(SystemExtensions.SizeDefinition.Megabyte, SystemExtensions.SizeDefinition.Kilobyte)}KB";
                }
                ExecuteNonQuery($"ALTER DATABASE [{database}] MODIFY FILE ( NAME = N'{database}' {fileGrowth})");
            }
        }

        private  void Update(RecoveryModel? recoveryModel, string database){
            if (recoveryModel.HasValue){
                string text =$"ALTER DATABASE [{database}] SET RECOVERY {recoveryModel} WITH NO_WAIT";
                ExecuteNonQuery(text);
            }
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Frame.Context==TemplateContext.ApplicationWindowContextName){
                var databaseMaintanance = ((IModelOptionsDatabaseMaintanence)Application.Model.Options).DatabaseMaintanance;
                var backupInterval = databaseMaintanance.BackupInterval;
                if (backupInterval.HasValue&&databaseMaintanance.NodeEnabled){
                    Frame.TemplateChanged+=FrameOnTemplateChanged;
                    Frame.Disposing+=FrameOnDisposing;
                }
            }
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            Frame.TemplateChanged-=FrameOnTemplateChanged;
            _timer=new Timer(10000) {SynchronizingObject = (ISynchronizeInvoke) Frame.Template,AutoReset = true};
            _timer.Elapsed+=TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs){
            _timer.Stop();
            Backup();
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing-=FrameOnDisposing;
            _timer.Stop();
        }

        private void Backup(){
            var databaseMaintanance = ((IModelOptionsDatabaseMaintanence)Application.Model.Options).DatabaseMaintanance;

            try{
                
                Execute(databaseMaintanance, (session, database) => {
                    if (!Directory.Exists(databaseMaintanance.BackupDirectory))
                        Directory.CreateDirectory(databaseMaintanance.BackupDirectory);
                    var backupFiles = Directory.GetFiles(databaseMaintanance.BackupDirectory,$"{database}*.bak").OrderByDescending(s => new FileInfo(s).CreationTime).ToArray();
                    if (!backupFiles.Any()){
                        BackupTask(database, databaseMaintanance).ContinueWith(task => _timer.Start());
                    }
                    else{
                        var creationTime = new FileInfo(backupFiles.First()).CreationTime;
                        var lastBackTime = DateTime.Now-creationTime;
                        if (lastBackTime>databaseMaintanance.BackupInterval){
                            BackupTask(database, databaseMaintanance).ContinueWith(task => {
                                foreach (var file in backupFiles.Skip(databaseMaintanance.BackupsToKeep).ToArray()) {
                                    File.Delete(file);
                                }
                                _timer.Start();
                            });
                        }
                        else{
                            _timer.Start();
                        }
                    }
                });
            }
            catch (Exception e){
                Tracing.Tracer.LogError(e);
            }
        }

        private Task BackupTask(string database, IModelDatabaseMaintanance databaseMaintanance){
            return Task.Factory.StartNew(() => {
                    try{
                        var dateTime = DateTime.Now.ToString("yyyy.MMMM.dd-HH.mm.ss");
                        string text = $@"BACKUP DATABASE [{database}] TO  DISK = N'{databaseMaintanance.BackupDirectory}\{database}-{dateTime
                            }.bak' WITH NOFORMAT, NOINIT,  NAME = N'{database}-Full Database Backup on {dateTime}', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                        ExecuteNonQuery(text,databaseMaintanance.BackupTimeout);

                    }
                    catch (Exception e){
                        Tracing.Tracer.LogError(e);
                    }
                },TaskCreationOptions.LongRunning|TaskCreationOptions.AttachedToParent);
            
        }

        [SuppressMessage("Design", "XAF0013:Avoid reading the XafApplication.ConnectionString property")]
        [SuppressMessage("Design", "XAF0012:Avoid calling the XafApplication.CreateObjectSpace() method without Type parameter", Justification = "<Pending>")]
        private void Execute(IModelDatabaseMaintanance databaseMaintanance,Action<Session,string>action) {
            using var objectSpace = Application.CreateObjectSpace();
            var isObjectFitForCriteria = IsObjectFitForCriteria(databaseMaintanance, objectSpace);
            if ((isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value)){
                var parser = new ConnectionStringParser(Application.ConnectionString);
                var database = parser.GetPartByName("Initial Catalog");
                action(objectSpace.Session(), database);
            }
        }

        private bool? IsObjectFitForCriteria(IModelDatabaseMaintanance databaseMaintanance, IObjectSpace objectSpace){
            var user = objectSpace.GetObject(SecuritySystem.CurrentUser);
            CriteriaOperator criteriaOperator;
            var typeInfo = databaseMaintanance.UserModelClass?.TypeInfo;
            var targetObjectType = typeInfo?.Type;
            using(objectSpace.CreateParseCriteriaScope()){
                
                criteriaOperator = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(databaseMaintanance.UserCriteria, targetObjectType);
            }

            var fit = objectSpace.GetExpressionEvaluator(new EvaluatorContextDescriptorDefault(typeInfo==null?typeof(object):targetObjectType), criteriaOperator).Fit(typeInfo==null?new object():user);
            return fit;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsDatabaseMaintanence>();
        }
    }
}
