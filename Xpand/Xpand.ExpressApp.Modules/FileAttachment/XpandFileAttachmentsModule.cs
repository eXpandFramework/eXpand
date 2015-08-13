using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.FileAttachment.BusinessObjects;
using Xpand.Persistent.Base.General;
using AggregatedAttribute = DevExpress.Xpo.AggregatedAttribute;

namespace Xpand.ExpressApp.FileAttachment {
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules), ToolboxBitmap(typeof (XpandFileAttachmentsModule)), ToolboxItem(true)]
    public sealed class XpandFileAttachmentsModule : XpandModuleBase {
        public const string FileDataFolderName = "FileData";
        public static int ReadBytesSize = 0x1000;

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions,IModelOptionsFileSystemStoreLocation>();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
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
            get { return GetFileSystemStoreLocation(); }
        }

        static string GetFileSystemStoreLocation() {        
            var modelOptionsFileSystemStoreLocation =((IModelOptionsFileSystemStoreLocation) CaptionHelper.ApplicationModel.Options);
            var appSetting = ConfigurationManager.AppSettings["FileSystemStoreLocation"];
            var path = appSetting ?? modelOptionsFileSystemStoreLocation.FileSystemStoreLocation;
            return Path.IsPathRooted(path) ? path : Path.Combine(PathHelper.GetApplicationFolder(), path);
        }
    }

    public interface IModelOptionsFileSystemStoreLocation {
        [Category("eXpand.FileAttachment")]
        [DefaultValue(XpandFileAttachmentsModule.FileDataFolderName)]
        string FileSystemStoreLocation { get; set; }
    }

}