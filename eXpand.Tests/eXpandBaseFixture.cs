using System.Data;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using MbUnit.Framework;

namespace eXpand.Tests{
    public abstract class eXpandBaseFixture{
        
        protected const string DefaultClassXml = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"Default\"></Class></BOModel></Application>";
        protected const string DefaultClassXml2 = "<Application><BOModel><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>";
        protected const string elClassXml = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"el\"></Class></BOModel></Application>";
        protected Dictionary DefaultDictionary;
        protected Dictionary DefaultDictionary2;
        protected Dictionary elDictionary;
        private SimpleDataLayer dataLayer;
        private DataSet dataSet;
        [SetUp]
        public virtual void Setup()
        {
            DictionaryNode.ReaderWriterLockWrapperCreator = new ReaderWriterLockWrapper();
            elDictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(elClassXml), Schema.GetCommonSchema());
            DefaultDictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(DefaultClassXml),
                                               Schema.GetCommonSchema());
            DefaultDictionary2 = new Dictionary(new DictionaryXmlReader().ReadFromString(DefaultClassXml2),
                                               Schema.GetCommonSchema());
            Session.DefaultSession.Disconnect();
            
            dataSet = new DataSet();
            XafTypesInfo.Reset(true);

            var dataStore = new InMemoryDataStore(dataSet, AutoCreateOption.DatabaseAndSchema);
            dataLayer = new SimpleDataLayer(XafTypesInfo.XpoTypeInfoSource.XPDictionary, dataStore);
            XpoDefault.DataLayer = dataLayer;
            
            
//            Isolate.Fake.ISecurityComplex();
            


        }



        [TearDown]
        public void TearDown(){
            if (dataLayer != null) {
                dataLayer.Dispose();
            }
            if (dataSet != null) {
                dataSet.Dispose();
            }
            ReflectionHelper.Reset();

        }
    }
}