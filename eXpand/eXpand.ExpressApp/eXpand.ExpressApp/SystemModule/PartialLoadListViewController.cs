using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.SystemModule {
    public class PartialLoadListViewController : ViewController
    {
        private XPCollection collection;
        private CollectionSourceBase collectionSource;
        private const int DefaultPartSize = 100;
        private int _partCountCore = 1;
        private void ResetPartCount()
        {
            _partCountCore = 1;
        }
        private void UpdatePartCount()
        {
            _partCountCore++;
        }
        private void InitCollection()
        {
            collectionSource = ((ListView)View).CollectionSource;
            collection = (XPCollection)collectionSource.Collection;
        }
        private void UpdateCollection()
        {
            collection.LoadingEnabled = false;

            decimal totalCount = Convert.ToDecimal(ObjectSpace.Session.Evaluate<DomainObject1>(CriteriaOperator.Parse("Count()"), ObjectSpace.CombineCriteria(collectionSource.Criteria.GetValues().ToArray())));

            int realCount = DefaultPartSize * PartCount;
            if (totalCount >= realCount)
            {
                collection.TopReturnedObjects = realCount;
            }
            collection.LoadingEnabled = true;
        }
        private void saNextPart_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            UpdatePartCount();
            UpdateCollection();
        }
        protected override void OnActivated()
        {
            ResetPartCount();
            InitCollection();
            UpdateCollection();
            base.OnActivated();
        }
        public int PartCount
        {
            get
            {
                return _partCountCore;
            }
        }
        public PartialLoadListViewController()
        {
            collectionSource = null;
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;
        }
    }
}