using System;
using System.Diagnostics;
using System.Threading;

namespace LettuceIo.Dotnet.Base
{
    public static class Timer
    {
        public static readonly double TickLength = 1000f / Stopwatch.Frequency;

        public static void Sleep(TimeSpan timeSpan) => Sleep(timeSpan.TotalMilliseconds);

        private static double ElapsedHiRes(this Stopwatch stopwatch) => stopwatch.ElapsedTicks * TickLength;

        public static void Sleep(double milliSeconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            double diff;
            stopwatch.Start();
            while (true)
            {
                diff = milliSeconds - stopwatch.ElapsedHiRes();
                if (diff <= 0d) return;
                if (diff < 1d) Thread.SpinWait(10);
                else if (diff < 5d) Thread.SpinWait(100);
                else if (diff < 15d) Thread.Sleep(1);
                else Thread.Sleep(10);
            }
        }
    }
}