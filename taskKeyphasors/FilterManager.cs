using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Filtering.Median;
using MathNet.Filtering.IIR;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Media.Effects;
using System.Xml.Resolvers;
using System.Security.Cryptography.X509Certificates;

namespace taskKeyphasors
{
    //константы 
    //filterManager.IntervalFilter(data, 1000, 0.40);
    //data = filterManager.IntervalDrop(data, 1000);
    //AlignValues(interval, 0.3);
    //resultSignal = DoSqareSignal(resultSignal, 0.80);
    //resultSignal = MyMedianFilter(resultSignal, 5);


    //для новых
    //data = filterManager.IntervalFilter(data, 50, 0.40);
    //data = filterManager.FillSpaces(data, 200);
   /* const double procentOfAmplitude = 0.8;
    const int windowSize = 3;
    const double procentOfLength = 0.3;*/


    class FilterManager
    {
        public FilterManager() { }

        public List<double> MedianFilter(IList<double> signal, int windowSize)
        {
            OnlineMedianFilter medianFilter = new OnlineMedianFilter(windowSize);

            List<double> filteredSignal = new List<double>();
            filteredSignal = medianFilter.ProcessSamples(signal.ToArray()).ToList();

            return filteredSignal;

        }

        public double[] MedianFilter(double[] signal, int windowSize)
        {
            OnlineMedianFilter medianFilter = new OnlineMedianFilter(windowSize);

            double[] filteredSignal = medianFilter.ProcessSamples(signal);

            return filteredSignal;

        }

        public double[] MyMedianFilter(double[] signal, int windowSize)
        {
            MedianFilter medianFilter = new MedianFilter();

            double[] filteredSignal = medianFilter.Execute(signal, windowSize);

            return filteredSignal;

        }


