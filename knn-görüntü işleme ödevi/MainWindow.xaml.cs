using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace knn_görüntü_işleme_ödevi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<String> Dataset = new List<string>();
        private String File = "";
        private int k = 3;

        public MainWindow()
        {
            InitializeComponent();
        }


        //örnek veriseti
        public List<String> GetDir()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                return Directory.GetFiles(fbd.SelectedPath).AsEnumerable().ToList();
            }
            return GetDir();
        }

        //hesaplanacak veri
        public String GetFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }
            return GetFile();
        }

        public Bitmap GetBitmap(String filename)
        {
            return new Bitmap(filename);
        }

        public double CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            double sum = 0;
            //x=width
            for (int height = 0; height < bmp1.Height; height++)
            {
                for (int width = 0; width < bmp1.Width; width++)
                {
                    sum += CalculateDistance(bmp1.GetPixel(width, height), bmp2.GetPixel(width, height));
                }
            }
            return sum;
        }

        public double CalculateDistance(System.Drawing.Color p1, System.Drawing.Color p2)
        {
            double distance = 0;

            distance += Math.Abs(p1.B - p2.B);

            return distance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SelectDataset_Click(object sender, RoutedEventArgs e)
        {
            Dataset = GetDir();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            File = GetFile();
        }

        private void ProcessFile_Click(object sender, RoutedEventArgs e)
        {
            k = Convert.ToInt32(textBox_kValue.Text);

            Dictionary<String, double> Filename_Distance = new Dictionary<string, double>();

            foreach (String DatasetFile in Dataset)
            {
                Filename_Distance.Add(
                    DatasetFile,
                    CompareBitmaps( GetBitmap(DatasetFile), GetBitmap(File) ) 
                    );
            }

            Filename_Distance = Filename_Distance.OrderBy(element => element.Value).ToDictionary(pair=>pair.Key, pair=>pair.Value);
            

            Dictionary<String, int> Filename_Repetition = new Dictionary<string, int>();

            for(int i = 0; i < k; i++)
            {
                String key = System.IO.Path.GetFileName(Filename_Distance.ElementAt(i).Key).Split('_')[0];
                if (!Filename_Repetition.ContainsKey(key))
                {
                    Filename_Repetition.Add(key, 0);
                }
                Filename_Repetition[key]++;
            }

            var c2 = Filename_Repetition.OrderBy(element => element.Value);

            label2.Content = ("En çok şu sınıfa benziyor: " +  c2.ElementAt(0).Key);
        }

    }
}
