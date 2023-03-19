using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Label _pathLabel;
        private Label _postfixLabel;
        private Label _amountLabel;
        private TextBox _postfixField;
        private CheckBox _newFileCheckBox;

        private int _startIndex;
        private int _endIndex;
        private string _postfix;
        private string _filePath;

        public MainWindow()
        {
            InitializeComponent();

            _pathLabel = FindName("FilePathLabel") as Label;
            _postfixLabel = FindName("PostfixLabel") as Label;
            _amountLabel = FindName("IndexesAmountLabel") as Label;
            _postfixField = FindName("PostfixField") as TextBox;
            _newFileCheckBox = FindName("NewFileCheckBox") as CheckBox;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Выберите файл, в котором нужно проставить индексы.";

            if (dialog.ShowDialog() == true)
            {
                _filePath = dialog.FileName;
                _pathLabel.Content = dialog.FileName;
            }
        }

        private void StartIndexField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _startIndex = Convert.ToInt32(((TextBox)sender).Text);

            if (_amountLabel != null)
                _amountLabel.Content = $"Кол-во: {_endIndex - _startIndex}";
        }

        private void EndIndexField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _endIndex = Convert.ToInt32(((TextBox)sender).Text);

            if (_amountLabel != null)
                _amountLabel.Content = $"Кол-во: {_endIndex - _startIndex}";
        }

        private void NewFileCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _postfixLabel.Visibility = Visibility.Visible;
            _postfixField.Visibility = Visibility.Visible;
        }

        private void NewFileCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _postfixLabel.Visibility = Visibility.Hidden;
            _postfixField.Visibility = Visibility.Hidden;
        }

        private void AddIndexButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
                MessageBox.Show("Сначала выберите путь!");

            bool saveToOtherFile = (bool)_newFileCheckBox.IsChecked;
            FileInfo file= new FileInfo(_filePath);

            string tempFilePath = saveToOtherFile
                ? Path.Combine(file.DirectoryName, file.Name.Replace($"{file.Extension}", "") + _postfixField.Text + file.Extension)
                : _filePath + "_temp";

            using (StreamReader reader = new StreamReader(_filePath))
            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                string line;
                int lineIndex = 0;
                int markIndex = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (lineIndex >= _startIndex - 1 && lineIndex < _endIndex)
                    {
                        line = $"{markIndex}_{line}";
                        markIndex++;
                    }

                    writer.WriteLine(line);

                    lineIndex++;
                }
            }

            if (!saveToOtherFile)
            {
                File.Delete(_filePath);
                File.Move(tempFilePath, _filePath);
            }

            MessageBox.Show($"Индексы добавлены!");
        }
    }
}
