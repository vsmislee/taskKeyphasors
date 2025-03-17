using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SciChart.Charting.Model.DataSeries;
using System.IO;

namespace taskKeyphasors
{
    internal class DataReader
    {
        public XyDataSeries<double, double> ReadXYFromCsv(string path)
        {
            XyDataSeries<double, double> data = new XyDataSeries<double, double>();


            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                HasHeaderRecord = true,
                Delimiter = ";"
            };

            using var streamReader = File.OpenText(path);
            using var csvReader = new CsvReader(streamReader, config);

            double xField;
            double yField;
           
            while (csvReader.Read())
            {
                /*if(csvReader.TryGetField<double>(0, out xField))
                    data.XValues.Add(xField);*/
                
                data.YValues.Add(csvReader.GetField<double>(1)); // читаю только одно поле, так как в файле отсутствуют данные о времени (X)
            }

            return data;

        }
    }
}
