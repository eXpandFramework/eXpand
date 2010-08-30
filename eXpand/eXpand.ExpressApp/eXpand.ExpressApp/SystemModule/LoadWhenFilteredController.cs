using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassLoadWhenFiltered:IModelNode
    {
        [Category("eXpand")]
        [Description("Only loads listview records when a filter is present")]
        bool LoadWhenFiltered { get; set; }    
    }
    [ModelInterfaceImplementor(typeof(IModelClassLoadWhenFiltered), "ModelClass")]
    public interface IModelListViewLoadWhenFiltered : IModelClassLoadWhenFiltered
    {
    }

    public abstract class LoadWhenFilteredController:ViewController<ListView>
    {
        protected const string LoadWhenFiltered = "LoadWhenFiltered";
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
            if (IsReady())
            {
                var filterController = Frame.GetController<FilterController>();
                filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
                View.CollectionSource.Criteria[LoadWhenFiltered]=GetDoNotLoadWhenFilterExistsCriteria();
            }
        }

        protected bool IsReady()
        {
            var modelListViewGridViewOptions = ((IModelListViewLoadWhenFiltered)View.Model);
            return modelListViewGridViewOptions.LoadWhenFiltered &&string.IsNullOrEmpty(GetActiveFilter());
        }

        protected abstract string GetActiveFilter();

        void FullTextFilterAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ParameterCurrentValue as string))
                View.CollectionSource.Criteria[LoadWhenFiltered]=GetDoNotLoadWhenFilterExistsCriteria();
            else
                ClearDoNotLoadWhenFilterExistsCriteria();
        }

        protected BinaryOperator GetDoNotLoadWhenFilterExistsCriteria()
        {
            IMemberInfo memberInfo = View.ObjectTypeInfo.KeyMember;
            Type memberType = memberInfo.MemberType;
            object o = memberType.IsValueType ? Activator.CreateInstance(memberType) : null;
            return new BinaryOperator(memberInfo.Name, o);
        }

        protected void ClearDoNotLoadWhenFilterExistsCriteria()
        {
            View.CollectionSource.Criteria[LoadWhenFiltered] = null;
        }

    }
}
