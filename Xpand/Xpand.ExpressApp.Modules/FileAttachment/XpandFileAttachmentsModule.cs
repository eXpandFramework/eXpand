using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.FileAttachment.BusinessObjects;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.FileAttachment {
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules), ToolboxBitmap(typeof (XpandFileAttachmentsModule)), ToolboxItem(true)]
    public sealed class XpandFileAttachmentsModule : XpandModuleBase {
        public const string FileDataFolderName = "FileData";
        public static int ReadBytesSize = 0x1000;

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions,IModelOptionsFileSystemStoreLocation>();
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            foreach (var memberInfo in typesInfo.PersistentTypes.SelectMany(info => info.Members)) {
                if (typeof(IFileSystemObject).IsAssignableFrom(memberInfo.MemberType)) {
                    if (memberInfo.FindAttribute<AggregatedAttribute>()==null)
                        memberInfo.AddAttribute(new AggregatedAttribute());
                    if (memberInfo.FindAttribute<NoForeignKeyAttribute>()==null)
                        memberInfo.AddAttribute(new NoForeignKeyAttribute());
                    if (memberInfo.FindAttribute<ImmediatePostDataAttribute>()==null)
                        memberInfo.AddAttribute(new ImmediatePostDataAttribute());
                    if (memberInfo.FindAttribute<ExpandObjectMembersAttribute>()==null)
                        memberInfo.AddAttribute(new ExpandObjectMembersAttribute(ExpandObjectMembers.Never));
                }
            }
        }
        public static string FileSystemStoreLocation {
            get { return String.Format("{0}{1}", PathHelper.GetApplicationFolder(), GetFileSystemStoreLocation()); }
        }

        static string GetFileSystemStoreLocation() {
            if (CaptionHelper.ApplicationModel != null) {
                var modelOptionsFileSystemStoreLocation =
                    ((IModelOptionsFileSystemStoreLocation) CaptionHelper.ApplicationModel.Options);
                return modelOptionsFileSystemStoreLocation.FileSystemStoreLocation;
            }
            var appSetting = ConfigurationManager.AppSettings["FileSystemStoreLocation"];
            return appSetting ?? FileDataFolderName;
        }
    }

    public interface IModelOptionsFileSystemStoreLocation {
        [Category("eXpand.FileAttachment")]
        [DefaultValue(XpandFileAttachmentsModule.FileDataFolderName)]
        string FileSystemStoreLocation { get; set; }
    }

}