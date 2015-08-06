using System.Drawing;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.Win.FunctionalTests.LayoutViewListEditor{
    [XpandNavigationItem("LayoutViewListEditor/Layout", "LayoutViewListEditor_Layout_ListView")]
    [CloneView(CloneViewType.ListView, "LayoutViewListEditor_Layout_ListView")]

    [XpandNavigationItem("LayoutViewListEditor/Edit", "LayoutViewListEditor_Edit_ListView")]
    [CloneView(CloneViewType.ListView, "LayoutViewListEditor_Edit_ListView")]
    public class LayoutViewListEditorObject : BaseObject{
        private Image _blue;
        private Image _red;
        private SecuritySystemUser _user;

        public LayoutViewListEditorObject(Session session)
            : base(session){
        }

        public SecuritySystemUser User{
            get { return _user; }
            set { SetPropertyValue("User", ref _user, value); }
        }

        [ValueConverter(typeof (ImageValueConverter))]
        [Size(SizeAttribute.Unlimited)]
        public Image Blue{
            get { return _blue; }
            set { SetPropertyValue("Blue", ref _blue, value); }
        }

        [ValueConverter(typeof (ImageValueConverter))]
        [Size(SizeAttribute.Unlimited)]
        public Image Red{
            get { return _red; }
            set { SetPropertyValue("Red", ref _red, value); }
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            Blue = CreateBitmap(Color.Blue);
            Red = CreateBitmap(Color.Red);
        }

        private Bitmap CreateBitmap(Color color){
            var bitmap = new Bitmap(200, 200);
            using (var graphics = Graphics.FromImage(bitmap)){
                graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, 200, 200));
                graphics.Save();
            }
            return bitmap;
        }
    }
}