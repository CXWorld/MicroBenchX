using System;

namespace MicroBenchX
{
    public class GaussianRandom
    {
        Random _random;
        double _mean;
        double _std;

        public GaussianRandom()
        {
            _random = new Random();
            _mean = 0.0;
            _std = 1.0;
        }

        public GaussianRandom(double mean, double std)
        {
            _random = new Random();
            _mean = mean;
            _std = std;
        }

        public double GetNext()
        {
            //Zwei gleichverteilte Zufallsvariablen
            double u1 = _random.NextDouble();
            double u2 = _random.NextDouble();

            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);

            return _mean + _std * randStdNormal;
        }
    }
}
