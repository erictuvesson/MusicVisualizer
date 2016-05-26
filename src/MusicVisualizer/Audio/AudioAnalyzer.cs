namespace MusicVisualizer.Audio
{
    using NAudio.Dsp;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using System.Diagnostics;

    public struct ComplexValue
    {
        public readonly float X;
        public readonly float Y;

        public ComplexValue(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public struct AudioSample
    {
        public readonly float MaxSample;
        public readonly float MinSample;

        public AudioSample(float maxSample, float minSample)
        {
            this.MaxSample = maxSample;
            this.MinSample = minSample;
        }
    }

    public struct AnalyzedAudio
    {
        public readonly ComplexValue[] FFT;
        public readonly ComplexValue[] SmoothFFT;

        public readonly AudioSample[] Samples;

        public AnalyzedAudio(ComplexValue[] fft, ComplexValue[] smoothFft, AudioSample[] samples)
        {
            this.FFT = fft;
            this.SmoothFFT = smoothFft;
            this.Samples = samples;
        }
    }

    public class AudioAnalyzer : IDisposable
    {
        public AnalyzedAudio CurrentAnalyzedAudio { get; private set; }

        public readonly int SamplesHistory;
        public float FFTSmoothness;

        public TimeSpan ThreadTargetElapsedTime;

        private readonly AudioPlayback audioPlayback;
        private readonly Thread thread;
        private bool threadRunning;

        private ConcurrentBag<AudioSample> lastSamples;
        private Complex[] lastFFT;
        private ComplexValue[] lastFFT2;
        private ComplexValue[] smoothFFT;

        public AudioAnalyzer(AudioPlayback audioPlayback, int samplesHistory = 3, float FFTSmoothness = 0.25f)
        {
            this.SamplesHistory = samplesHistory;
            this.FFTSmoothness = FFTSmoothness;

            this.ThreadTargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60);

            this.CurrentAnalyzedAudio = new AnalyzedAudio(new ComplexValue[0], new ComplexValue[0], new AudioSample[0]);
            this.lastSamples = new ConcurrentBag<AudioSample>();

            this.audioPlayback = audioPlayback;
            this.audioPlayback.MaximumCalculated += audioGraph_MaximumCalculated;
            this.audioPlayback.FftCalculated += audioGraph_FftCalculated;

            // TODO: If the sound stops it stops raporting the data

            this.threadRunning = true;
            this.thread = new Thread(new ThreadStart(thread_run));
            this.thread.Start();
        }

        private void audioGraph_MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            lastSamples.Add(new AudioSample(e.MaxSample, e.MinSample));
        }

        private void audioGraph_FftCalculated(object sender, FftEventArgs e)
        {
            lastFFT = e.Result;
        }

        private void thread_run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (threadRunning)
            {
                if (stopwatch.ElapsedMilliseconds > ThreadTargetElapsedTime.TotalMilliseconds)
                {
                    stopwatch.Restart();

                    if (lastFFT != null)
                    {
                        if (smoothFFT == null || smoothFFT.Length != lastFFT.Length)
                            smoothFFT = new ComplexValue[lastFFT.Length];
                        if (lastFFT2 == null || lastFFT2.Length != lastFFT.Length)
                            lastFFT2 = new ComplexValue[lastFFT.Length];

                        for (int i = 0; i < lastFFT.Length; i++)
                        {
                            var smoothX = MathHelper.Lerp(smoothFFT[i].X, lastFFT[i].X, FFTSmoothness); // TODO: Time
                            var smoothY = MathHelper.Lerp(smoothFFT[i].Y, lastFFT[i].Y, FFTSmoothness);
                            smoothFFT[i] = new ComplexValue(smoothX, smoothY);

                            lastFFT2[i] = new ComplexValue(lastFFT[i].X, lastFFT[i].Y);
                        }
                    }

                    var samples = new List<AudioSample>(CurrentAnalyzedAudio.Samples);
                    samples.InsertRange(0, lastSamples);

                    var newBag = new ConcurrentBag<AudioSample>();
                    Interlocked.Exchange(ref lastSamples, newBag);

                    CurrentAnalyzedAudio = new AnalyzedAudio(lastFFT2, smoothFFT, samples.Take(SamplesHistory).ToArray());
                }
            }
        }

        public void Dispose()
        {
            threadRunning = false;
            thread.Join();
        }
    }
}
