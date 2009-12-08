using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace Fixtures.eXpand.ExpressApp
{
    /*
     * Serialize any iterface to a set of attributes
     * Attributes can be modified beforecreation
     * Can Create any element of the Basic Model Elements
     * Can Inject schema at any element of the Basic Model Elements
     */
    [TestFixture]
    public class SchemaHelperFixture
    {

        [Test]
        public void Serialize_any_class_to_a_set_of_attributes()
        {
            var schemaHelper = new SchemaHelper();
            string schema = schemaHelper.Serialize<MyClassSerializeToSchemaAttributes>(true);

            Assert.AreEqual(
                "<Attribute Name=\"StringProperty\"/><Attribute Name=\"BoolProperty\" Choice=\"True,False\"/><Attribute Name=\"EnumProperty\" Choice=\"{" +
                typeof(Enum).FullName + "}\"/><Attribute Name=\"IntProperty\"/>", schema);
        }

        [Test]
        public void Attributes_can_be_modified_before_creation()
        {
            var schemaHelper = new SchemaHelper();
            schemaHelper.AttibuteCreating +=
                (sender, args) =>
                    {
                        args.Handled = true;
                        args.Attribute = args.Attribute.Replace("Attribute", "ChangeAttribute");
                    };
            string schema = schemaHelper.Serialize<MyClassSerializeToSchemaAttributes>(true);
            
            Assert.AreEqual(
                "<ChangeAttribute Name=\"StringProperty\"/><ChangeAttribute Name=\"BoolProperty\" Choice=\"True,False\"/><ChangeAttribute Name=\"EnumProperty\" Choice=\"{" +
                typeof(Enum).FullName + "}\"/><ChangeAttribute Name=\"IntProperty\"/>", schema);
        }
        [Test]
        [Isolated]
        public void Create_Application()
        { 
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.Application);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_BOModel()
        { 
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.BOModel);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            dictionaryNode.AddChildNode("Element").SetAttribute("Name", ModelElement.BOModel.ToString());

            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_Class()
        {
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.Class);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = dictionaryNode.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.BOModel.ToString());
            childNode.AddChildNode("Element").SetAttribute("Name", ModelElement.Class.ToString());
            

            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_Member()
        {
            
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.Member);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = dictionaryNode.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.BOModel.ToString());
            var addChildNode = childNode.AddChildNode("Element");
            addChildNode.SetAttribute("Name", ModelElement.Class.ToString());
            addChildNode.AddChildNode("Element").SetAttribute("Name",ModelElement.Member.ToString());

            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_View()
        {
            
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.Views);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            dictionaryNode.AddChildNode("Element").SetAttribute("Name", ModelElement.Views.ToString());

            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_ListView()
        {
            
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.ListView);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = dictionaryNode.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.Views.ToString());
            childNode.AddChildNode("Element").SetAttribute("Name", ModelElement.ListView.ToString());
            

            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
        }
        [Test]
        [Isolated]
        public void Create_DetailView()
        {
            
            var helper = new SchemaHelper();

            DictionaryNode node=helper.CreateElement(ModelElement.DetailView);

            var dictionaryNode = new DictionaryNode("Element");
            dictionaryNode.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = dictionaryNode.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.Views.ToString());
            childNode.AddChildNode("Element").SetAttribute("Name", ModelElement.DetailView.ToString());


            Assert.AreEqual(dictionaryNode.ToXml(), node.ToXml());
            
        }
        
        [Test]
        [Isolated]
        public void Inject_Into_Class()
        {
            var helper = new SchemaHelper();
            var element = new DictionaryNode("Element");
            element.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = element.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.BOModel.ToString());
            var addChildNode = childNode.AddChildNode("Element");
            addChildNode.SetAttribute("Name", ModelElement.Class.ToString());
            addChildNode.AddChildNode("Element1");

            DictionaryNode dictionaryNode=helper.Inject(@"<Element1/>", ModelElement.Class);


            
            Assert.AreEqual(element.ToXml(), dictionaryNode.ToXml());
        }
        [Test]
        [Isolated]
        public void Inject_Into_DetailView()
        {
            var helper = new SchemaHelper();
            var element = new DictionaryNode("Element");
            element.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = element.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.Views.ToString());
            var addChildNode = childNode.AddChildNode("Element");
            addChildNode.SetAttribute("Name", ModelElement.DetailView.ToString());
            addChildNode.AddChildNode("Element1");

            DictionaryNode dictionaryNode=helper.Inject(@"<Element1/>", ModelElement.DetailView);


            
            Assert.AreEqual(element.ToXml(), dictionaryNode.ToXml());
        }
        [Test]
        [Isolated]
        public void Inject_Into_ListView()
        {
            var helper = new SchemaHelper();
            var element = new DictionaryNode("Element");
            element.SetAttribute("Name", ModelElement.Application.ToString());
            var childNode = element.AddChildNode("Element");
            childNode.SetAttribute("Name", ModelElement.Views.ToString());
            var addChildNode = childNode.AddChildNode("Element");
            addChildNode.SetAttribute("Name", ModelElement.ListView.ToString());
            addChildNode.AddChildNode("Element1");

            DictionaryNode dictionaryNode=helper.Inject(@"<Element1/>", ModelElement.ListView);


            
            Assert.AreEqual(element.ToXml(), dictionaryNode.ToXml());
        }
    }

    internal class MyClassSerializeToSchemaAttributesBase
    {
        public int IntProperty { get; set; }
    }

    internal class MyClassSerializeToSchemaAttributes : MyClassSerializeToSchemaAttributesBase
    {
        public string StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public Enum EnumProperty { get; set; }
    }
}