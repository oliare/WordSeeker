using System.IO;
using System.Windows;

namespace Exam
{
    public partial class ReportWin : Window
    {

        public ReportWin(List<ViewReport> list)
        {
            InitializeComponent();
            filesListView.ItemsSource = list;
        }

        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file = Path.Combine(desktop, "Report.txt");

            if (File.Exists(file))
            {
                string newName = GenerateNewFileName(desktop, "Report.txt");
                file = Path.Combine(desktop, newName);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine("Word\t\tFile Name\t\t\tOccurrences");

                    foreach (var item in filesListView.Items)
                    {
                        var report = (ViewReport)item;
                        writer.WriteLine($"{report.Word,-10}\t{report.FileName,-30}\t{report.Occurrences}");
                    }
                }
                MessageBox.Show($"The report is saved on the desktop: {file}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private string GenerateNewFileName(string dir, string initialFile)
        {
            int counter = 1;
            string newFile = initialFile;

            while (File.Exists(Path.Combine(dir, newFile)))
            {
                newFile = $"{Path.GetFileNameWithoutExtension(initialFile)}_{counter}{Path.GetExtension(initialFile)}";
                counter++;
            }
            return newFile;
        }
    }

    public class ViewReport
    {
        public string Word { get; set; }
        public string FileName { get; set; }
        public int Occurrences { get; set; }
    }

}

