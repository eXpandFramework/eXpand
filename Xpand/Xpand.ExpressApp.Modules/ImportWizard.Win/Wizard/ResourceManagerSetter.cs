using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.ExpressApp.Win.Templates;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard
{
    [DesignerSerializer(typeof(ResourceManagerSetterSerializer), typeof(CodeDomSerializer)), ToolboxItem(true)]
    public class ResourceManagerSetter : Component { }

    public class ResourceManagerSetterSerializer : CodeDomSerializer
    {
        public override object Deserialize(IDesignerSerializationManager manager, object codeDomObject)
        {
            var baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(ResourceManagerSetter).BaseType, typeof(CodeDomSerializer));
            return baseSerializer.Deserialize(manager, codeDomObject);
        }
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            var baseSerializer = (CodeDomSerializer)manager
                .GetSerializer(typeof(ResourceManagerSetter).BaseType, typeof(CodeDomSerializer));
            object codeObject = baseSerializer.Serialize(manager, value);

            var statements = codeObject as CodeStatementCollection;
            if (statements != null)
            {
                CodeExpression leftCodeExpression = new CodeVariableReferenceExpression("resources");
                var classTypeDeclaration = (CodeTypeDeclaration)manager.GetService(typeof(CodeTypeDeclaration));
                CodeExpression typeofExpression = new CodeTypeOfExpression(classTypeDeclaration.Name);
                CodeExpression rightCodeExpression =
                    new CodeObjectCreateExpression(typeof(XafComponentResourceManager),
                                                                  new[] { typeofExpression });
                //CodeExpression rightCodeExpression =
                //    new CodeTypeReferenceExpression(
                //        "new DevExpress.ExpressApp.Win.Templates"),
                //        "XafComponentResourceManager", new[] { typeofExpression });

                statements.Insert(0, new CodeAssignStatement(leftCodeExpression, rightCodeExpression));
            }

            return codeObject;


        }
    }
}
