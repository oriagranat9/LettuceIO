using System;
using System.Reactive.Linq;
using LettuceIo.Dotnet.Core;

namespace LettuceIo.Dotnet.Base.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<Message> Limit(this IObservable<Message> observable, Limits limits)
        {
            IObservable<Message> ret = observable;
            if (limits.Duration != null) ret = ret.Limit((TimeSpan) limits.Duration);
            if (limits.Amount != null) ret = ret.Limit((long) limits.Amount);
            if (limits.SizeKB != null) ret = ret.Limit((double) limits.SizeKB);
            return ret;
        }

        public static IObservable<T> Limit<T>(this IObservable<T> observable, TimeSpan duration) =>
            observable.TakeUntil(Observable.Timer(duration));

        public static IObservable<T> Limit<T>(this IObservable<T> observable, long amount)
        {
            var i = 0L;
            return observable.TakeUntil(_ => i++ < amount);
        }

        public static IObservable<Message> Limit(this IObservable<Message> observable, double sizeKB)
        {
            var s = 0D;
            return observable.TakeUntil(message =>
            {
                s += message.SizeKB();
                return s < sizeKB;
            });
        }
    }
}