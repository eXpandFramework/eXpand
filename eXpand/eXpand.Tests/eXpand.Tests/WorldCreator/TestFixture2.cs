using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using eXpand.ExpressApp;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace eXpand.Tests.WorldCreator
{
    [TestFixture]
    public class TestFixture2
    {
        [Test]
        public void Test()
        {
            Console.WriteLine("Properties of an arbitrary object: Console.Out:");
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Console.Out);
            foreach (PropertyDescriptor property in properties) Console.WriteLine("- " + property.Name);

            Console.WriteLine("\nAdd our useless provider System.Object and chain it with the original one...");
            TypeDescriptor.AddProvider(new RuntimeMembersTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(object))), typeof(object));

            Console.WriteLine("\nProperties of an arbitrary object: Console.Out:");
            properties = TypeDescriptor.GetProperties(Console.Out);
            foreach (PropertyDescriptor property in properties) Console.WriteLine("- " + property.Name);
        }
    }

//    /// <summary>
//    /// This is our custom provider. It simply provides a custom type descriptor
//    /// and delegates all its other tasks to its parent
//    /// </summary>
//    internal sealed class UselessTypeDescriptionProvider : TypeDescriptionProvider
//    {
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        internal UselessTypeDescriptionProvider(TypeDescriptionProvider parent)
//            : base(parent)
//        {
//        }
//
//        /// <summary>
//        /// Create and return our custom type descriptor and chain it with the original
//        /// custom type descriptor
//        /// </summary>
//        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
//        {
//            return new UselessCustomTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
//        }
//    }

//    /// <summary>
//    /// This is our custom type descriptor. It creates a new property and returns it along
//    /// with the original list
//    /// </summary>
//    internal sealed class UselessCustomTypeDescriptor : CustomTypeDescriptor
//    {
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        internal UselessCustomTypeDescriptor(ICustomTypeDescriptor parent)
//            : base(parent)
//        {
//        }
//
//        /// <summary>
//        /// This method add a new property to the original collection
//        /// </summary>
//        public override PropertyDescriptorCollection GetProperties()
//        {
//            // Enumerate the original set of properties and create our new set with it
//            PropertyDescriptorCollection originalProperties = base.GetProperties();
//            List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
//            foreach (PropertyDescriptor pd in originalProperties) newProperties.Add(pd);
//
//            // Create a new property and add it to the collection
//            PropertyDescriptor newProperty = TypeDescriptor.CreateProperty(typeof(object), "UselessProperty", typeof(string));
//            newProperties.Add(newProperty);
//
//            // Finally return the list
//            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
//        }
//    }
}
