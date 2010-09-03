using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp.Core
{
    public static class DetailViewExtensions
    {
        public static ICollection<PropertyEditor> GetPropertyEditors(this XpandDetailView xpandDetailView, Type editorIsOfType) 
        {
            IEnumerable<PropertyEditor> editors = from editor in xpandDetailView.GetItems<PropertyEditor>()
                                                  where editor.Control != null&&editorIsOfType.IsAssignableFrom(editor.Control.GetType()) 
                                                  select editor;
            return editors.ToList();
        }
    }
}