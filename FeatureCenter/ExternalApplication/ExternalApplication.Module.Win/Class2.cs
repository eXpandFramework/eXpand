//using DevExpress.Xpo;
//
//public class Estados : XPLiteObject
//{
// 
//    private EstadosKey _Key;
//    [Persistent(""), Key(false)]
//    public EstadosKey Key {
//        get { return _Key; }
//        set { SetPropertyValue("Key", ref _Key, value); }
//    }
//
//    [Association("FK_Municipios_Estados")]
//    public XPCollection<Municipios> MunicipiosIdPaiss {
//        get { return GetCollection<Municipios>("MunicipiosIdPaiss"); }
//    }
//
//}
//public struct EstadosKey {
//    private int _IdEstado;
//    [Persistent("IdEstado")]
//    public int IdEstado {
//        get { return _IdEstado; }
//        set {
//            _IdEstado = value;
//        }
//    }
//    private int _IdPais;
//    [Persistent("IdPais")]
//    public int IdPais {
//        get { return _IdPais; }
//        set {
//            _IdPais = value;
//        }
//    }
//}
//public class Municipios : XPLiteObject {
//    int _IdMunicipio;
//
//    [Key(true)]
//    [Persistent("IdMunicipio")]
//    public int IdMunicipio {
//        get { return _IdMunicipio; }
//        set { SetPropertyValue("IdMunicipio", ref _IdMunicipio, value); }
//    }
//
//    private Estados _Estados;
//
//    [Persistent("")]
//    [AssociationAttribute("FK_Municipios_Estados")]
//    public Estados Estados {
//        get { return _Estados; }
//        set { SetPropertyValue("Estados", ref _Estados, value); }
//    }
//}



//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using DevExpress.Xpo;
//
//namespace TestAssembly {
//    [Persistent("Estados")]
//    [DefaultClassOptions]
//    public class Estados : XPLiteObject {
//        [Key(false)] [Size(2)] [Persistent("")] 
//        public EstadosKeyStruct EstadosKeyStruct;
//        string _Nombre;
//
//        public Estados(Session session) : base(session) {
//        }
//
//        [Size(50)]
//        [Persistent("Nombre")]
//        public string Nombre {
//            get { return _Nombre; }
//            set { SetPropertyValue("Nombre", ref _Nombre, value); }
//        }
//
//
//        [Association("FK_Municipios_Estados")]
//        public XPCollection<Municipios> MunicipiosIdPaiss {
//            get { return GetCollection<Municipios>("MunicipiosIdPaiss"); }
//        }
//    }
//    public struct EstadosKeyStruct
//    {
//        [Size(2)]
//        [Persistent("IdEstado")]
//        public string IdEstado { get; set; }
//
//        [Size(2)]
//        [Association("FK_Estados_Paises")]
//        [Persistent("IdPais")]
//        public Paises IdPais { get; set; }
//    }
//    [Persistent("Paises")]
//    [DefaultClassOptions]
//    public class Paises : XPLiteObject
//    {
//        string _CodArea;
//        bool _EsDefault;
//        string _IdPais;
//        string _Nombre;
//
//        public Paises(Session session)
//            : base(session)
//        {
//        }
//
//        [Association("FK_Estados_Paises")]
//        public XPCollection<Estados> EstadosKeyStructIdPaiss
//        {
//            get { return GetCollection<Estados>("EstadosKeyStructIdPaiss"); }
//        }
//
//        [Key(false)]
//        [Size(2)]
//        [Persistent("IdPais")]
//        public string IdPais
//        {
//            get { return _IdPais; }
//            set { SetPropertyValue("IdPais", ref _IdPais, value); }
//        }
//
//        [RuleRequiredField("RuleRequired for Nombre at Paises", "Save")]
//        [Size(100)]
//        [Persistent("Nombre")]
//        public string Nombre
//        {
//            get { return _Nombre; }
//            set { SetPropertyValue("Nombre", ref _Nombre, value); }
//        }
//
//        [Size(3)]
//        [Persistent("CodArea")]
//        public string CodArea
//        {
//            get { return _CodArea; }
//            set { SetPropertyValue("CodArea", ref _CodArea, value); }
//        }
//
//        [RuleRequiredField("RuleRequired for EsDefault at Paises", "Save")]
//        [Persistent("EsDefault")]
//        public bool EsDefault
//        {
//            get { return _EsDefault; }
//            set { SetPropertyValue("EsDefault", ref _EsDefault, value); }
//        }
//    }
//    [Persistent("Municipios")]
//    [DefaultClassOptions]
//    public class Municipios : XPLiteObject
//    {
//        Estados _IdEstado;
//        int _IdMunicipio;
//
//        string _Nombre;
//
//        public Municipios(Session session)
//            : base(session)
//        {
//        }
//
//        [Key(true)]
//        [Persistent("IdMunicipio")]
//        public int IdMunicipio
//        {
//            get { return _IdMunicipio; }
//            set { SetPropertyValue("IdMunicipio", ref _IdMunicipio, value); }
//        }
//
//        [RuleRequiredField("RuleRequired for Nombre at Municipios", "Save")]
//        [Size(100)]
//        [Persistent("Nombre")]
//        public string Nombre
//        {
//            get { return _Nombre; }
//            set { SetPropertyValue("Nombre", ref _Nombre, value); }
//        }
//
//        [RuleRequiredField("RuleRequired for IdEstado at Municipios", "Save")]
//        [Size(2)]
//                [DevExpress.Xpo.AssociationAttribute("FK_Municipios_Estados")]
//        [Persistent("")]
//        public Estados IdEstado
//        {
//            get { return _IdEstado; }
//            set { SetPropertyValue("IdEstado", ref _IdEstado, value); }
//        }
//    }
//
//}
//
//
//
