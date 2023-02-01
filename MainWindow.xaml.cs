using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CSV_to_chart_WPF
{

    public class chart_header
    {
        public bool chart_ischecked { get; set; }
        public string chart_Title { get; set; }
        public Brush chart_color { get; set; }
        public string chart_scale_factor { get; set; }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        List<chart_header> header = new List<chart_header>();

        ArrayList line_color = new ArrayList();



        public MainWindow()
        {
            InitializeComponent();
        }


        private void init_chart_plot()
        {

        }


        private void button_filename_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                textbox_filename.Text = openFileDialog.FileName;

                if (File.Exists(textbox_filename.Text))
                {
                    get_file_contents();
                }
            }
        }


        private void get_file_contents()
        {
            using (TextFieldParser csv_parser = new TextFieldParser(textbox_filename.Text))
            {
                int number_of_fields = 0;

                csv_parser.TextFieldType = FieldType.Delimited;
                csv_parser.SetDelimiters(",");

                
                /* Read header */
                if (!csv_parser.EndOfData)
                {
                    Random rnd = new Random();

                    string[] fields = csv_parser.ReadFields();

                    /* Set properties for header selection control */
                    foreach (string field in fields)
                    {
                        Color randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
                        Brush brush = new SolidColorBrush(randomColor);
                        line_color.Add(randomColor);

                        header.Add(new chart_header() { chart_ischecked = true, chart_color = brush, chart_scale_factor = "1", chart_Title = field });
                        number_of_fields++;
                    }
                    listbox_display_header.ItemsSource = header;
                }

                /* Read remaining values from csv file */
                ArrayList al = new ArrayList();
                while (!csv_parser.EndOfData)
                {
                    string[] fields = csv_parser.ReadFields();
                    al.Add(fields);
                }

                /* Create dynamic 2-dimensional array */
                double[][] arr = new double[number_of_fields][];
                double[] counter = new double[al.Count];
                for (int field = 0; field < number_of_fields; field++)
                {
                    arr[field] = new double[al.Count];
                }
                
                /* add entries into chart array */
                for (int count = 0; count < al.Count; count++)
                {
                    string[] fields = (string[])al[count];
                    counter[count] = count;

                    for (int field = 0; field < number_of_fields; field++)
                    {
                        arr[field][count] = Convert.ToDouble(fields[field]);
                    }
                }

                /* add value into chart area and customize the chart properties */
                for (int idx = 0; idx < number_of_fields; idx++)
                {
                    chart_header item = (chart_header)listbox_display_header.ItemContainerGenerator.Items[idx];
                    
                    var signal = scottplot_chart.Plot.AddScatterLines(counter, arr[idx]);
                    var temp_color = (Color)line_color[idx];
                    signal.LineColor = System.Drawing.Color.FromArgb(temp_color.A, temp_color.R, temp_color.G, temp_color.B);
                    signal.Label = item.chart_Title;

                    var new_axes = scottplot_chart.Plot;
                }

                scottplot_chart.Refresh();
            }
        }
    }
}
