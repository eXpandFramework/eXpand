using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class HighlightFocusedLayoutItemDetailViewControllerBase : ViewController<DetailView>
    {
        public const string HighlightFocusedLayoutItemAttributeName = "HighlightFocusedLayoutItem";
        public const string EnableHighlightFocusedLayoutItemAttributeName = "EnableHighlightFocusedLayoutItem";
        public const string ActiveKeyHighlightFocusedEditor = "HighlightFocusedLayoutItem";
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            var dv = view as DetailView;
            if (dv != null)
                Active[ActiveKeyHighlightFocusedEditor] = dv.Info.GetAttributeBoolValue(HighlightFocusedLayoutItemAttributeName);
        }
        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                 @"<Element Name=""Application"">
                        <Element Name=""Options"">
	                        <Attribute Name=""" + EnableHighlightFocusedLayoutItemAttributeName + @""" Choice=""True,False"" />
                        </Element>
                    	<Element Name=""Views"">
                            <Element Name=""DetailView"">
							    <Attribute Name=""" + HighlightFocusedLayoutItemAttributeName + @""" Required=""False"" Choice=""True,False"" DefaultValueExpr=""SourceNode=Options; SourceAttribute=@" + EnableHighlightFocusedLayoutItemAttributeName + @"""/>
						    </Element>
					    </Element>
				</Element>")
                );
        }
        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
            var optionsNode = dictionary.RootNode.FindChildElementByPath(@"Options") as DictionaryNode;
            if (optionsNode != null)
                optionsNode.SetAttribute(EnableHighlightFocusedLayoutItemAttributeName, true);
        }
        protected abstract void AssignStyle(object control);
    }
}