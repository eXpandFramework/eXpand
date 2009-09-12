using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.LookAndFeel;
using DevExpress.Xpo.DB;
using eXpand.ExpressApp.FilterDataStore;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace Fixtures.eXpand.ExpressApp.SkinToDataStore
{
    [TestFixture]
    public class FilterDataStoreModuleFixture
    {



        [Test]
        [Isolated]
        [VerifyMocks]
        public void ApplicationSettingUpIsSubscribed()
        {
            var xafApplication = Isolate.Fake.Instance<XafApplication>();

            MockGetEvent(xafApplication);

            var module = new FilterDataStoreModule();
            module.Setup(xafApplication);


        }

        private void MockGetEvent(XafApplication xafApplication)
        {
            using (RecorderManager.StartRecording())
                xafApplication.SetupComplete += null;
            return;
        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void FilteringIsEnabled()
        {
            var module = Isolate.Fake.Instance<FilterDataStoreModule>(Members.CallOriginal);
            Isolate.WhenCalled(() => module.ApplyCondition(null)).ReturnRecursiveFake();
            Isolate.WhenCalled(
                () =>
                module.Application.Info.GetChildNode("Options").GetAttributeBoolValue(
                    FilterDataStoreModule.FilterDataStoreModuleAttributeName)).WillReturn(true);

            module.filterData(new[] { Isolate.Fake.Instance<SelectStatement>() });

            Isolate.Verify.WasCalledWithAnyArguments(() => module.ApplyCondition(null));


        }

        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void SkinIsShared()
        {
            throw new NotImplementedException();

//            var skinToDataStoreModule = Isolate.Fake.Instance<FilterDataStoreModule>(Members.CallOriginal);
//            skinToDataStoreModule.Application = Isolate.Fake.Instance<XafApplication>();
//            Isolate.WhenCalled(() => skinToDataStoreModule.FilterIsShared(null, null)).CallOriginal();
//            Isolate.WhenCalled(
//                () => skinToDataStoreModule.FindClassNameInDictionary(null)).WillReturn("ClassFullName");
//            var applicationNodeWrapper = Isolate.Fake.Instance<ApplicationNodeWrapper>();
//            Isolate.Swap.NextInstance<ApplicationNodeWrapper>().With(applicationNodeWrapper);
//            Isolate.WhenCalled(() => applicationNodeWrapper.BOModel.FindClassByName("ClassFullName").Node.GetAttributeBoolValue(
//                                         eXpandSkinToDataStoreModule.SkinIsSharedAttributeName)).WillReturn(false);
//            
//
//            bool skared=skinToDataStoreModule.SkinIsShared(null);
//
//            Assert.IsFalse(skared);

        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void SkinIsSharedWhenClassNotFoundInModel()
        {
            var skinToDataStoreModule = Isolate.Fake.Instance<FilterDataStoreModule>(Members.CallOriginal);
            skinToDataStoreModule.Application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => skinToDataStoreModule.FilterIsShared(null, null)).CallOriginal();
            Isolate.WhenCalled(
                () => skinToDataStoreModule.FindClassNameInDictionary(null)).WillReturn("ClassFullName");
            var applicationNodeWrapper = Isolate.Fake.Instance<ApplicationNodeWrapper>();
            Isolate.Swap.NextInstance<ApplicationNodeWrapper>().With(applicationNodeWrapper);
            Isolate.WhenCalled(() => applicationNodeWrapper.BOModel.FindClassByName("ClassFullName")).WillReturn(
                null);

            bool skared = skinToDataStoreModule.FilterIsShared(null, null);

            Assert.IsTrue(skared);

        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void InsertData()
        {
            Isolate.Fake.StaticMethods(typeof(UserLookAndFeel));
            Isolate.WhenCalled(() => UserLookAndFeel.Default.ActiveSkinName).WillReturn("ActiveSkinName");
            var instance = Isolate.Fake.Instance<FilterDataStoreModule>();
            var statement = new InsertStatement {Operands = new CriteriaOperatorCollection(),Parameters = new QueryParameterCollection(new OperandValue("skinvalue"))};
            statement.Operands.Add(new QueryOperand("Skin","Alias"));
            Isolate.WhenCalled(() => instance.InsertData(null)).CallOriginal();

            instance.InsertData(new List<InsertStatement> { statement });

            Assert.AreEqual("ActiveSkinName", statement.Parameters[0].Value);
        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void UpdateData()
        {
            Isolate.Fake.StaticMethods(typeof(UserLookAndFeel));
            Isolate.WhenCalled(() => UserLookAndFeel.Default.ActiveSkinName).WillReturn("ActiveSkinName");
            var instance = Isolate.Fake.Instance<FilterDataStoreModule>();
            var statement = new UpdateStatement {Operands = new CriteriaOperatorCollection(),Parameters = new QueryParameterCollection(new OperandValue("skinvalue"))};
            statement.Operands.Add(new QueryOperand("Skin","Alias"));
            Isolate.WhenCalled(() => instance.UpdateData(null)).CallOriginal();

            instance.UpdateData(new List<UpdateStatement> { statement });

            Assert.AreEqual("ActiveSkinName", statement.Parameters[0].Value);
        }

        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void SkinIsSharedWhenClassNotFoundInXpDictionary()
        {
            var skinToDataStoreModule = Isolate.Fake.Instance<FilterDataStoreModule>(Members.CallOriginal);
            skinToDataStoreModule.Application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => skinToDataStoreModule.FilterIsShared(null, null)).CallOriginal();
            Isolate.WhenCalled(
                () => skinToDataStoreModule.FindClassNameInDictionary(null)).WillReturn(null);
            

            bool skared = skinToDataStoreModule.FilterIsShared(null, null);

            Assert.IsTrue(skared);

        }

        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void ApplyConditionWhenSkinIsSharedOrSkinConditionExists()
        {
            var skinToDataStoreModule = Isolate.Fake.Instance<FilterDataStoreModule>();
            Isolate.WhenCalled(() => skinToDataStoreModule.ApplyCondition(null)).CallOriginal();
            Isolate.WhenCalled(() => skinToDataStoreModule.FilterIsShared(null, null)).WillReturn(true);

            var baseStatement = Isolate.Fake.Instance<SelectStatement>();
            BaseStatement statement = skinToDataStoreModule.ApplyCondition(baseStatement);

            Assert.AreEqual(baseStatement, statement);
        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void ApplyConditionWhenSkinIsNotSharedAndSkinConditionNotExists()
        {
            var skinToDataStoreModule = Isolate.Fake.Instance<FilterDataStoreModule>();
            Isolate.WhenCalled(() => skinToDataStoreModule.ApplyCondition(null)).CallOriginal();
            Isolate.WhenCalled(() => skinToDataStoreModule.FilterIsShared(null, null)).WillReturn(false);

            var baseStatement = Isolate.Fake.Instance<SelectStatement>(Members.CallOriginal);
            baseStatement.Alias = "Alias";
            Isolate.Fake.Instance<CriteriaOperatorExtractor>();
            Isolate.Fake.StaticMethods<UserLookAndFeel>();
            Isolate.WhenCalled(() => UserLookAndFeel.Default.ActiveSkinName).WillReturn("SkinName");
            BaseStatement statement = skinToDataStoreModule.ApplyCondition(baseStatement);

            
            Assert.AreEqual("Alias.{Skin} = \'SkinName\'", statement.Condition.ToString());
        }



    }
}