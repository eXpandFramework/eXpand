using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public class ParseCriteriaScope : IDisposable
    {
        private const String objectTag = "NHObject";

        [ThreadStatic]
        private static IObjectSpace currentObjectSpace;
        private static ITypesInfo typesInfo;
        private IObjectSpace objectSpace;
        private IObjectSpace prevObjectSpace;
        private Boolean isDisposed;

        private static void CriteriaOperator_UserValueToString(Object sender, UserValueProcessingEventArgs e)
        {
            if (!e.Handled && (e.Value != null))
            {
                ITypeInfo typeInfo = typesInfo.FindTypeInfo(e.Value.GetType());
                if ((typeInfo != null) && typeInfo.IsPersistent)
                {

                    IObjectSpace objectSpace = null;
                    if (e.Value is IObjectSpaceLink)
                    {
                        objectSpace = ((IObjectSpaceLink)e.Value).ObjectSpace;
                    }
                    if (objectSpace == null)
                    {
                        objectSpace = ParseCriteriaScope.currentObjectSpace;
                    }
                    if (objectSpace != null)
                    {
                        e.Data = objectSpace.GetObjectHandle(e.Value);
                        e.Tag = objectTag;
                        e.Handled = true;
                    }
                    else
                    {
                        e.Data = ObjectHandleHelper.CreateObjectHandle(typesInfo, typeInfo.Type, NHObjectSpace.GetKeyValueAsString(typesInfo, e.Value));
                        e.Tag = objectTag;
                        e.Handled = true;
                    }
                }
            }
        }

        private static void CriteriaOperator_UserValueParse(Object sender, UserValueProcessingEventArgs e)
        {
            if (!e.Handled && !String.IsNullOrWhiteSpace(e.Data) && (e.Tag == objectTag) && (ParseCriteriaScope.currentObjectSpace != null))
            {
                try
                {
                    e.Value = ParseCriteriaScope.currentObjectSpace.GetObjectByHandle(e.Data);
                    e.Handled = true;
                }
                catch
                {
                    e.Value = e.Data;
                }
            }
        }
        static ParseCriteriaScope()
        {
            CriteriaOperator.UserValueToString += CriteriaOperator_UserValueToString;
            CriteriaOperator.UserValueParse += CriteriaOperator_UserValueParse;
        }
        public ParseCriteriaScope(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
            prevObjectSpace = ParseCriteriaScope.currentObjectSpace;
            ParseCriteriaScope.currentObjectSpace = objectSpace;
        }
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (ParseCriteriaScope.currentObjectSpace != objectSpace)
                {
                    throw new InvalidOperationException("Incorrect ParseCriteriaScope usage detected.");
                }
                ParseCriteriaScope.currentObjectSpace = prevObjectSpace;
                prevObjectSpace = null;
                objectSpace = null;
            }
        }
        internal static void Init(ITypesInfo typesInfo)
        {
            ParseCriteriaScope.typesInfo = typesInfo;
        }

    }
}
