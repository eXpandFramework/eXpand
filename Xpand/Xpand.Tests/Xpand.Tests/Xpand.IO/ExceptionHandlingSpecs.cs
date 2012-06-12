using System;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Persistent.BaseImpl.ImportExport;
using Xpand.Xpo;

namespace Xpand.Tests.Xpand.IO {
    public class ExceptionHandlingSpecsBase {
        Establish context = () => {
            XafTypesInfo.Instance.RegisterEntity(typeof(IOError));
            XafTypesInfo.Instance.RegisterEntity(typeof(Role));
        };
    }
    public class When_SerializedObject_Type_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentObject.xml");
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_an_info_object = () => {
            _ioError =
                _unitOfWork.FindObject<IOError>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, null);
            _ioError.ShouldNotBeNull();
        };

        It should_have_the_reason_of_the_fail = () => _ioError.Reason.ShouldEqual(FailReason.TypeNotFound);
        It should_have_the_Element_description = () => {
            _ioError.ElementXml.ShouldNotBeNull();
            _ioError.InnerXml.ShouldBeNull();
        };
    }
    public class When_SerializedObjectRef_Type_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentObjectProperty.xml");
            XafTypesInfo.Instance.RegisterEntity(typeof(ModelDifferenceObject));
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_an_info_object = () => {
            _ioError =
                _unitOfWork.FindObject<IOError>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, null);
            _ioError.ShouldNotBeNull();
        };

        It should_have_the_reason_of_the_fail = () => _ioError.Reason.ShouldEqual(FailReason.TypeNotFound);
        It should_have_element_description = () => _ioError.InnerXml.ShouldNotBeNull();
        It should_have_the_parent_element_description = () => _ioError.ElementXml.ShouldNotBeNull();
    }
    public class When_Simple_property_name_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentSimpleProperty.xml");
            XafTypesInfo.Instance.RegisterEntity(typeof(ModelDifferenceObject));
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_an_info_object = () => {
            _ioError =
                _unitOfWork.FindObject<IOError>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, null);
            _ioError.ShouldNotBeNull();
        };

        It should_have_the_reason_of_the_fail = () => _ioError.Reason.ShouldEqual(FailReason.PropertyNotFound);
        It should_have_element_description = () => _ioError.InnerXml.ShouldNotBeNull();
        It should_have_the_parent_element_description = () => _ioError.ElementXml.ShouldNotBeNull();
    }
    public class When_object_property_name_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentObjectsProperty.xml");
            XafTypesInfo.Instance.RegisterEntity(typeof(ModelDifferenceObject));
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_an_info_object = () => {
            _ioError =
                _unitOfWork.FindObject<IOError>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, null);
            _ioError.ShouldNotBeNull();
        };

        It should_have_the_reason_of_the_fail = () => _ioError.Reason.ShouldEqual(FailReason.PropertyNotFound);
        It should_have_element_description = () => _ioError.InnerXml.ShouldNotBeNull();
        It should_have_the_parent_element_description = () => _ioError.ElementXml.ShouldNotBeNull();
    }
    public class When_collection_property_name_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentObjectCollectionProperty.xml");
            XafTypesInfo.Instance.RegisterEntity(typeof(ModelDifferenceObject));
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_an_info_object = () => {
            _ioError =
                _unitOfWork.FindObject<IOError>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, null);
            _ioError.ShouldNotBeNull();
        };

        It should_have_the_reason_of_the_fail = () => _ioError.Reason.ShouldEqual(FailReason.PropertyNotFound);
        It should_have_element_description = () => _ioError.InnerXml.ShouldNotBeNull();
        It should_have_the_parent_element_description = () => _ioError.ElementXml.ShouldNotBeNull();

    }
    public class When_collection_and_simple_and_object_property_names_does_not_exist : ExceptionHandlingSpecsBase {
        static Exception _exception;
        static IOError _ioError;
        static Stream _manifestResourceStream;
        static UnitOfWork _unitOfWork;

        Establish context = () => {
            _unitOfWork = new UnitOfWork(((XPObjectSpace)ObjectSpaceInMemory.CreateNew()).Session.DataLayer);
            _manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xpand.Tests.Xpand.IO.Resources.NonExistentAllTypesOfProperties.xml");
            XafTypesInfo.Instance.RegisterEntity(typeof(ModelDifferenceObject));
        };

        Because of = () => {
            _exception = Catch.Exception(() => new ImportEngine(ErrorHandling.CreateErrorObjects).ImportObjects(_manifestResourceStream, new XPObjectSpace(_unitOfWork)));
        };

        It should_not_throw_any_exceptions = () => _exception.ShouldBeNull();

        It should_create_3_info_object = () => _unitOfWork.GetCount<IOError>().ShouldEqual(3);

    }

}
