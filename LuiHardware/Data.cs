using System;
using System.Collections.Generic;

namespace LuiHardware
{
    /// <summary>
    ///     Provides methods for the manipulation of numerical data,
    ///     including array math, statistics and optical calculations.
    /// </summary>
    public static class Data
    {
        public static void NormalizeArray(IList<int> arr, int maxval)
        {
            var max = AbsMax(arr);
            for (var i = 0; i < arr.Count; i++)
            {
                var denom = max * maxval;
                arr[i] = denom == 0 ? 0 : arr[i] / denom;
            }
        }

        public static void Accumulate(IList<int> a, IList<int> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] += b[i];
        }

        public static void Accumulate(IList<double> a, IList<int> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] += b[i];
        }

        public static void Accumulate(IList<double> a, IList<double> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] += b[i];
        }

        public static void Dissipate(IList<int> a, IList<int> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] -= b[i];
        }

        public static void Dissipate(IList<double> a, IList<double> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] -= b[i];
        }

        public static void Dissipate(IList<double> a, IList<int> b)
        {
            for (var i = 0; i < a.Count; i++)
                a[i] -= b[i];
        }

        public static int[] DummySpectrum(double t)
        {
            var data = new int[1024];
            return data;
        }

        public static int AbsMax(IList<int> arr)
        {
            var curmax = int.MinValue;
            foreach (var i in arr)
                curmax = Math.Abs(arr[i]) > curmax ? Math.Abs(arr[i]) : curmax;
            return curmax;
        }

        public static void DivideArray(IList<int> arr, int N)
        {
            for (var i = 0; i < arr.Count; i++) arr[i] /= N;
        }

        public static void DivideArray(IList<double> arr, double N)
        {
            for (var i = 0; i < arr.Count; i++) arr[i] /= N;
        }

        public static void MultiplyArray(IList<int> arr, int N)
        {
            for (var i = 0; i < arr.Count; i++) arr[i] *= N;
        }

        public static void MultiplyArray(IList<double> arr, double N)
        {
            for (var i = 0; i < arr.Count; i++) arr[i] *= N;
        }

        public static void ColumnSum(IList<double> accumulator, double[] matrix)
        {
            for (var i = 0; i < matrix.Length / accumulator.Count; i++)
            {
                var start = i * accumulator.Count;
                Accumulate(accumulator, new ArraySegment<double>(matrix, start, accumulator.Count));
            }
        }

        public static void ColumnSum(IList<int> accumulator, int[] matrix)
        {
            for (var i = 0; i < matrix.Length / accumulator.Count; i++)
            {
                var start = i * accumulator.Count;
                Accumulate(accumulator, new ArraySegment<int>(matrix, start, accumulator.Count));
            }
        }

        public static void ColumnSum(IList<double> accumulator, int[] matrix)
        {
            for (var i = 0; i < matrix.Length / accumulator.Count; i++)
            {
                var start = i * accumulator.Count;
                Accumulate(accumulator, new ArraySegment<int>(matrix, start, accumulator.Count));
            }
        }

        public static void ColumnSumInPlace(int[] matrix, int width)
        {
            var AllButLastRow = new ArraySegment<int>(matrix, 0, matrix.Length - width);
            var LastRow = new ArraySegment<int>(matrix, matrix.Length - width, width);
            for (var i = 0; i < AllButLastRow.Count / width; i++)
            {
                var start = i * width;
                Accumulate(LastRow, new ArraySegment<int>(matrix, start, width));
            }
        }

        public static T[,] Transpose<T>(T[,] Matrix)
        {
            var TransposedMatrix = new T[Matrix.GetLength(1), Matrix.GetLength(0)];
            for (var i = 0; i < Matrix.GetLength(0); i++)
                for (var j = 0; j < Matrix.GetLength(1); j++)
                    TransposedMatrix[j, i] = Matrix[i, j];
            return TransposedMatrix;
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="Spectrum"></param>
        /// <returns>LD</returns>
        public static double[] Y(IList<double> Spectrum)
        {
            var Y = new double[Spectrum.Count];
            for (var i = 0; i < Y.Length; i++)
                Y[i] = Spectrum[i];
            return Y;
        }



        /// <summary>
        /// Computes S from +/- beta transmitted intensity, and dark counts.
        /// </summary>
        /// <param name="PlusB"></param>
        /// <param name="MinusB"></param>
        /// <param name="Dark"></param>
        /// <returns>LD</returns>
        public static double[] S(IList<double> PlusB, IList<double> MinusB, IList<double> Dark)
        {
            var S = new double[PlusB.Count];
            for (var i = 0; i < S.Length; i++)
                S[i] = (PlusB[i] - MinusB[i]) / (PlusB[i] + MinusB[i] - 2 * Dark[i]);
            return S;
        }

        /// <summary>
        /// Compute the extinction for crossed polarizers
        /// </summary>
        /// <param name="SmallAngleSpectrum"></param>
        /// <param name="CrossedSpectrum"></param>
        /// <param name="beta"></param>
        /// <returns>Extinction</returns>
        public static double[] Extinction(IList<double> SmallAngleSpectrum, IList<double> CrossedSpectrum, double beta)
        {

            double angle = Math.PI * (90.0 + beta) / 180.0;
            double sinAngle = Math.Sin(angle);

            var AlignedSpectrum = new double[SmallAngleSpectrum.Count];
            for (var i = 0; i < AlignedSpectrum.Length; i++)
            {
                AlignedSpectrum[i] = (SmallAngleSpectrum[i] - CrossedSpectrum[i])/(sinAngle*sinAngle);
            }

            var Extinction = new double[SmallAngleSpectrum.Count];
            for (var i =0; i < Extinction.Length; i++)
            {
                Extinction[i] = CrossedSpectrum[i] / AlignedSpectrum[i];
            }

            return Extinction;
        }

        /// <summary>
        ///     Computes OD from Sample and Blank counts, subtracting Dark counts from both.
        /// </summary>
        /// <param name="Sample"></param>
        /// <param name="Blank"></param>
        /// <param name="Dark"></param>
        /// <returns>Optical density</returns>
        public static double[] OpticalDensity(IList<int> Sample, IList<int> Blank, IList<int> Dark)
        {
            var OD = new double[Sample.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = -Math.Log10((Sample[i] - Dark[i]) / (double)(Blank[i] - Dark[i]));
            return OD;
        }

        /// <summary>
        ///     Computes OD from Sample and Blank counts. Assumes Dark has been applied or is zero.
        /// </summary>
        /// <param name="Sample"></param>
        /// <param name="Blank"></param>
        /// <returns>Optical density</returns>
        public static double[] OpticalDensity(IList<int> Sample, IList<int> Blank)
        {
            var OD = new double[Sample.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = -Math.Log10(Sample[i] / (double)Blank[i]);
            return OD;
        }

        /// <summary>
        ///     Computes delta OD from Ground, Trans and Dark counts.
        /// </summary>
        /// <param name="Ground"></param>
        /// <param name="Trans"></param>
        /// <param name="Dark"></param>
        /// <returns>Delta OD</returns>
        public static double[] DeltaOD(IList<int> Ground, IList<int> Trans, IList<int> Dark)
        {
            var OD = new double[Ground.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10((Ground[i] - Dark[i]) / (double)(Trans[i] - Dark[i]));
            return OD;
        }

        /// <summary>
        ///     Computes delta OD from Ground, Trans and Dark counts.
        /// </summary>
        /// <param name="Ground"></param>
        /// <param name="Trans"></param>
        /// <param name="Dark"></param>
        /// <returns>Delta OD</returns>
        public static double[] DeltaOD(IList<double> Ground, IList<double> Trans, IList<double> Dark)
        {
            var OD = new double[Ground.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10((Ground[i] - Dark[i]) / (Trans[i] - Dark[i]));
            return OD;
        }

        /// <summary>
        ///     Computes delta OD from Ground and Trans. Assumes Dark has been applied or is zero.
        /// </summary>
        /// <param name="Ground"></param>
        /// <param name="Trans"></param>
        /// <returns>Delta OD</returns>
        public static double[] DeltaOD(IList<int> Ground, IList<int> Trans)
        {
            var OD = new double[Ground.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10(Ground[i] / (double)Trans[i]);
            return OD;
        }

        public static double[] DeltaOD(IList<double> Ground, IList<double> Trans)
        {
            var OD = new double[Ground.Count];
            for (var i = 0; i < OD.Length; i++)
                OD[i] = Math.Log10(Ground[i] / Trans[i]);
            return OD;
        }

        public static double[] Gaussian(int n, double scale, double mean, double sigma)
        {
            var g = new double[n];
            for (var i = 0; i < g.Length; i++)
            {
                var x = (double)i * 1024 / n;
                g[i] = scale * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(sigma, 2)));
            }

            return g;
        }

        public static int[] Gaussian(int n, int scale, double mean, double sigma)
        {
            var g = new int[n];
            for (var i = 0; i < g.Length; i++)
            {
                var x = (double)i * 1024 / n;
                g[i] = (int)(scale * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(sigma, 2))));
            }

            return g;
        }

        public static double[] Calibrate(IList<double> channel, double slope, double intercept)
        {
            var cal = new double[channel.Count];
            for (var i = 0; i < channel.Count; i++) cal[i] = slope * channel[i] + intercept;
            return cal;
        }

        public static double[] Calibrate(int n, double slope, double intercept)
        {
            var cal = new double[n];
            for (var i = 0; i < n; i++) cal[i] = slope * (i + 1) + intercept;
            return cal;
        }

        /// <summary>
        ///     Linear least squares fit of variables in x to variables in y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Tuple containing slope, y-intercept and R^2</returns>
        public static Tuple<double, double, double> LinearLeastSquares(IList<double> x, IList<double> y)
        {
            double n = x.Count;
            double xysum = 0;
            double xsum = 0;
            double xsqsum = 0;
            double ysum = 0;
            double ysqsum = 0;
            for (var i = 0; i < x.Count; i++)
            {
                xysum += x[i] * y[i];
                xsum += x[i];
                xsqsum += Math.Pow(x[i], 2);
                ysum += y[i];
                ysqsum += Math.Pow(y[i], 2);
            }

            var xyhat = xysum / n;
            var xhat = xsum / n;
            var yhat = ysum / n;
            var xsqhat = xsqsum / n;
            var ysqhat = ysqsum / n;

            var cov = xyhat - xhat * yhat;
            var xvar = xsqhat - Math.Pow(xhat, 2);
            var yvar = ysqhat - Math.Pow(yhat, 2);

            var slope = cov / xvar;
            var yint = yhat - slope * xhat;
            var rsq = Math.Pow(cov / Math.Sqrt(xvar * yvar), 2);

            return Tuple.Create(slope, yint, rsq);
        }

        public static int[] Uniform(int n, int scale)
        {
            var R = new Random();
            var A = new int[n];
            //for (int i = 0; i < n; i++) A[i] = (int)Math.Round(R.NextDouble() * (double)scale);
            for (var i = 0; i < n; i++) A[i] = R.Next(scale);
            return A;
        }

        public static double[] Uniform(int n, double scale)
        {
            var R = new Random();
            var A = new double[n];
            for (var i = 0; i < n; i++) A[i] = Math.Round(R.NextDouble() * scale);
            return A;
        }

        public static void CumulativeMovingAverage(IList<double> CMA, IList<double> X, int n)
        {
            for (var i = 0; i < CMA.Count; i++)
                CMA[i] = (X[i] + n * CMA[i]) / (n + 1);
        }

        public static void CumulativeMovingAverage(IList<double> CMA, IList<int> X, int n)
        {
            for (var i = 0; i < CMA.Count; i++)
                CMA[i] = (X[i] + n * CMA[i]) / (n + 1);
        }
    }
}