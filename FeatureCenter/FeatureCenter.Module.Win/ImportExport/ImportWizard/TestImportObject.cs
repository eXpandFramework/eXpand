﻿using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.ImportExport
{
    
    
    [DisplayFeatureModel("TestImportObject_ListView", "ImportWizard")]
    public class TestImportObject : BaseObject
    {
        private string _Code;

        [DisplayName("Code")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }
        
        private String _Name;
        [Size(255)]
        [DisplayName("Name")]
        public String Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }


        private int _Age;

        [DisplayName("Age")]
        public int Age
        {
            get { return _Age; }
            set { SetPropertyValue("Age", ref _Age, value); }
        }


        private TestGroupObject _Group;

        [DisplayName("Group")]
        public TestGroupObject Group
        {
            get { return _Group; }
            set { SetPropertyValue("Group", ref _Group, value); }
        }


        public TestImportObject(Session session) : base(session)
        {
        }

        public TestImportObject()
        {
        }
    }
}