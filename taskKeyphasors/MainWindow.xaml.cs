using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathNet.Numerics.IntegralTransforms;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace taskKeyphasors
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XyDataSeries<double, double> currentDataSerries = new XyDataSeries<double, double>();
        private XyDataSeries<double, double> currentDataSerriesFiltered = new XyDataSeries<double, double>();

        private DataReader dataReader = new DataReader();
        private const string pathToFirstData = @"C:\Users\kjgug\OneDrive\Рабочий стол\ТИК\графики\выборки фазоотметик\KE201М010 Виброперемещение (выборка) Без преобразования 25-03-06_12-15-02-161.csv";
        private const string pathToSecondData = @"C:\Users\kjgug\OneDrive\Рабочий стол\ТИК\графики\выборки фазоотметик\KE201М011 Виброперемещение (выборка) Без преобразования 25-03-06_12-15-29-791.csv";
        private const string pathToThirdData = @"C:\Users\kjgug\OneDrive\Рабочий стол\ТИК\графики\выборки фазоотметик\KE201М012 Виброперемещение (выборка) Без преобразования 25-03-06_12-15-43-666.csv";

        private const string pathToWriteSecondData = @"C:\Users\kjgug\OneDrive\Рабочий стол\ТИК\графики\скрыны\periods3.csv";

        private string currentPath;

        MedianFilter medianFilter = new MedianFilter();
        const int countOfNeighbors = 3;

        FilterManager filterManager = new FilterManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            comboBoxChoseSignal.Items.Add("Сигнал 1");
            comboBoxChoseSignal.Items.Add("Сигнал 2");
            comboBoxChoseSignal.Items.Add("Сигнал 3");

            FirstGraphSeries.DataSeries = currentDataSerries;
            FirstGraphFilteredSeries.DataSeries = currentDataSerriesFiltered;

        }

        private XyDataSeries<double, double> TakeValuesFromSource(string pathToData)
        {
            XyDataSeries<double, double> data = new XyDataSeries<double, double>();

            try
            {
                data = dataReader.ReadXYFromCsv(pathToData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return data;
        }

        private void UpdateDataSeries(XyDataSeries<double, double> series, IList<double> newData)
        {
            using (series.SuspendUpdates())
            {
                series.Clear();
                for (int i = 0; i < newData.Count; i++)
                {
                    series.Append(i, newData[i]);
                }
            }

        }

        private void OnFilterButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IList<double> tmp;
                double[] data = currentDataSerries.YValues.ToArray();

                data = filterManager.IntervalsFilter(data, 1000);
                data = filterManager.IntervalDrop(data, 1000);
                //data = filterManager.IntervalDrop(data, 750);

                UpdateDataSeries(currentDataSerriesFiltered, data);


                ShowPeriod(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OncomboBoxChoseSignalSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxChoseSignal.SelectedIndex == 0)
            {
                currentPath = pathToFirstData;
            }
            else if (comboBoxChoseSignal.SelectedIndex == 1)
            {
                currentPath = pathToSecondData;
            }
            else if (comboBoxChoseSignal.SelectedIndex == 2)
            {
                currentPath = pathToThirdData;
            }

            XyDataSeries<double, double> tmp1 = TakeValuesFromSource(currentPath);
            UpdateDataSeries(currentDataSerries, tmp1.YValues);
            currentDataSerriesFiltered.Clear();
            
        }

        private void ShowPeriod(double[] signal)
        {
            List<int> periods = new List<int>();

            int periodCount = 0;
            bool isPeriodOnGo = false;

            double previousValue = signal[0];
            double max = signal.Max();
            double min = signal.Min();

            for (int i = 0; i < signal.Length; i++)
            {
                if (previousValue == min && signal[i] == max)
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

            WriteToCsv(pathToWriteSecondData, periods);
        }

        public void WriteToCsv(string filePath, List<int> data)  // убрать в другое место
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }
        }


    }
}
