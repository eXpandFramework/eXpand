using System;
using DevExpress.Xpo;

namespace FeatureCenter.Module.PivotChart.HideChart {
    public class Updater : PivotChart.Updater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }

        protected override string GetName() {
            return "HideChart";
        }
    }
}