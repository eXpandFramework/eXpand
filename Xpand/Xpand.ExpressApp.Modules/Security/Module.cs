using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Fasterflect;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Xpo;
using Xpand.Persistent.Base.Xpo.MetaData;
using Xpand.XAF.Modules.ModelViewInheritance;
using MyDetailsController = Xpand.ExpressApp.Security.Controllers.MyDetailsController;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSecurityModule : XpandModuleBase {           
        public const string BaseImplNameSpace = "Xpand.Persistent.BaseImpl.Security";
        public const string XpandSecurity = "eXpand.Security";
        public XpandSecurityModule() {
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
            RequiredModuleTypes.Add(typeof(SecurityModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            AdditionalExportedTypes.Add(typeof(XpandLogonParameters));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsAuthentication>();
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                if (Application.Security != null && typeof(IPermissionPolicyUser).IsAssignableFrom(Application.Security.UserType))
                    AddToAdditionalExportedTypes(BaseImplNameSpace);
                Application.SetupComplete += ApplicationOnSetupComplete;
                Application.LogonFailed += (o, eventArgs) => {
                    if (SecuritySystem.LogonParameters is IXpandLogonParameters logonParameters && logonParameters.RememberMe) {
                        logonParameters.RememberMe = false;
                        ObjectSerializer.WriteObjectPropertyValues(null, logonParameters.Storage, logonParameters);
                    }
                };
            }
            else {
                AddToAdditionalExportedTypes(BaseImplNameSpace);
            }
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            var types = new[]{
                typeof(ManageUsersOnLogonController),
                typeof(ChooseDatabaseAtLogonController),
                typeof(ShowNavigationItemController),
                typeof(MyDetailsController),
                typeof(MyDetailsPermissionController),
                typeof(FilterCustomPermissionsController),
                typeof(DefaultRolePermissionsController),
                typeof(RememberMeController),
                typeof(CreatableItemController),
                typeof(FilterByColumnController),
                typeof(CreateExpandAbleMembersViewController),
                typeof(HideFromNewMenuViewController),
                typeof(CustomAttributesController),
                typeof(NotifyMembersController),
                typeof(XpandModelMemberInfoController),
                typeof(XpandLinkToListViewController),
                typeof(ModifyObjectSpaceController)
            };
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(types).Concat(types));
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var xafApplication = ((XafApplication)sender);
            if (xafApplication.Security is SecurityStrategy securityStrategy) {
                securityStrategy.AnonymousAllowedTypes.Add(typeof(RegisterUserParameters));
                securityStrategy.AnonymousAllowedTypes.Add(typeof(RestorePasswordParameters));
                if (((IModelOptionsChooseDatabaseAtLogon) xafApplication.Model.Options).ChooseDatabaseAtLogon) {
                    securityStrategy.AnonymousAllowedTypes.Add(typeof(StringObject));
                }
                (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
            }
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e){
            CustomizeRequestProcessors(e);
        }

        public static void CustomizeRequestProcessors(CustomizeRequestProcessorsEventArgs e){
            var customPermissions = e.Permissions.WithCustomPermissions();
            var fieldName = "permissionsGroupedByRole";
            var requestProcessors = e.Processors.Select(pair => pair.Value)
                .Where(processor => processor is SerializablePermissionRequestProcessorWrapper ||
                                    processor is NavigationPermissionRequestProcessor);
            foreach (var processor in requestProcessors){
                IPermissionRequestProcessor requestProcessor;
                if (processor is NavigationPermissionRequestProcessor){
                    requestProcessor = processor;
                    var enumerable =
                        ((IEnumerable<IEnumerable<IOperationPermission>>) requestProcessor.GetFieldValue(fieldName)).Select(
                            permissions => new PermissionDictionary(permissions).WithSecurityOperationAttributePermissions()
                                .GetPermissions<IOperationPermission>());
                    requestProcessor.SetFieldValue(fieldName, enumerable);
                    customPermissions = new PermissionDictionary(customPermissions.GetPermissions<IOperationPermission>()
                        .Concat(enumerable.SelectMany(permissions => permissions)));
                }
                else{
                    requestProcessor = (IPermissionRequestProcessor) processor.GetFieldValue("requestProcessor");
                    if (requestProcessor is ServerPermissionRequestProcessor){
                        fieldName = "permissionsDictionary";
                        var processorDictionary = ((IPermissionDictionary) requestProcessor.GetFieldValue(fieldName))
                            .WithSecurityOperationAttributePermissions();
                        requestProcessor.SetFieldValue(fieldName, processorDictionary);
                        var operationPermissions = processorDictionary.GetPermissions<IOperationPermission>().ToList();
                        customPermissions = new PermissionDictionary(customPermissions.GetPermissions<IOperationPermission>()
                            .Concat(operationPermissions));
                    }
                }
            }

            var keyValuePairs = new[]{
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(MyDetailsOperationRequest),
                    customPermissions.GetProcessor<MyDetailsRequestProcessor>()),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(AnonymousLoginOperationRequest),
                    customPermissions.GetProcessor<AnonymousLoginRequestProcessor>()),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(IsAdministratorPermissionRequest),
                    customPermissions.GetProcessor<IsAdministratorPermissionRequestProcessor>()),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(NavigationItemPermissionRequest),
                    customPermissions.WithHiddenNavigationItemPermissions()
                        .GetProcessor<NavigationItemPermissionRequestProcessor>())
            };
            foreach (var keyValuePair in keyValuePairs){
                e.Processors.Add(keyValuePair);
            }
        }


        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CurrentUserNameOperator.Instance.Register();
            ApplySecurityOperations(typesInfo);
            var types = typesInfo.DomainSealedInfos<IModifier>();
            foreach (var typeInfo in types) {
                typeInfo.AddAttribute(new NewObjectCreateGroupAttribute("SimpleModifer"));
            }
        }

        private void ApplySecurityOperations(ITypesInfo typesInfo){
            var securityOperationInfos =typesInfo.PersistentTypes.Where(info => info.FindAttribute<SecurityOperationsAttribute>() != null);
            var roleInfos = typesInfo.DomainSealedInfos<ISecurityRole>().ToArray();
            foreach (var securityOperationInfo in securityOperationInfos){
                var securityOperationsAttributes = securityOperationInfo.FindAttributes<SecurityOperationsAttribute>();
                foreach (var securityOperationsAttribute in securityOperationsAttributes){
                    foreach (var roleInfo in roleInfos.Where(info => (!RuntimeMode || info.Type == RoleType)&&!(((TypeInfo)info).Source is ReflectionTypeInfoSource))){
                        if (roleInfo.FindMember(securityOperationsAttribute.OperationProviderProperty) == null)
                            roleInfo.CreateMember(securityOperationsAttribute.OperationProviderProperty,typeof(SecurityOperationsEnum));
                        if (!RuntimeMode)
                            CreateWeaklyTypedCollection(typesInfo, roleInfo.Type,securityOperationsAttribute.CollectionName);
                    }
                }
            }
        }

    }

}