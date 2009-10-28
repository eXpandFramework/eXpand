using System;
using System.Reflection.Emit;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public interface IPropertyDefineBuilder {
        Type Type { get; }
        TypeBuilder TypeBuilder { get; }
        void DefineProperties();
    }
}