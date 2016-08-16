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
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Fasterflect;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSecurityModule : XpandModuleBase {
        public const string BaseImplNameSpace = "Xpand.Persistent.BaseImpl.Security";
        public const string XpandSecurity = "eXpand.Security";
        public XpandSecurityModule() {
            RequiredModuleTypes.Add(typeof(SecurityModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
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
                    var logonParameters = SecuritySystem.LogonParameters as IXpandLogonParameters;
                    if (logonParameters != null && logonParameters.RememberMe) {
                        logonParameters.RememberMe = false;
                        ObjectSerializer.WriteObjectPropertyValues(null, logonParameters.Storage, logonParameters);
                    }
                };
            }
            else {
                AddToAdditionalExportedTypes(BaseImplNameSpace);
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = ((XafApplication)sender).Security as SecurityStrategy;
            if (securityStrategy != null) (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }


        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e) {
            var permissionDictionary = e.Permissions.WithCustomPermissions();
            var permissionRequestProcessor = e.Processors.Select(pair => pair.Value).OfType<ISecurityProcessor>().FirstOrDefault();
            if (permissionRequestProcessor != null){
                
                var fieldName = "permissionDictionary";
                if (permissionRequestProcessor is ServerPermissionRequestProcessor)
                    fieldName = "permissions";
                var processorDictionary = ((IPermissionDictionary)permissionRequestProcessor.GetFieldValue(fieldName));
                permissionRequestProcessor.SetFieldValue(fieldName,processorDictionary.WithSecurityOperationAttributePermissions());
            }
            var keyValuePairs = new[]{
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (MyDetailsOperationRequest), new MyDetailsRequestProcessor(permissionDictionary)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (AnonymousLoginOperationRequest), new AnonymousLoginRequestProcessor(permissionDictionary)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (IsAdministratorPermissionRequest), new IsAdministratorPermissionRequestProcessor(permissionDictionary)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(NavigationItemPermissionRequest), new NavigationItemPermissionRequestProcessor(e.Permissions.WithHiddenNavigationItemPermissions()))
            };
            foreach (var keyValuePair in keyValuePairs) {
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
                    foreach (var roleInfo in roleInfos.Where(info => !RuntimeMode || info.Type == RoleType)){
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