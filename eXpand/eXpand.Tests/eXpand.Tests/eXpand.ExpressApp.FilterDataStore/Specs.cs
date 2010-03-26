using DevExpress.Xpo.DB;
using eXpand.ExpressApp.FilterDataStore;
using eXpand.ExpressApp.FilterDataStore.Core;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.FilterDataStore {
    
    public class When_filtering_a_select_statement_with_a_provider_with_value_of_type_enumerable {
        static FilterDataStoreModule _filterDataStoreModule;
        static SelectStatement _selectStatement;

        Establish context = () => {
            var filterProviderBase = Isolate.Fake.Instance<FilterProviderBase>();
            Isolate.WhenCalled(() => filterProviderBase.FilterValue).WillReturn(new[]{1,2,3});
            Isolate.WhenCalled(() => filterProviderBase.FilterMemberName).WillReturn("TestMember");
            Isolate.WhenCalled(() => filterProviderBase.Name).WillReturn("TestProvider");
            Isolate.Fake.StaticMethods(typeof(FilterProviderManager));
            var filterProviderCollection = new FilterProviderCollection {filterProviderBase};
            Isolate.WhenCalled(() => FilterProviderManager.Providers).WillReturn(filterProviderCollection);
            Isolate.WhenCalled(() => FilterProviderManager.GetFilterProvider(null,StatementContext.Select)).WillReturn(filterProviderBase);
            _selectStatement = new  SelectStatement {Alias = "Alias"};
            _filterDataStoreModule = new FilterDataStoreModule();
            Isolate.WhenCalled(() => _filterDataStoreModule.FindClassNameInDictionary(null)).ReturnRecursiveFake();
            Isolate.WhenCalled(() => _filterDataStoreModule.Application).ReturnRecursiveFake();
        };

        Because of = () => _filterDataStoreModule.ApplyCondition(_selectStatement);
        It should_create_an_or_statement_with_all_values = () => _selectStatement.Condition.ToString().ShouldEqual("Alias.{TestMember} = '1' Or Alias.{TestMember} = '2' Or Alias.{TestMember} = '3'");
    }
}