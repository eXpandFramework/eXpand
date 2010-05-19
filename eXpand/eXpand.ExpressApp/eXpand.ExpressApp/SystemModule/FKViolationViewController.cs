using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassEnableFKViolations
    {
        [Category("eXpand")]
        bool EnableFKViolations { get; set; }
    }

    public class FKViolationViewController : ViewController, IModelExtender
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (((IModelClassEnableFKViolations)View.Model.ModelClass).EnableFKViolations)
                ObjectSpace.ObjectDeleting += ObjectSpace_OnObjectDeleting;
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

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassEnableFKViolations>();
        }
    }
}