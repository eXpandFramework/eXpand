using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.Security.Win.Permissions;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Win {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandSecurityWinModule : XpandSecurityModuleBase {
        private const string LogonParametersFile = "LogonParameters.bin";
        private string _logonParametersFilePath;

        public XpandSecurityWinModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            PermissionProviderStorage.Instance.Add(new OverallCustomizationAllowedPermission());
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            OverallCustomizationAllowedPermissionRequest.Register(Application);
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            Application.CreateCustomLogonParameterStore+=ApplicationOnCreateCustomLogonParameterStore;
            Application.LastLogonParametersWriting+=ApplicationOnLastLogonParametersWriting;
        }

        private void ApplicationOnLastLogonParametersWriting(object sender, LastLogonParametersWritingEventArgs e){
            if (((IModelOptionsAuthentication)Application.Model.Options).Athentication.AutoAthentication.Enabled) {
                var windowsCredentialStorage = e.SettingsStorage as EncryptedSettingsStorage;
                if (windowsCredentialStorage != null){
                    var path = Path.Combine(_logonParametersFilePath, LogonParametersFile);
                    if (((XpandLogonParameters) e.LogonObject).RememberMe){
                        ObjectSerializer.WriteObjectPropertyValues(e.DetailView, e.SettingsStorage, e.LogonObject);
                        var contents = windowsCredentialStorage.GetContent();
                        File.WriteAllBytes(path, contents);
                    }
                    else{
                        File.Delete(path);
                    }
                }
            }
        }

        private void ApplicationOnCreateCustomLogonParameterStore(object sender, CreateCustomLogonParameterStoreEventArgs e){
            if (SecuritySystem.LogonParameters is XpandLogonParameters&& ((IModelOptionsAuthentication) Application.Model.Options).Athentication.AutoAthentication.Enabled){
                var encryptedSettingsStorage = new EncryptedSettingsStorage();
                e.Storage = encryptedSettingsStorage;
                _logonParametersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), CaptionHelper.ApplicationModel.Title);
                if (!Directory.Exists(_logonParametersFilePath))
                    Directory.CreateDirectory(_logonParametersFilePath);
                var path = Path.Combine(_logonParametersFilePath, LogonParametersFile);
                if (File.Exists(path)){
                    var readAllBytes = File.ReadAllBytes(path);
                    try{
                        encryptedSettingsStorage.SetContents(readAllBytes);
                    }
                    catch (CryptographicException cryptographicException){
                        Tracing.Tracer.LogError(cryptographicException);
                        File.Delete(path);
                    }
                }
                e.Handled = true;
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var type = Application == null ? typeof(XpandRole) : RoleType;
            var typeInfo = typesInfo.FindTypeInfo(type);
            if (typeInfo != null && typeInfo.FindMember("ModifyLayout") == null)
                typeInfo.CreateMember("ModifyLayout", typeof(bool));
        }
    }
}