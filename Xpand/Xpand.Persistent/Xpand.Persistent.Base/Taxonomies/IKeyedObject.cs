using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IKeyedObject : IXPObject
    {
        string Value { get; set;}
        string Key { get; set; }
        KeyedObjectValidity Validity { get; set;}
    }
}