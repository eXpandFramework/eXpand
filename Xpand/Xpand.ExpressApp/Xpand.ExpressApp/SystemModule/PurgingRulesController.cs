using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomFunctions;
using Xpand.Utils.Threading;
using Task = System.Threading.Tasks.Task;

namespace Xpand.ExpressApp.SystemModule{

    [ModelAbstractClass]
    public interface IModelClassPurgingRules:IModelClass{
        IModelPurgingRules PurgingRules{ get; }
    }

    [ModelNodesGenerator(typeof (PurgingRulesModelNodesGenerator))] 
    public interface IModelPurgingRules : IModelNode, IModelList<IModelPurgingRule>{
    }

    public interface IModelPurgingRule:IModelNode{
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
        [CriteriaOptions("TypeInfo")]
        [DefaultValue(EvaluateCSharpOperator.OperatorName+"('new Regex(\".*\").IsMatch(Environment.MachineName)')")]
        string Criteria{ get; set; }
        [Browsable(false)]
        [ModelValueCalculator("((IModelClass) Parent.Parent).TypeInfo")]
        ITypeInfo TypeInfo { get; }
        TimeSpan Interval{ get; set; }
        [DefaultValue(500)]
        int ChunkSize{ get; set; }
    }

    public class PurgingRulesModelNodesGenerator:ModelNodesGeneratorBase{
        protected override void GenerateNodesCore(ModelNode node){
        }
    }

    public class PurgingController:Controller
        ,IModelExtender{

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow){
                var purgingRules = Application.Model.BOModel.Cast<IModelClassPurgingRules>().SelectMany(rules => rules.PurgingRules ).ToArray();
                IEnumerable<(IModelPurgingRule rule, DateTime executed)> ruleExecutionTimes;
                using (var objectSpace = Application.CreateObjectSpace()){

                    DeleteObsoleteRules(objectSpace, purgingRules);
                    var rulesToSchedule = purgingRules.Where(rule =>rule.Interval!=TimeSpan.MinValue);
                    ruleExecutionTimes = CalculateExecutionTimes(rulesToSchedule, objectSpace).ToArray();
                }

                foreach (var ruleExecutionTime in ruleExecutionTimes){
                    var timeSinceLastExecution = DateTime.Now.Subtract(ruleExecutionTime.executed);
                    int delay=timeSinceLastExecution<ruleExecutionTime.rule.Interval?(int) ruleExecutionTime.rule.Interval.Subtract(timeSinceLastExecution).TotalMilliseconds:0;
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.Token.Register(Frame.Dispose);
                    var maxIterations = ruleExecutionTime.rule.Interval==TimeSpan.Zero?1:-1;
                    Task.Factory.StartNewPeriodic(() => PurgeObjects(ruleExecutionTime.rule,cancellationTokenSource.Token),
                        interval: (int) ruleExecutionTime.rule.Interval.TotalMilliseconds, delay: delay,cancelToken:cancellationTokenSource.Token,maxIterations:maxIterations);
                }
                
            }
        }

        private static IEnumerable<(IModelPurgingRule rule, DateTime executed)> CalculateExecutionTimes(IEnumerable<IModelPurgingRule> rulesToSchedule, IObjectSpace objectSpace){
            return rulesToSchedule.Select(rule => {
                var ruleId = ((ModelNode) rule).Id;
                var ruleScheduleStorage = objectSpace.GetObjectsQuery<RuleInfoObject>()
                                              .FirstOrDefault(storage =>storage.RuleScheduleType == RuleScheduleType.Purging && storage.RuleId ==ruleId);
                return (rule,executed:ruleScheduleStorage?.Executed ?? DateTime.MinValue);
            });
        }

        private void DeleteObsoleteRules(IObjectSpace objectSpace, IModelPurgingRule[] purgingRules){
            return;
//            var ids = purgingRules.Cast<ModelNode>().Select(node => node.GetParent<IModelClass>().Id()+node.Id).ToArray();
//            var rulesToDelete = objectSpace.GetObjectsQuery<RuleInfoObject>().Where(storage =>
//                storage.RuleScheduleType == RuleScheduleType.Purging && !ids.Contains(storage.RuleId)).ToArray();
//            objectSpace.Delete(rulesToDelete);
//            objectSpace.CommitChanges();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassPurgingRules>();
        }

        private void PurgeObjects(IModelPurgingRule purgingRule, CancellationToken token){
            try{
                Tracing.Tracer.LogVerboseText($"Purging {purgingRule}");
                var objectsCount = 0;
                if (Application != null)
                    using (var objectSpace = Application.CreateObjectSpace()){
                        StoreExecutionTime(purgingRule, objectSpace);
                        var criteriaOperator = objectSpace.ParseCriteria(purgingRule.Criteria);
                        var objects = objectSpace.GetObjects(purgingRule.TypeInfo.Type, criteriaOperator);
                        objectSpace.SetTopReturnedObjectsCount(objects, purgingRule.ChunkSize);
                        while (objects.Count > 0){
                            token.ThrowIfCancellationRequested();
                            objectsCount += objects.Count;
                            objectSpace.Delete(objects);
                            objectSpace.CommitChanges();
                            objectSpace.ReloadCollection(objects);
                        }
                    }

                Tracing.Tracer.LogVerboseText($"Purged {purgingRule}-{objectsCount}");
            }
            catch (TaskCanceledException){

            }
            catch (Exception e){
                Tracing.Tracer.LogError(e);
            }
        }

        private static void StoreExecutionTime(IModelPurgingRule purgingRule, IObjectSpace objectSpace){
            var ruleId = purgingRule.GetParent<IModelClass>().Id()+((ModelNode) purgingRule).Id;
            var ruleScheduleStorage =objectSpace.GetObjectsQuery<RuleInfoObject>().FirstOrDefault(storage =>
                    storage.RuleScheduleType == RuleScheduleType.Purging && storage.RuleId == ruleId) ?? objectSpace.CreateObject<RuleInfoObject>();
            ruleScheduleStorage.RuleScheduleType = RuleScheduleType.Purging;
            ruleScheduleStorage.RuleId = ((ModelNode) purgingRule).Id;
            ruleScheduleStorage.Executed = DateTime.Now;
            objectSpace.CommitChanges();
        }
    }
}