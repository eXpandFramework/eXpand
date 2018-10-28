using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ExcelImporter.Services;

namespace ExcelImporterTester.Module.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty(nameof(ImportName1))]
    public class Customer : BaseObject{
        // Fields...
        private GenderBase _gender;
        private string _importName1;
        int? _nullAbleInt;

        public int? NullAbleInt {
            get => _nullAbleInt;
            set => SetPropertyValue(nameof(NullAbleInt), ref _nullAbleInt, value);
        }

        public Customer(Session session) : base(session){
        }

        public GenderBase Gender{
            get => _gender;
            set => SetPropertyValue("Gender", ref _gender, value);
        }
        [DevExpress.Xpo.DisplayName("ImportName")]
        [RuleUniqueValue]
        public string ImportName1{
            get => _importName1;
            set => SetPropertyValue("ImportName1", ref _importName1, value);
        }

        string _hidden1;

        [VisibleInListView(false)]
        public string Hidden1{
            get => _hidden1;
            set => SetPropertyValue(nameof(Hidden1), ref _hidden1, value);
        }

        string _hidden4;
        [Browsable(false)]
        public string Hidden4{
            get => _hidden4;
            set => SetPropertyValue(nameof(Hidden4), ref _hidden4, value);
        }
        string _hidden2;

        [VisibleInLookupListView(false)]
        public string Hidden2{
            get => _hidden2;
            set => SetPropertyValue(nameof(Hidden2), ref _hidden2, value);
        }

        DateTime? _nullAbleDate;

        public DateTime? NullAbleDate {
            get => _nullAbleDate;
            set => SetPropertyValue(nameof(NullAbleDate), ref _nullAbleDate, value);
        }

        string _hidden3;
        [VisibleInExcelMap(false)]
        public string Hidden3{
            get => _hidden3;
            set => SetPropertyValue(nameof(Hidden3), ref _hidden3, value);
        }
    }
}