        public double[] AverageValueFilter(double[] signal, int countOfNeighbors)
        {
            List<double> filteredSignal = new List<double>();
            List<double> neighbors = new List<double>(countOfNeighbors);
            int oneSideNeighbors = (int)(countOfNeighbors / 2);

            double median = 0;


            for (int i = 0; i < signal.Length; i++)
            {
                if (i < oneSideNeighbors)// i в начале  
                {
                    median = FindAverageOnInterval(signal, 0, oneSideNeighbors + (oneSideNeighbors - i));// тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else if (i > (signal.Length - oneSideNeighbors)) // i в конце
                {
                    // тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else // i в середине
                {
                    median = FindAverageOnInterval(signal, i - oneSideNeighbors, i + oneSideNeighbors);
                }

                filteredSignal.Add(median);
            }
            return filteredSignal.ToArray();

        }

        private double FindAverageOnInterval(IList<double> list, int startIndex, int endIndex)
        {
            List<double> data = new List<double>();

            for (int i = startIndex; i < endIndex; i++)
            {
                data.Add(list[i]);
            }

            return data.Average();

        }


        public double[] Filter(double[] signal, double procent)
        {
            double[] filteredSignal;

            /*filteredSignal = AverageValueFilter(signal, 3);
            filteredSignal = MedianFilter(filteredSignal, 3);*/

            filteredSignal = DoSqareSignal(signal, procent);

            //filteredSignal = AverageValueFilter(filteredSignal, 3);
            filteredSignal = MyMedianFilter(filteredSignal, 3);

            //double[] newfilteredSignal = LastFilter(filteredSignal);

            return filteredSignal;
        }

        public double[] FilterOnMin(double[] signal, double procent)
        {
            double[] filteredSignal;

            filteredSignal = AverageValueFilter(signal, 3);
            filteredSignal = MedianFilter(filteredSignal, 3);



            //1500

            filteredSignal = DoSqareSignalOnMin(signal, procent);

            //разделить на интервалы


            filteredSignal = AverageValueFilter(filteredSignal, 3);
            filteredSignal = MedianFilter(filteredSignal, 3);

            double[] newfilteredSignal = LastFilter(filteredSignal);

            return newfilteredSignal;
        }

        private double[] FindPeaks(double[] signal)
        {
            List<double> peaks = new List<double>();
            bool isFindsPeak = false;
            double prevValue = signal[0];
            double max = int.MinValue;
            for (int i = 1; i < signal.Length; i++)
            {
                if (signal[i] > max)
                {
                    max = signal[i];
                    isFindsPeak = true;
                }
                else if (signal[i] < max && isFindsPeak)
                {
                    peaks.Add(max);
                    max = int.MinValue;
                    isFindsPeak = false;
                }
            }

            return peaks.ToArray();
        }

        private double[] DoSqareSignal(double[] signal, double procent)
        {
            double[] newSignal = new double[signal.Length];

            double maxValue = signal.Max();
            double minValue = signal.Min();
            double amplitude = (maxValue - minValue)/2;
            double eps = amplitude * procent;

            for (int i = 0; i < signal.Length; i++)
            {
                if ((maxValue - signal[i]) < eps)  // if (((maxValue - signal[i]) / maxValue) < procent)
                {
                    newSignal[i] = maxValue;
                }
                else
                {
                    newSignal[i] = minValue;
                }
            }

            return newSignal;
        }


        private double[] DoSqareSignalOnMin(double[] signal, double procent)
        {
            double[] newSignal = new double[signal.Length];

            double maxValue = signal.Max();
            double minValue = signal.Min();
            double amplitude = maxValue - minValue;
            double eps = amplitude * procent;

            for (int i = 0; i < signal.Length; i++)
            {
                if (maxValue - signal[i] > eps)
                {
                    newSignal[i] = minValue;
                }
                else
                {
                    newSignal[i] = maxValue;
                }
            }

            return newSignal;
        }

        private double[] LastFilter(double[] signal)
        {
            double[] newSignal = new double[signal.Length];

            double maxValue = signal.Max();
            double minValue = signal.Min();

            for (int i = 0; i < signal.Length; i++)
            {
                if (signal[i] != minValue)
                {
                    newSignal[i] = maxValue;
                }
                else
                {
                    newSignal[i] = signal[i];
                }
            }

            return newSignal;

        }

        public double[] IntervalFilter(double[] signal, int intervalLength, double procent)
        {
            double[] resultSignal = new double[signal.Length];
            const int windowSize = 3;

            List<double> s = new List<double>(signal);

            int countOfIntervals = (signal.Length / intervalLength) + 1;
            int startIndex = 0;
            int endIndex = intervalLength;

            double[] interval = new double[intervalLength];
            double[] filteredInterval;

            //signal = MyMedianFilter(signal, windowSize);

            for (int i = 0; i < countOfIntervals; i++)
            {
                CopyArray(signal, interval, startIndex, endIndex);

                filteredInterval = DoSqareSignalOnMin(interval, procent);
                filteredInterval = MyMedianFilter(filteredInterval, windowSize);

                for (int j = 0, k = startIndex; j < filteredInterval.Length && k < endIndex; j++, k++)
                {
                    resultSignal[k] = filteredInterval[j];
                }

                startIndex = endIndex;
                endIndex += intervalLength;

                if (endIndex >= signal.Length)
                {
                    endIndex = signal.Length;
                }
            }

            resultSignal = DoSqareSignal(resultSignal, procent*2);
            resultSignal = MyMedianFilter(resultSignal, 5);

            return resultSignal;
        }


        private void CopyArray(double[] arrayFrom, double[] arrayTo, int startIndex, int endIndex) 
        {
            for (int i = 0, j = startIndex; i < arrayTo.Length && j < endIndex; i++, j++)
            {
                arrayTo[i] = arrayFrom[j];
            }
        }

        private void AlignValues(double[] signal, double procent)
        {
            double minValue = signal.Min();
            int maxLength = MaxSpaceLength(signal);
            //int lengthForEquals = (int)(maxLength - maxLength * procent);
            int lengthForEquals = (int)(maxLength * procent);
            int serriesCount = 0;

            bool IsSignalBegin = true;

            for (int i = 0; i < signal.Length; i++)
            {
                if (signal[i] == minValue)
                {
                    serriesCount++;
                }
                else if (serriesCount != 0)
                {
                    if (serriesCount < lengthForEquals && !IsSignalBegin)
                    {
                        SetMaxTo(signal, i - serriesCount, i);
                    }
                    else
                    {
                        IsSignalBegin = false;
                    }
                    serriesCount = 0;
                }
            }
        }

        private int MaxSpaceLength(double[] signal)
        {
            int maxLength = int.MinValue;
            double minValue = signal.Min();
            int serriesCount = 0;

            for (int i = 0; i < signal.Length; i++)
            {
                if (signal[i] == minValue)
                {
                    serriesCount++;
                }
                else if (signal[i] != minValue)
                {
                    if (serriesCount > maxLength)
                    {
                        maxLength = serriesCount;
                    }
                    serriesCount = 0;
                }
            }

            return maxLength;
        }

        private void SetMaxTo(double[] array, int startIndex, int endIndex)
        {
            double max = array.Max();
            for (int i = startIndex; i < endIndex; i++)
            {
                array[i] = max;
            }
        }

        private int FindMaxLength(double[] signal)
        {
            bool isSeriesGo = false;

            double minValue = signal.Min();

            List<int> lengthList = new List<int>();
            int serriesLength = 0;


            for (int i = 1; i < signal.Length; i++)
            {
                if (isSeriesGo && signal[i] == minValue)
                {
                    serriesLength++;
                }
                else if (!isSeriesGo && signal[i] == minValue)
                {
                    isSeriesGo = true;
                    serriesLength++;
                }
                else if (isSeriesGo && signal[i] != minValue)
                {
                    lengthList.Add(serriesLength);
                    isSeriesGo = false;
                    serriesLength = 0;
                }
                else
                {
                    /*serriesLength = 0;
                    isSeriesGo = false;*/
                }
            }


            return lengthList.Max();
            
        }

        public double[] FillSpaces(double[] signal, int intervalLength)
        {
            const double procentOfAmplitude = 0.8;
            const int windowSize = 3;
            const double procentOfLength = 0.3;

            double[] resultSignal = new double[signal.Length];

            List<double> s = new List<double>(signal);

            int countOfIntervals = (int)(signal.Length / intervalLength) + 1; ////////////
            int startIndex = 0;
            int endIndex = intervalLength;

            double[] interval = new double[intervalLength];
           

            for (int i = 0; i < countOfIntervals; i++)
            {
                CopyArray(signal, interval, startIndex, endIndex);

                AlignValues(interval, procentOfLength);

                for (int j = 0, k = startIndex; j < interval.Length && k < endIndex; j++, k++)
                {
                    resultSignal[k] = interval[j];
                }

                startIndex = endIndex;
                endIndex += intervalLength;

                if (endIndex >= signal.Length)
                {
                    endIndex = signal.Length;
                }
            }

            resultSignal = DoSqareSignal(resultSignal, procentOfAmplitude);
            resultSignal = MyMedianFilter(resultSignal, windowSize);

            return resultSignal;
        }




        private double[] DropForOne(double[] interval)
        {
            double[] resultInterval = new double[interval.Length];

            List<int> indexes = new List<int>();
            List<int> lengthList = new List<int>();

            int maxLength = FindMaxLength(interval);
            int eps = (int)(maxLength * 0.05);

            double maxValue = interval.Max();

            Dictionary<int, int> indexLength = new Dictionary<int, int>();

            bool isSeriesGo = false;
            double minValue = interval.Min();

            int serriesLength = 0;


            for (int i = 0; i < interval.Length; i++)
            {
                if (isSeriesGo && interval[i] == minValue)
                {
                    serriesLength++;
                }
                else if (!isSeriesGo && interval[i] == minValue)
                {
                    isSeriesGo = true;
                    indexes.Add(i);
                    serriesLength++;
                }
                else if (isSeriesGo && interval[i] != minValue)
                {
                    lengthList.Add(serriesLength);
                    isSeriesGo = false;
                    serriesLength = 0;
                }
                else
                {
                    /*serriesLength = 0;
                    isSeriesGo = false;*/
                }

            }

            for (int i = 0; i < indexes.Count; i++)
            {
                if (lengthList[i] <= (maxLength + eps) && lengthList[i] >= (maxLength - eps))
                {
                    indexLength.Add(indexes[i], lengthList[i]);
                }
                
            }

            for (int i = 0; i < interval.Length; i++)
            {
                if (!IsInInterval(indexLength, i))
                {
                    resultInterval[i] = interval[i];
                }
                else
                {
                    resultInterval[i] = maxValue;
                }
            }

            return resultInterval;
        }


        public void TrimSignal(double[] signal, double trimValue)
        { 
            for(int i = 0; i < signal.Length; i++)
            {
                if (signal[i] > trimValue)
                    signal[i] = trimValue;
            }
        }


        private bool IsInInterval(Dictionary<int, int> interval, int value)
        {
            foreach (var item in interval)
            {
                if (value >= item.Key && value < item.Value)
                {
                    return true;
                }
            }

            return false;
        }


        private double[] PhasePeriod(double[] signal)
        {
            List<double> periods = new List<double>();

            int periodCount = 0;
            bool isPeriodOnGo = false;

            double previousValue = signal[0];
            double max = signal.Max();
            double min = signal.Min();

            for (int i = 0; i < signal.Length; i++)
            {
                if (previousValue == max && signal[i] == min)
                {
                    periods.Add(periodCount);
                    periodCount = 0;
                    previousValue = signal[i];
                }
                else
                {
                    periodCount++;
                    previousValue = signal[i];
                }
            }

            return periods.ToArray();
        }



        public int[] PeaksCoordinates(double[] signal)
        { 
            List<int> peaksCoordinates = new List<int>();

            double previousValue = signal[0];
            double max = signal.Max();
            double min = signal.Min();

            for (int i = 0; i < signal.Length; i++)
            {
                if (previousValue == min && signal[i] == max)
                {
                    peaksCoordinates.Add(i);
                }
                previousValue = signal[i];
            }

            return peaksCoordinates.ToArray();
        }


        public int[] FindPeaksCoordinates(double[] signal) // основной метод, который нужно будет вынести
        {
            const int intervalLength = 1000;
            const double procentOfAmplitude = 0.40;
            const int intervalLengthForFillSpaces = 1000;

            double[] filteredSignal = IntervalFilter(signal, intervalLength, procentOfAmplitude);
            filteredSignal = FillSpaces(filteredSignal, intervalLengthForFillSpaces);
            int[] peaksCoordinates = PeaksCoordinates(filteredSignal);

            return peaksCoordinates;
        }
    }
}
