using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace MicroBenchX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();           
        }

        // Lightweight workloads
        // https://github.com/Kwull/NSpeedTest
        // http://www.808.dk/?code-csharp-driveperformance

        [DllImport("kernel32.dll")]
        static extern int GetCurrentProcessorNumber();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(async () =>
              {
                  // 1GB: 1048576 
                  // 512MB: 524288
                  // 256MB: 262144
                  // 128MB: 131072
                  const int listSize = 524288 * 1024 / sizeof(double);
                  var dataArray = new double[listSize];

                  var gaussianRandom = new GaussianRandom();

                  for (int i = 0; i < listSize; i++)
                  {
                      dataArray[i] = gaussianRandom.GetNext();
                  }

                  bool doUpdateStatus = false;

                  // Create a timer with a two second interval.
                  var aTimer = new System.Timers.Timer(2000);
                  // Hook up the Elapsed event for the timer. 
                  aTimer.Elapsed += (o, e) => doUpdateStatus = true;
                  aTimer.AutoReset = true;
                  aTimer.Enabled = true;

                  do
                  {
                      DoRandomMemoryAccess(dataArray);
                      await Task.Delay(1);
                      
                      if (doUpdateStatus)
                      {
                          Debug.WriteLine($"Task is running on core #{GetCurrentProcessorNumber()}");
                          doUpdateStatus = false;
                      }

                  } while (true);
              });

        }

        private void DoRandomMemoryAccess(double[] dataArray)
        {
            int listSize = dataArray.Length;
            var randomIndex = new Random();
            int iter = listSize;
            while (iter > 1)
            {
                iter--;
                int j = randomIndex.Next(0, listSize);
                double tmp = dataArray[iter];
                dataArray[iter] = dataArray[j];
                dataArray[j] = tmp;
            }

            double maxVal = double.MinValue;
            for (int i = 0; i < listSize; i++)
            {
                int index = randomIndex.Next(0, listSize);
                double curVal = dataArray[index];

                if (curVal > maxVal)
                    maxVal = curVal;
            }

            Debug.WriteLine($"Max value is {Math.Round(maxVal, 4)}");
        }
    }
}
