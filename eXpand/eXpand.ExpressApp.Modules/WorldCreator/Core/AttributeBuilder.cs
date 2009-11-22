using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Utils.ExpressionBuilder;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public class AttributeBuilder : Builder<CustomAttributeBuilder>, IAttributeBuilder {
        public static IAttributeBuilder BuildAttribute()
        {
            return new AttributeBuilder();
        }

        public void Define(IEnumerable<IPersistentAttributeInfo> persistentAttributeInfos, Action<CustomAttributeBuilder> afterCreated)
        {
            foreach (IPersistentAttributeInfo attributeInfo in persistentAttributeInfos)
            {
                AttributeInfo attribute = attributeInfo.Create();
                afterCreated.Invoke(new CustomAttributeBuilder(attribute.Constructor, attribute.InitializedArgumentValues));
            }
        }

    }

}
