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
        const string LoadWhenFiltered = "LoadWhenFiltered";
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var modelListViewGridViewOptions = ((IModelListViewLoadWhenFiltered)View.Model);
            if (IsReady(modelListViewGridViewOptions))
            {
                var filterController = Frame.GetController<FilterController>();
                filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
                SetDoNotLoadWhenFilterExistsCriteria();
            }
        }

        protected bool IsReady(IModelListViewLoadWhenFiltered modelListViewGridViewOptions)
        {
            return modelListViewGridViewOptions.LoadWhenFiltered &&
                   GetActiveFilter() == string.Empty;
        }

        protected abstract string GetActiveFilter();

        void FullTextFilterAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ParameterCurrentValue as string))
                SetDoNotLoadWhenFilterExistsCriteria();
            else
                ClearDoNotLoadWhenFilterExistsCriteria();
        }
        void SetDoNotLoadWhenFilterExistsCriteria()
        {
            IMemberInfo memberInfo = View.ObjectTypeInfo.KeyMember;
            Type memberType = memberInfo.MemberType;
            object o = memberType.IsValueType ? Activator.CreateInstance(memberType) : null;
            (View).CollectionSource.Criteria[LoadWhenFiltered] = new BinaryOperator(memberInfo.Name, o);
        }
        void ClearDoNotLoadWhenFilterExistsCriteria()
        {
            View.CollectionSource.Criteria[LoadWhenFiltered] = null;
        }

    }
}
