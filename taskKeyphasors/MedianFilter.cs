using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskKeyphasors
{
    class MedianFilter
    {
        static public IList<double> Execute(IList<double> signal, int countOfNeighbors)
        { 
            List<double> filteredSignal = new List<double>();
            List<double> neighbors = new List<double>(countOfNeighbors);
            int oneSideNeighbors = (int)(countOfNeighbors / 2);

            double median = 0;


            for (int i = 0; i < signal.Count; i++)
            {
                if(i < oneSideNeighbors)// i в начале  
                {
                    median = FindMedianOnInterval(signal, 0, oneSideNeighbors + (oneSideNeighbors-i));// тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else if(i > (signal.Count - oneSideNeighbors)) // i в конце
                {
                    // тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else // i в середине
                {
                    median = FindMedianOnInterval(signal, i - oneSideNeighbors, i + oneSideNeighbors);
                }

                filteredSignal.Add(median);
            }
            return filteredSignal;
            
        }

        public double[] Execute(double[] signal, int countOfNeighbors)
        {
            List<double> filteredSignal = new List<double>();
            List<double> neighbors = new List<double>(countOfNeighbors);
            int oneSideNeighbors = (int)(countOfNeighbors / 2);

            double median = 0;


            for (int i = 0; i < signal.Length; i++)
            {
                if (i < oneSideNeighbors)// i в начале  
                {
                    median = FindMedianOnInterval(signal, 0, oneSideNeighbors + (oneSideNeighbors - i));// тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else if (i >= (signal.Length - oneSideNeighbors)) // i в конце
                {
                    // тут тоже самое, но нужно со startIndex и endIndex подумать
                }
                else // i в середине
                {
                    median = FindMedianOnInterval(signal, i - oneSideNeighbors, i + oneSideNeighbors);
                }

                filteredSignal.Add(median);
            }
            return filteredSignal.ToArray();

        }

        private static double FindMedianOnInterval(IList<double> list, int startIndex, int endIndex)
        {
            List<double> data = new List<double>();

            for (int i = startIndex; i <= endIndex; i++)
            { 
                data.Add(list[i]);
            }

            return FindMedian(data);

        }

        private static double FindMedian(List<double> list)
        {
            list.Sort();
            int medianIndex = list.Count / 2;

            return list[medianIndex];
        }
    }
}
