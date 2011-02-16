using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Web.ImageEditors {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (typesInfo.Type == typeof(PictureObject)) {
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("ImageEditors/Thumbnails", "PictureObject_ListView");
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute("5/2/2011", xpandNavigationItemAttribute);
            }
            else if (typesInfo.Type == typeof(PictureMasterObject)) {
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("ImageEditors/NCarousel", "PictureMasterObject_DetailView",
                                                                                    "Title='masterobject'");
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute("4/2/2011", xpandNavigationItemAttribute);
            }

        }
    }
}
