using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Security.Permissions;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    public abstract class ModelRuleProviderModuleBase<TModelRuleAttribute, TModelRulesNodeWrapper, TModelRuleNodeWrapper,
                                                      TModelRuleInfo, TModelRule, TRulePermission> : ModuleBase, IRuleCollector, IRuleHolder
        where TModelRuleAttribute : ModelRuleAttribute
        where TModelRulesNodeWrapper : ModelRulesNodeWrapper<TModelRuleNodeWrapper, TModelRuleAttribute>
        where TModelRuleNodeWrapper : ModelRuleNodeWrapper
        where TModelRuleInfo : ModelRuleInfo<TModelRule>, new()
        where TModelRule : ModelRule
        where TRulePermission : RulePermission
    {
        public abstract string ModelRulesNodeAttributeName { get; }

        void CreateModelRulesFromClassAttributes(
            ModelRulesNodeWrapper<TModelRuleNodeWrapper, TModelRuleAttribute> wrapper, ITypeInfo typeInfo) {
            foreach (
                TModelRuleAttribute attribute in
                    ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.
                        FindAttributes(typeInfo)) {
                TModelRuleNodeWrapper modelRuleNodeWrapper = wrapper.AddRule(attribute, typeInfo);
                OnRuleAdded(modelRuleNodeWrapper, attribute);
            }
        }

        protected virtual void OnRuleAdded(TModelRuleNodeWrapper modelRuleNodeWrapper, TModelRuleAttribute attribute) {
        }

        public void CollectRules(XafApplication xafApplication) {
            lock (ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.Instance)
            {
                SecuritySystem.ReloadPermissions();
                TModelRulesNodeWrapper wrapper = CreateModelWrapper(xafApplication.Model);
                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                    ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.Instance[
                                                                                                                                          typeInfo] = null;
                    List<TModelRule> enumerable = CollectRulesFromModelCore(wrapper, typeInfo).ToList();
                    List<TModelRule> permissions = ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.
                        GetRulesFromPermissions(xafApplication, typeInfo).ToList();
                    enumerable.AddRange(permissions);
                    ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.Instance[
                                                                                                                                          typeInfo] = new List<TModelRule>(enumerable);
                }
            }
        }

        protected virtual IEnumerable<TModelRule> CollectRulesFromModelCore(TModelRulesNodeWrapper wrapper,
                                                                            ITypeInfo typeInfo) {
            IEnumerable<TModelRuleNodeWrapper> modelRuleNodeWrappers = wrapper.FindRules(typeInfo);
            return
                modelRuleNodeWrappers.Select(rule => Activator.CreateInstance(typeof (TModelRule), rule)).Select(
                                                                                                                    modelRule => modelRule).Cast<TModelRule>();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += (sender, args) => CollectRules(application);
            application.LoggedOn += (o, eventArgs) => CollectRules(application);
        }

        void CreateModelRulesFromMethodsAttributes(
            ModelRulesNodeWrapper<TModelRuleNodeWrapper, TModelRuleAttribute> wrapper, ITypeInfo typeInfo) {
            foreach (
                MethodInfo methodInfo in
                    typeInfo.Type.GetMethods(
                                                ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.
                                                    MethodRuleBindingFlags)) {
                foreach (
                    TModelRuleAttribute attribute in
                        ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.
                            FindAttributes(methodInfo)) {
                    wrapper.AddRule(attribute, typeInfo);
                }
            }
        }

        public override void UpdateModel(Dictionary model) {
            base.UpdateModel(model);
            TModelRulesNodeWrapper wrapper = CreateModelWrapper(model);
            foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                CreateModelRulesFromClassAttributes(wrapper, typeInfo);
                CreateModelRulesFromMethodsAttributes(wrapper, typeInfo);
            }
        }

        TModelRulesNodeWrapper CreateModelWrapper(Dictionary dictionary) {
            DictionaryNode dictionaryNode = GetRootNode(dictionary).GetChildNode(ModelRulesNodeAttributeName);

            return
                (TModelRulesNodeWrapper)
                Activator.CreateInstance(typeof (TModelRulesNodeWrapper),
                                         dictionaryNode.GetChildNode(GetElementGroupNodeName()));
        }

        public virtual DictionaryNode GetRootNode(Dictionary dictionary) {
            return dictionary.RootNode;
        }



        public override Schema GetSchema() {
            var schemaHelper = new SchemaHelper();
            schemaHelper.AttibuteCreating += (sender, args) => modifyAttributes(args);
            string CommonTypeInfos = @"<Element Name=""" + ModelRulesNodeAttributeName +@""">
                                            <Element Name=""" +GetElementGroupNodeName() +@""">
                                                <Element Name=""" +GetElementNodeName() +@""" KeyAttribute=""ID"" Multiple=""True"">
                                                    " +schemaHelper.Serialize<IModelRule>(true) +@"
                                                    " + GetMoreSchema() +@"
				                                </Element>
                                            </Element>
                                        </Element>";
            var schema = new Schema(schemaHelper.Inject(CommonTypeInfos, ModelElement.Application));
            return schema;
        }


        void modifyAttributes(AttibuteCreatedEventArgs args) {
            if (args.Attribute.IndexOf("Nesting") > -1 || args.Attribute.IndexOf("ViewType") > -1)
                args.AddTag(@" IsInvisible=""{" + typeof (ViewVisibilityCalculator).FullName +
                            @"}ID=..\..\@ID;ViewType=" + ViewType.Any + @"""");
            else if (args.Attribute.IndexOf("ID") > -1)
                args.AddTag(@" Required=""True""");
            else if (args.Attribute.IndexOf("TypeInfo") > -1)
                args.AddTag(@"Required=""True"" RefNodeName=""/Application/BOModel/Class""");
        }

        protected virtual string GetMoreSchema() {
            return null;
        }

        public abstract string GetElementNodeName();

        public virtual string GetElementGroupNodeName() {
            return "Rules";
        }

        public bool HasRules(View view) {
            return ModelRuleManager<TModelRuleAttribute, TModelRuleNodeWrapper, TModelRuleInfo, TModelRule, TRulePermission>.HasRules(view);
        }
    }
}