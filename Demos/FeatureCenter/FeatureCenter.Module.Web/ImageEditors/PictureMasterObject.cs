using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace FeatureCenter.Module.Web.ImageEditors {

    [DisplayFeatureModelAttribute("FeatureCenter.Module.Web.ImageEditors.PictureMasterObject_ListView", "NCarousel")]
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Captions.HeaderNCarousel, "1=1", "1=1",
        Captions.ViewMessageNCarousel, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Captions.HeaderNCarousel, "1=1", "1=1",
        Captions.HeaderNCarousel, Position.Top, ViewType = ViewType.DetailView)]
    public class PictureMasterObject : BaseObject {
        string _title;

        public PictureMasterObject(Session session)
            : base(session) {
        }

        public string Title {
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }

        [Aggregated]
        [Association("MasterObject-HorizontalPictureObjects")]
        public XPCollection<PictureObject> HorizontalPicObjects {
            get { return GetCollection<PictureObject>("HorizontalPicObjects"); }
        }

        [Aggregated]
        [Association("MasterObject-HorizontalPictureObjectsStyleModified")]
        public XPCollection<PictureObject> HorizontalPicObjectsStyleModified {
            get { return GetCollection<PictureObject>("HorizontalPicObjectsStyleModified"); }
        }

        [Aggregated]
        [Association("MasterObject-VerticalPictureObjects")]
        public XPCollection<PictureObject> VerticalPicObjects {
            get { return GetCollection<PictureObject>("VerticalPicObjects"); }
        }

        [Aggregated]
        [Association("MasterObject-VerticalPicObjects")]
        public XPCollection<PictureObject> VerticalPicObjectsStyleModified {
            get { return GetCollection<PictureObject>("VerticalPicObjectsStyleModified"); }
        }
        [Aggregated]
        [Association("MasterObject-HorizontalPicObjectsWithNoImage")]
        public XPCollection<PictureObject> HorizontalPicObjectsWithNoImage {
            get { return GetCollection<PictureObject>("HorizontalPicObjectsWithNoImage"); }
        }
        [Aggregated]
        [Association("MasterObject-VerticalPicObjectsWithNoImage")]
        public XPCollection<PictureObject> VerticalPicObjectsWithNoImage {
            get { return GetCollection<PictureObject>("VerticalPicObjectsWithNoImage"); }
        }

    }
}