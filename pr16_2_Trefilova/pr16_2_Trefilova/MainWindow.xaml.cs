using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace pr16_2_Trefilova
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Student> allStudents = new ObservableCollection<Student>();
        public MainWindow()
        {
            InitializeComponent();
            StudentsDataGrid.ItemsSource = allStudents;
        }
        private void GradeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "CSV файл (*.csv)|*.csv";
            saveDialog.FileName = "students.csv";

            if (saveDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                {
                    writer.WriteLine("Фамилия,Группа,Предмет,Оценка");
                    foreach (var s in allStudents)
                    {
                        writer.WriteLine($"{s.Surname},{s.Group},{s.Subject},{s.Grade}");
                    }
                }
                MessageBox.Show("Сохранено!");
            }
        }
        private void ImportFromCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.Filter = "CSV файл (*.csv)|*.csv";

            if (openDialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(openDialog.FileName);
                allStudents.Clear();

                for (int i = 1; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length >= 4)
                    {
                        allStudents.Add(new Student
                        {
                            Surname = parts[0],
                            Group = parts[1],
                            Subject = parts[2],
                            Grade = int.Parse(parts[3])
                        });
                    }
                }

                StudentsDataGrid.ItemsSource = null;
                StudentsDataGrid.ItemsSource = allStudents;
                MessageBox.Show($"Загружено {allStudents.Count} записей");
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            string surname = SurnameTextBox1.Text.Trim();
            string group = (GroupComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
            string subject = SubjectTextBox1.Text.Trim();
            string gradeText = GradeTextBox1.Text.Trim();
            if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(gradeText))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            if (!int.TryParse(gradeText, out int grade) || grade < 1 || grade > 5)
            {
                MessageBox.Show("Оценка от 1 до 5");
                return;
            }
            var existing = allStudents.FirstOrDefault(s => s.Surname == surname && s.Group == group && s.Subject == subject);
            if (existing != null)
            {
                existing.Grade = grade;
            }
            else
            {
                allStudents.Add(new Student { Surname = surname, Group = group, Subject = subject, Grade = grade });
            }
            SurnameTextBox1.Text = "";
            SubjectTextBox1.Text = "";
            GradeTextBox1.Text = "";
            StudentsDataGrid.ItemsSource = null;
            StudentsDataGrid.ItemsSource = allStudents;
        }
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filterGroup = (FilterComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (filterGroup == "Все")
            {
                StudentsDataGrid.ItemsSource = null;
                StudentsDataGrid.ItemsSource = allStudents;
            }
            else
            {
                var filtered = allStudents.Where(s => s.Group == filterGroup).ToList();
                StudentsDataGrid.ItemsSource = null;
                StudentsDataGrid.ItemsSource = filtered;
            }
        }
        private void SortSurname_Click(object sender, RoutedEventArgs e)
        {
            var sorted = allStudents.OrderBy(s => s.Surname).ToList();
            StudentsDataGrid.ItemsSource = null;
            StudentsDataGrid.ItemsSource = sorted;
        }
        private void SortGroup_Click(object sender, RoutedEventArgs e)
        {
            var sorted = allStudents.OrderBy(s => s.Group).ThenBy(s => s.Surname).ToList();
            StudentsDataGrid.ItemsSource = null;
            StudentsDataGrid.ItemsSource = sorted;
        }
    }
}
