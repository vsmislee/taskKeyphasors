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

namespace taskKeyphasors
{
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

            filteredSignal = AverageValueFilter(signal, 3);
            filteredSignal = MedianFilter(filteredSignal, 3);

            

            //1500

            filteredSignal = DoSqareSignal(signal, procent);

            //разделить на интервалы

           
            filteredSignal = AverageValueFilter(filteredSignal, 3);
            filteredSignal = MedianFilter(filteredSignal, 3);

            double[] newfilteredSignal = LastFilter(filteredSignal);

            return newfilteredSignal;
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

            for (int i = 0; i < signal.Length; i++)
            {
                if (((maxValue - signal[i]) / maxValue) < procent)
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

            if (minValue > 0)
            {
                for (int i = 0; i < signal.Length; i++)
                {
                    if (signal[i] > (minValue + minValue * procent))
                    {
                        newSignal[i] = maxValue;
                    }
                    else
                    {
                        newSignal[i] = minValue;
                    }
                }
            }
            else if (minValue < 0)
            {
                for (int i = 0; i < signal.Length; i++)
                {
                    if (signal[i] > (minValue - minValue * procent))
                    {
                        newSignal[i] = maxValue;
                    }
                    else
                    {
                        newSignal[i] = minValue;
                    }
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

        public double[] IntervalsFilter(double[] signal, int intervalLength)
        {
            double[] resultSignal = new double[signal.Length];

            List<double> s = new List<double>(signal);

            int countOfIntervals = (int)(signal.Length / intervalLength) + 1; ////////////
            int startIndex = 0;
            int endIndex = intervalLength;

            double[] interval = new double[intervalLength];
            double[] filteredInterval;

            for (int i = 0; i < countOfIntervals; i++)
            {
                //s.CopyTo(interval, startIndex);  /// совб функцию

                CopyArray(signal, interval, startIndex, endIndex);

                filteredInterval = Filter(interval, 0.55);

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


            resultSignal = Filter(resultSignal, 0.90);

            return resultSignal;
        }


        private void CopyArray(double[] arrayFrom, double[] arrayTo, int startIndex, int endIndex) 
        {
            for (int i = 0, j = startIndex; i < arrayTo.Length && j < endIndex; i++, j++)
            {
                arrayTo[i] = arrayFrom[j];
            }
        }

        private void AlignValues(double[] signal) 
        {
            double minValue = signal.Min();
            int maxLength = MaxSpaceLength(signal);
            int lengthForEquals = (int)(maxLength - maxLength * 0.1);
            int serriesCount = 0;

            for(int i = 0; i < signal.Length; i++)
            {
                if (signal[i] == minValue)
                {
                    serriesCount++;
                }
                else if(serriesCount != 0)
                {
                    if (serriesCount < lengthForEquals)
                    {
                        SetMaxTo(signal, i - serriesCount, i);
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

        public double[] IntervalDrop(double[] signal, int intervalLength)
        {
            double[] resultSignal = new double[signal.Length];

            List<double> s = new List<double>(signal);

            int countOfIntervals = (int)(signal.Length / intervalLength) + 1; ////////////
            int startIndex = 0;
            int endIndex = intervalLength;

            double[] interval = new double[intervalLength];
           

            for (int i = 0; i < countOfIntervals; i++)
            {
                CopyArray(signal, interval, startIndex, endIndex);

                AlignValues(interval);

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

            resultSignal = AverageValueFilter(resultSignal, 5);
            resultSignal = MedianFilter(resultSignal, 5);

            resultSignal = LastFilter(resultSignal);

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
    }
}
