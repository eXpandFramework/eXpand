using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectController : WinDetailViewController
    {
        public const string RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectAttributeName = "RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObject";
        protected override void OnViewQueryCanChangeCurrentObject(System.ComponentModel.CancelEventArgs e)
        {
            if (View.Info.GetAttributeBoolValue(RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectAttributeName, true))
                base.OnViewQueryCanChangeCurrentObject(e);
        }
        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                        <Element Name=""Views"" >
                            <Element Name=""DetailView"">
                                 <Attribute Name=""" + RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectAttributeName + @""" Choice=""False,True""/>
                            </Element>
                            <Element Name=""ListView"">
                                 <Attribute Name=""" + RollBackObjectChangesOnDetailViewQueryCanChangeCurrentObjectAttributeName + @""" Choice=""False,True""/>
                            </Element>
                        </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}