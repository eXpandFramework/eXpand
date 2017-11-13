using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Mobile.Controllers {
    public partial class ContactDefaultImageController : ViewController {
        public ContactDefaultImageController() {
            InitializeComponent();
            TargetObjectType = typeof(Contact);
        }
        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectSaving += ObjectSpace_ObjectSaving;
        }

        void ObjectSpace_ObjectSaving(object sender, ObjectManipulatingEventArgs e) {
            Contact contact = e.Object as Contact;
            if(contact != null && ObjectSpace.IsNewObject(contact) && contact.Photo == null) {
                MemoryStream ms = new MemoryStream();
                Image image = ImageLoader.Instance.GetImageInfo("NoProfileImage").Image;
                lock(image) {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                contact.Photo = ms.ToArray();
            }
        }
        protected override void OnDeactivated() {
            ObjectSpace.ObjectSaving -= ObjectSpace_ObjectSaving;
            base.OnDeactivated();
        }
    }
}
