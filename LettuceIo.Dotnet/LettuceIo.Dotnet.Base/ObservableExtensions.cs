using System;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;

namespace LettuceIo.Dotnet.Base
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Limit<T>(this IObservable<T> observable, JToken settings)
        {
            IObservable<T> ret = observable;
            if (settings.Value<bool>("timeLimit"))
            {
                var seconds = settings.Value<double>("duration");
                ret = ret.TakeUntil(Observable.Timer(TimeSpan.FromSeconds(seconds)));
            }

            if (settings.Value<bool>("countLimit"))
            {
                var i = 0;
                var countLimit = settings.Value<int>("count");
                ret = ret.TakeWhile(pattern => i++ < countLimit);
            }

            if (settings.Value<bool>("sizeLimit"))
            {
                throw new NotImplementedException("sizeLimit is not implemented.");
            }

            return ret;
        }
    }
}