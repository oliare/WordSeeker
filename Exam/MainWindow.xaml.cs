using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Exam
{

    public partial class MainWindow : Window
    {
        public string Word { get; set; }
        public string Source { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_Button(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Source;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Source = dialog.FileName;
                sourceTextBox.Text = Source;
            }
        }

        private async void Search_Button(object sender, RoutedEventArgs e)
        {
            Word = wordTextBox.Text;
            Source = sourceTextBox.Text;

            if (string.IsNullOrEmpty(Word))
            {
                MessageBox.Show("Enter a word to search for");
                return;
            }
            if (string.IsNullOrEmpty(Source))
            {
                MessageBox.Show("Specify the correct path");
                return;
            }
            if (!File.Exists(Source) && !Directory.Exists(Source))
            {
                MessageBox.Show("The specified file or directory does not exist");
                return;
            }

            await SearchWordAsync(Word, Source);
        }
        private async Task SearchWordAsync(string word, string source)
        {
            await Task.Run(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);

                    int allFiles = files.Length;
                    int processed = 0;

                    foreach (string file in files)
                    {
                        int occurrences = TheNumberOfWordOccurrences(file, word);

                        processed++;

                        int progress = (int)((double)processed / allFiles * 100);
                        RefreshProgress(progress);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var res = new List<ViewReport>();
                        foreach (string file in files)
                        {
                            int occurrences = TheNumberOfWordOccurrences(file, word);
                            res.Add(new ViewReport { Word = word, FileName = Path.GetFileName(file), Occurrences = occurrences });
                        }
                        ReportWin reportWin = new ReportWin(res);
                        reportWin.Show();
                        reportWin.Closed += (sender, e) => { percent.Value = 0; progressTextBox.Text = $"0%"; };
                    });

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
        private void RefreshProgress(int progress)
        {
            Dispatcher.Invoke(() =>
            {
                percent.Value = progress;
                progressTextBox.Text = $"{progress:0}%";
            });
        }

        private int TheNumberOfWordOccurrences(string file, string word)
        {
            int count = 0;
            using (StreamReader reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    Regex regex = new Regex(@"\b" + Regex.Escape(word) + @"\b", RegexOptions.IgnoreCase);
                    MatchCollection matches = regex.Matches(line);
                    count += matches.Count;

                }
            }
            return count;

        }
    }

}
