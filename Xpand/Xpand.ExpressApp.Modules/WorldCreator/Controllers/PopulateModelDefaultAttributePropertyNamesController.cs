using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.Controllers{
    public class PopulateModelDefaultAttributePropertyNamesController :PopulateController<IPersistentModelDefaultAttribute>{

        protected override string GetPredefinedValues(IModelMember wrapper){
            var currentObject = (IPersistentModelDefaultAttribute) View.CurrentObject;
            if (currentObject == null){
                return null;
            }

            IPersistentTypeInfo owner = currentObject.Owner;
            if (owner is IExtendedMemberInfo || owner is IPersistentMemberInfo){
                return GetModelProperties<IModelMember>();
            }
            if (owner is IPersistentClassInfo){
                return GetModelProperties<IModelClass>();
            }

            return null;
        }

        protected override Expression<Func<IPersistentModelDefaultAttribute, object>> GetPropertyName(){
            return x => x.PropertyName;
        }

        private static string GetModelProperties<T>() where T : IModelNode{
            return string.Join(";", typeof (T).GetPublicProperties().Where(x => x.CanWrite).Select(x => x.Name));
        }
    }
}