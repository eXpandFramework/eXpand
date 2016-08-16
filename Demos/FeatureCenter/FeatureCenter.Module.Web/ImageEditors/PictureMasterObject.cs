using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace FeatureCenter.Module.Web.ImageEditors {

    [DisplayFeatureModelAttribute("PictureMasterObject_ListView", "NCarousel")]
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
        public XPCollection<PictureObject> HorizontalPicObjects => GetCollection<PictureObject>("HorizontalPicObjects");

        [Aggregated]
        [Association("MasterObject-HorizontalPictureObjectsStyleModified")]
        public XPCollection<PictureObject> HorizontalPicObjectsStyleModified => GetCollection<PictureObject>("HorizontalPicObjectsStyleModified");

        [Aggregated]
        [Association("MasterObject-VerticalPictureObjects")]
        public XPCollection<PictureObject> VerticalPicObjects => GetCollection<PictureObject>("VerticalPicObjects");

        [Aggregated]
        [Association("MasterObject-VerticalPicObjects")]
        public XPCollection<PictureObject> VerticalPicObjectsStyleModified => GetCollection<PictureObject>("VerticalPicObjectsStyleModified");

        [Aggregated]
        [Association("MasterObject-HorizontalPicObjectsWithNoImage")]
        public XPCollection<PictureObject> HorizontalPicObjectsWithNoImage => GetCollection<PictureObject>("HorizontalPicObjectsWithNoImage");

        [Aggregated]
        [Association("MasterObject-VerticalPicObjectsWithNoImage")]
        public XPCollection<PictureObject> VerticalPicObjectsWithNoImage => GetCollection<PictureObject>("VerticalPicObjectsWithNoImage");
    }
}