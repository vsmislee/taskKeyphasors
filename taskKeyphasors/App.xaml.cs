using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SciChart.Charting.Visuals;

namespace taskKeyphasors
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Set this code once in App.xaml.cs or application startup before any SciChartSurface is shown 
            SciChartSurface.SetRuntimeLicenseKey("Z3rc0OXqvgexK/4vnuoMNv6Nu6CCEVlCfmXH1uddwbBWu74Cguy04OPRzHJLhkYHKyDopdbjDw6Rkp6WHVKRhvtDuNfLG/BB/mmuwktgSab9fZoyEicFKVQfUapzUhk0/ogy0lwZDrjYpbRBVBJZcUjOD6zWE40BeSd1n9cV3DPZ4liwj3AAur9UBUoq1Iig0aXNulyHCrsP+p9Xhgd3pjhksumUP9NNwv0RN2wABxJyn3f2TiDxwXKLK1p7reikNKpB8+/ked1ZgnSn/DFZ44K5kRvz2+t7lt7nOZwTH4mNc3wgDG/to00kX1XYvY9jGdZX59J6doaQ8yrmZkiRZGxT+Cd1niLsAoe46bTGufBKcJl+sGUQurR2etnj91H+fOP7DJRJpW1dfDdCrU0pGyhXGDrYWuM4Ih/nRA/pl+VnO3AEmPkjxps2XSeAkVK0/UpQq7XN5OJu6biYYZ9WOPogo37Z/RTMZol4iXNnIeK2hx/zC82OqyoOZASsJ4Fh+3vwdkLIGfB3NN1WSnYUedr2cXEHwIJ9zu9svREtOaFQSOjs5N7VnlccrpMrOAxbccU=");
        }
    }
}
