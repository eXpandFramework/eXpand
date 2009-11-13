using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;
using DevExpress.DXCore.Controls.Xpo;
using eXpandAddIns.Extensioons;

namespace eXpandAddIns
{
    [Flags]
    public enum Avaliabiltity
    {
        None=0,
        XpoOnePart=1,
        XpoManyPart=2
    }
    public class AvaliabiltityCalulator
    {
        
        private readonly LanguageElement languageElement;

        public AvaliabiltityCalulator(LanguageElement languageElement)
        {
            this.languageElement = languageElement;
        }
        public bool IsCollection
        {
            get
            {
                return IsActiveProperty && (languageElement.Is(typeof(ICollection)) || languageElement.Is(typeof(ICollection<>)));
            }
        }
        public bool IsIXPSimpleObject
        {
            get
            {
                return IsActiveProperty && languageElement.Is(typeof(IXPSimpleObject));
            }
        }

        public Avaliabiltity Avaliability
        {
            get {
                if (IsActiveProperty && ((CodeElement)languageElement).FindAttribute(typeof(AssociationAttribute)) != null)
                {
                    if (IsCollection)
                        return Avaliabiltity.XpoManyPart|Avaliabiltity.XpoOnePart;
                    if (IsIXPSimpleObject)
                        return Avaliabiltity.XpoManyPart;
                }
                return Avaliabiltity.None;

            }
        }
        public bool IsActiveProperty
        {
            get
            {
                return languageElement == CodeRush.Source.ActiveProperty;
            }
        }

    }
}