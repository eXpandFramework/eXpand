namespace Xpand.Tests.Xpand.IO {
//    [Subject("All Objects")]
//    public class When_Displaying_a_list_view:With_Isolations
//    {
//        static Frame _frame;
//        static IFrameCreationHandler frameCreationHandler;
//
//        Establish context = () => {
//            IArtifactHandler<User> artifactHandler = new TestAppLication<User>().Setup(null,user => {
//                new SerializationConfiguration(user.Session) {TypeToSerialize = typeof (User)};
//            });
//            artifactHandler.UnitOfWork.CommitChanges();            
//            frameCreationHandler = artifactHandler.WithArtiFacts(IOArtifacts).CreateListView();
//        };
//
//        
//        Because of = () => frameCreationHandler.CreateFrame(frame => _frame=frame);
//
//        It should_enable_export_action_if_there_is_an_associated_serialization_configuration=() => _frame.GetController<ViewController>().ExportToXmlAction.Active.ResultValue.ShouldBeTrue();
//    }

//    [Subject("All Objects")][Ignore]
//    public class When_exporting_a_list_of_objects:With_Isolations {
//        static SimpleActionExecuteEventArgs _simpleActionExecuteEventArgs;
//        static ISelectionContext _selectionContext;
//        static SerializationConfiguration _serializationConfiguration;
//        static SimpleAction _exportToXmlAction;
//        static Frame _frame;
//
//        Establish context = () =>
//        {
//            IArtifactHandler<User> artifactHandler = new TestAppLication<User>().Setup(null, user => {
//                _serializationConfiguration = new SerializationConfiguration(user.Session) { TypeToSerialize = typeof(User) };
//            });
//            artifactHandler.UnitOfWork.CommitChanges();
//            artifactHandler.WithArtiFacts(IOArtifacts).CreateListView().CreateFrame(frame => {
//                _exportToXmlAction = frame.GetController<ViewController>().ExportToXmlAction;
//            });
//            _selectionContext = Isolate.Fake.Instance<ISelectionContext>();
//            Isolate.WhenCalled(() => _selectionContext.SelectedObjects).WillReturn(new List<object>{_serializationConfiguration});            
//        };
//
//        Because of = () => {
//            _simpleActionExecuteEventArgs = new SimpleActionExecuteEventArgs(_exportToXmlAction, _selectionContext);
//            Isolate.Invoke.Event(() => _exportToXmlAction.Execute += null, null, _simpleActionExecuteEventArgs);
//        };
//
//        It should_select_the_configuration_to_export_with=() =>
//                                                          ((ListView)
//                                                           _simpleActionExecuteEventArgs.ShowViewParameters.CreatedView).CollectionSource.ObjectTypeInfo.Type.ShouldBeOfType(typeof(SerializationConfiguration));
//        It should_export_the_selected_objects;
//    }
}