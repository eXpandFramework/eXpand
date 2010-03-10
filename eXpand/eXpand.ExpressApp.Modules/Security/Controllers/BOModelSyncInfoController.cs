using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Security.Controllers
{
    public abstract partial class BOModelSyncInfoController : WindowController
    {
        protected const string STR_Name = "Name";
        public const string NormalCriteria = "NormalCriteria";
        public const string EmptyCriteria = "EmptyCriteria";

        protected BOModelSyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        public override Schema GetSchema()
        {
            var schemaHelper = new SchemaHelper();
            string CommonTypeInfos = @"<Element Name=""Application"">
                    <Element Name=""BOModel"" >
                        <Element Name=""Class"" >
                            <Element Name=""" + GetElementStateActionName() + @""">
                                <Element Name=""Item"" KeyAttribute=""" + STR_Name + @""" DisplayAttribute=""" + STR_Name + @""" Multiple=""True"">
                                    " +GetMoreSchema() + @"
                                    " +schemaHelper.Serialize<ILogicRule>(true) + @"
				                </Element>
                            </Element>
                        </Element>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
        protected abstract string GetMoreSchema();
        protected abstract string GetElementStateActionName();
    }
}