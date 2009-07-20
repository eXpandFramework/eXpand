using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers
{
    public abstract partial class ViewsSyncInfoController : WindowController
    {
        public const string NormalCriteria = "NormalCriteria";
        public const string EmptyCriteria = "EmptyCriteria";
        protected const string STR_Name = "Name";

        protected ViewsSyncInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        public override Schema GetSchema()
        {
            string CommonTypeInfos = @"<Element Name=""Application"">
                    <Element Name=""Views"">
						<Element Name=""ListView"">
							<Element Name=""" + GetElementStateActionName() + @""">
								<Element Name=""Item"" KeyAttribute=""" + STR_Name + @""" DisplayAttribute=""" + STR_Name + @""" Multiple=""True"">
                                    " +GetMoreSchema()+ @"
                                    <Attribute Name=""" + typeof(Nesting).Name + @""" Choice=""{" + typeof(Nesting).FullName + @"}""/>
                                    <Attribute Name=""" + NormalCriteria + @""" />
                                    <Attribute Name=""" + EmptyCriteria + @""" />
			                    </Element>
							</Element>
						</Element>
					    <Element Name=""DetailView"">
							<Element Name=""" + GetElementStateActionName() + @""">
								<Element Name=""Item"" KeyAttribute=""" + STR_Name + @""" DisplayAttribute=""" + STR_Name + @""" Multiple=""True"">
				                    " +GetMoreSchema()+ @"
                                    <Attribute Name=""" + NormalCriteria + @""" />
			                    </Element>
							</Element>
						</Element>
					</Element>
                    
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

        protected virtual string GetMoreSchema()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetElementStateActionName()
        {
            throw new NotImplementedException();
        }
    }
}