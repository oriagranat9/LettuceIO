using System;
using System.Reactive.Subjects;
using LettuceIo.Dotnet.Core;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace LettuceIo.Dotnet.Base.Actions
{
    public class Publish : IAction
    {
        public IObservable<ActionMetrics> Stats => _statsSubject;

        #region fields

        private IConnectionFactory _connectionFactory;
        private readonly Limits _limits;
        private readonly string _exchange;
        private readonly string _folderPath;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ISubject<ActionMetrics> _statsSubject = new Subject<ActionMetrics>();

        #endregion

        public Publish(IConnectionFactory connectionFactory, Limits limits, string exchange, string folderPath,
            JsonSerializerSettings serializerSettings)
        {
            _connectionFactory = connectionFactory;
            _limits = limits;
            _exchange = exchange;
            _folderPath = folderPath;
            _serializerSettings = serializerSettings;
        }


        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}