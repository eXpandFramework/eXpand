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
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSecurityModule : XpandModuleBase {
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
                Application.SetupComplete += ApplicationOnSetupComplete;
                Application.LogonFailed += (o, eventArgs) => {
                    var logonParameters = SecuritySystem.LogonParameters as IXpandLogonParameters;
                    if (logonParameters != null && logonParameters.RememberMe) {
                        logonParameters.RememberMe = false;
                        ObjectSerializer.WriteObjectPropertyValues(null, logonParameters.Storage, logonParameters);
                    }
                };
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = ((XafApplication)sender).Security as SecurityStrategy;
            if (securityStrategy != null) (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e) {
            var keyValuePairs = new[]{
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (MyDetailsOperationRequest), new MyDetailsRequestProcessor(e.Permissions)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (AnonymousLoginOperationRequest), new AnonymousLoginRequestProcessor(e.Permissions)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (IsAdministratorPermissionRequest), new IsAdministratorPermissionRequestProcessor(e.Permissions)),
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(NavigationItemPermissionRequest), new NavigationItemPermissionRequestProcessor(e.Permissions))
            };
            foreach (var keyValuePair in keyValuePairs) {
                e.Processors.Add(keyValuePair);
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CurrentUserNameOperator.Register();
            var typeInfos = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo<SecuritySystemRole>()).Where(info => !info.IsAbstract).ToArray();
            foreach (var attribute in SecurityOperationsAttributes(typesInfo)) {
                CreateMember(typeInfos, attribute, typesInfo);
            }
            AddNewObjectCreateGroup(typesInfo, new List<Type> { typeof(ModifierPermission), typeof(ModifierPermissionData) });
        }

        void AddNewObjectCreateGroup(ITypesInfo typesInfo, IEnumerable<Type> types) {
            foreach (var type in types) {
                var typeDescendants = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo(type));
                foreach (var typeInfo in typeDescendants) {
                    typeInfo.AddAttribute(new NewObjectCreateGroupAttribute("SimpleModifer"));
                }
            }
        }

        void CreateMember(IEnumerable<ITypeInfo> typeInfos, SecurityOperationsAttribute attribute, ITypesInfo typesInfo) {
            foreach (var typeInfo in typeInfos){
                if (!RuntimeMode)
                    CreateWeaklyTypedCollection(typesInfo, typeInfo.Type,attribute.CollectionName);
                if (typeInfo.FindMember(attribute.OperationProviderProperty) == null) {
                    typeInfo.CreateMember(attribute.OperationProviderProperty, typeof(SecurityOperationsEnum));
                }
            }
        }

        IEnumerable<SecurityOperationsAttribute> SecurityOperationsAttributes(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<SecurityOperationsAttribute>() != null);
            return typeInfos.SelectMany(info => info.FindAttributes<SecurityOperationsAttribute>());
        }
    }

}