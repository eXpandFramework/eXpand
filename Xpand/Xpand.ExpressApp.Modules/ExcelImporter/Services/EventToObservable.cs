using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using Fasterflect;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ExcelImporter.Services {
    public static class EventToObservable {
        public static IObservable<T> WhenProgressChanged<T>(this Progress<T> item) {
            return Observable.FromEventPattern<EventHandler<T>, T>(h => item.ProgressChanged += h,
                    h => item.ProgressChanged -= h)
                .Select(pattern => pattern.EventArgs);
        }

        public static IObservable<Unit> ToUnit(this IObservable<object> source) {
            return source.To<Unit>();
        }

        public static IObservable<T> To<T>(this IObservable<object> source) {
            return source.Select(o => (T) typeof(T).CreateInstance());
        }



    }
}
