using System;
using System.Diagnostics;
using System.Threading;

namespace LettuceIo.Dotnet.Base
{
    public class Timer
    {
        private static readonly double TickLength = 1000f / Stopwatch.Frequency;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public void Sleep(double milliSeconds)
        {
            _stopwatch.Restart();
            while (true)
            {
                var diff = milliSeconds - _stopwatch.ElapsedTicks * TickLength;
                if (diff <= 0d) break;
                if (diff < 1d) Thread.SpinWait(10);
                else if (diff < 5d) Thread.SpinWait(100);
                else if (diff < 15d) Thread.Sleep(1);
                else Thread.Sleep(10);
            }
            _stopwatch.Stop();
        }

        public void Sleep(TimeSpan timeSpan) => Sleep(timeSpan.TotalMilliseconds);
    }
}