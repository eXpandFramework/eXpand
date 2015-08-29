using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;

namespace MasterDetailTester.Module.Win.FunctionalTests{
    [DefaultClassOptions]
    public class Project : BaseObject, ISupportMasterDetail{
        public Project(Session session) : base(session){
        }

        public string Name { get; set; }

        [Association("Project-Contributors"), Aggregated]
        public XPCollection<Contributor> Contributors{
            get { return GetCollection<Contributor>("Contributors"); }
        }
    }

    public interface ISupportMasterDetail{
    }

    [CloneView(CloneViewType.ListView, "Contributor_Layout_ListView")]
    public class Contributor : BaseObject, ISupportMasterDetail{
        private Image _image;
        private Project _project;

        public Contributor(Session session) : base(session){
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            Image=CreateBitmap(Color.Blue);
        }

        private Bitmap CreateBitmap(Color color) {
            var bitmap = new Bitmap(200, 200);
            using (var graphics = Graphics.FromImage(bitmap)) {
                graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, 200, 200));
                graphics.Save();
            }
            return bitmap;
        }


        public string Name { get; set; }

        [Size((SizeAttribute.Unlimited))]
        [ValueConverter(typeof (ImageValueConverter))]
        [InvisibleInAllViews]
        public Image Image{
            get { return _image; }
            set { SetPropertyValue("Image", ref _image, value); }
        }

        [Association("Project-Contributors")]
        public Project Project{
            get { return _project; }
            set { SetPropertyValue("Project", ref _project, value); }
        }
    }
}