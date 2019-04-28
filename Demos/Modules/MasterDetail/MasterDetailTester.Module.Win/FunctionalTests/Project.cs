using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace MasterDetailTester.Module.Win.FunctionalTests{
    [DefaultClassOptions]
    public class Project : BaseObject, ISupportMasterDetail{
        private string _detailViewType
            ;

        private int _relations;

        public Project(Session session) : base(session){
        }

        public string Name { get; set; }

        public int Relations{
            get { return _relations; }
            set { SetPropertyValue(nameof(Relations), ref _relations, value); }
        }

        [Association("Project-Contributors")]
        [Aggregated]
        public XPCollection<Contributor> Contributors => GetCollection<Contributor>("Contributors");

        [Association("Project-Developers")]
        [Aggregated]
        public XPCollection<Developer> Developers => GetCollection<Developer>(nameof(Developers));

        public string DetailViewType{
            get { return _detailViewType; }
            set { SetPropertyValue(nameof(DetailViewType), ref _detailViewType, value); }
        }

        public override string ToString(){
            return Name;
        }
    }

    public class Developer : BaseObject{
        private string _name;

        private Project _project;

        public Developer(Session session) : base(session){
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [Association("Project-Developers")]
        public Project Project{
            get { return _project; }
            set { SetPropertyValue(nameof(Project), ref _project, value); }
        }
    }

    public interface ISupportMasterDetail{
    }

    [CloneModelView(CloneViewType.ListView, "Contributor_Layout_ListView")]
    public class Contributor : BaseObject, ISupportMasterDetail{
        private Image _image;
        private Project _project;

        public Contributor(Session session) : base(session){
            
        }

        public string Name { get; set; }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(ImageValueConverter))]
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

        public override void AfterConstruction(){
            base.AfterConstruction();
            Image = CreateBitmap(Color.Blue);
        }

        private Bitmap CreateBitmap(Color color){
            var bitmap = new Bitmap(200, 200);
            using (var graphics = Graphics.FromImage(bitmap)){
                graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, 200, 200));
                graphics.Save();
            }
            return bitmap;
        }

        public override string ToString(){
            return Name;
        }
    }
}