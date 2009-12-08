using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class FKViolationViewController : ViewController
    {
        public const string EnableFKViolations = "EnableFKViolations";
        public FKViolationViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.Info.GetAttributeBoolValue(EnableFKViolations))
                ObjectSpace.ObjectDeleting+=ObjectSpace_OnObjectDeleting;
        }

        private void ObjectSpace_OnObjectDeleting(object sender, ObjectsManipulatingEventArgs e)
        {
            foreach (var o in e.Objects)
            {
                var count = ObjectSpace.Session.CollectReferencingObjects(o).Count;
                if (count > 0)
                {
                    var result = new RuleSetValidationResult();
                    var messageTemplate = "Cannot be deleted " + count + " referemces found";
                    result.AddResult(new RuleSetValidationResultItem(o, ContextIdentifier.Delete, null,
                                                                     new RuleValidationResult(null, this, false,
                                                                                              messageTemplate)));
                    throw new ValidationException(messageTemplate, result);
                }    
            }
            
        }

        public override Schema GetSchema()
        {
            const string s = @"<Element Name=""Application"">;
                            <Element Name=""BOModel"">
                                <Element Name=""Class"">
                                    <Attribute Name=""" + EnableFKViolations + @""" Choice=""False,True""/>
                                </Element>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

    }
}