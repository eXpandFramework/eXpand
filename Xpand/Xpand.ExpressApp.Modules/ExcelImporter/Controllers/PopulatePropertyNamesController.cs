//using System;
//using System.Linq.Expressions;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Model;
//using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
//using Xpand.Persistent.Base.General.Controllers;
//
//namespace Xpand.ExpressApp.ExcelImporter.Controllers{
//    public class PopulatePropertyNamesController : PopulateController<ExcelColumnMap>{
//        private ExcelImport _excelImport;
//
//        protected override void OnActivated(){
//            if (View is ListView listView){
//                _excelImport = (ExcelImport) ((PropertyCollectionSource) listView.CollectionSource).MasterObject;
//                base.OnActivated();
//            }
//        }
//
//        protected override string GetPredefinedValues(IModelMember wrapper){
//            
//            return string.Join(";", _excelImport.TypePropertyNames) ;
//        }
//
//        protected override Expression<Func<ExcelColumnMap, object>> GetPropertyName(){
//            return map => map.PropertyName;
//        }
//    }
//}