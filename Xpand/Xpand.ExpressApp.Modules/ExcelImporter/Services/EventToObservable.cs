using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using Fasterflect;

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


        public static IObservable<EventPattern<EventArgs>> WhenDisposed(this IObjectSpace objectSpace) {
            return Observable.Empty<EventPattern<EventArgs>>();
//            return Observable.FromEventPattern<EventHandler, EventArgs>(
//                h => objectSpace.Disposed += h, h => objectSpace.Disposed -= h);

        }

        public static IObservable<EventPattern<CancelEventArgs>> WhenCommiting(this IObjectSpace objectSpace) {
            return Observable.FromEventPattern<EventHandler<CancelEventArgs>, CancelEventArgs>(
                h => objectSpace.Committing += h, h => objectSpace.Committing -= h)
                .TakeUntil(objectSpace.WhenDisposed());
        }

    }
}
