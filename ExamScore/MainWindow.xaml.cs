using CsvHelper;
using CsvHelper.Configuration;
using ExamScore.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamScore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ExamScoreContext _context;
        private List<Score> scores;
        Dictionary<string, int> yearIds;
        Dictionary<string, int> subjectIds;
        public MainWindow()
        {
            InitializeComponent();

            _context = new ExamScoreContext();
            scores = new List<Score>();
            subjectIds = new Dictionary<string, int>();
            subjectIds = new Dictionary<string, int>();
            subjectIds.Add("Toan", 1);
            subjectIds.Add("Van", 2);
            subjectIds.Add("Ly", 3);
            subjectIds.Add("Sinh", 4);
            subjectIds.Add("NgoaiNgu", 5);
            subjectIds.Add("Hoa", 6);
            subjectIds.Add("LichSu", 7);
            subjectIds.Add("DiaLy", 8);
            subjectIds.Add("GDCD", 9);

            yearIds = new Dictionary<string, int>();
            yearIds.Add("2017", 1);
            yearIds.Add("2018", 2);
            yearIds.Add("2019", 3);
            yearIds.Add("2020", 4);
            yearIds.Add("2021", 5);

            ScoreDataGrid.ItemsSource = scores;
            for (int year = 2017; year <= 2021; year++)
            {
                YearComboBox.Items.Add(year.ToString());
            }
        }

      

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedYear = YearComboBox.SelectedValue?.ToString();

            if (!string.IsNullOrEmpty(selectedYear))
            {
                // Retrieve scores for the selected year from the database
                scores.Clear(); // Clear the existing ObservableCollection
                var scoresForYear = _context.Scores
                    .Where(s => s.Student.SchoolYear.ExamYear == int.Parse(selectedYear))
                    .ToList();

                // Add scores to the ObservableCollection
                scores.AddRange(scoresForYear);
            }
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;
            string selectedYear = YearComboBox.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(selectedYear))
            {
                MessageBox.Show("Please select a file and a year before importing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Implement your logic to import data from the file and update the database
                ImportDataFromFile(selectedYear);

                MessageBox.Show("Import successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during import: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportDataFromFile(string selectedYear)
        {
            string filePath = $"C:\\Users\\KHANG PHAM\\Downloads\\archive (1)\\{selectedYear}.csv";
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            using (var dbContext = new ExamScoreContext())
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    ProcessCsvRecords(filePath, dbContext);

                    transaction.Commit();
                    Console.WriteLine("Import completed successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error during import: {ex.Message}");
                    // Log the exception using a logging framework
                }
            }
        }

        private void ProcessCsvRecords(string filePath, ExamScoreContext dbContext)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = csv.GetRecords<RecordModelCSV>().ToList();
                int i = 0;
                var dataTable = new DataTable();
                dataTable.Columns.Add("StudentCode", typeof(string));
                dataTable.Columns.Add("SchoolYearId", typeof(int));
                dataTable.Columns.Add("Status", typeof(int));


                foreach (var record in records) // Process only the first 10 records for testing
                {
                    var studentCode = record.SBD;
                    var year = record.Year;
                    int schoolyearId = yearIds[year];

                    var existedStudent = dbContext.Students
                        .FirstOrDefault(s => s.StudentCode.Equals(studentCode) && schoolyearId == s.SchoolYearId);

                    if (existedStudent == null)
                    {
                        // Add a row to the DataTable
                        dataTable.Rows.Add(studentCode, schoolyearId, 1);
                    }
                }


                using (var sqlBulkCopy = new SqlBulkCopy("server=.;database=ExamScore;uid=sa;pwd=12345;TrustServerCertificate=True;MultipleActiveResultSets=True"))
                {
                    sqlBulkCopy.DestinationTableName = "Students"; // Set your table name

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    sqlBulkCopy.WriteToServer(dataTable);
                }
                
                ProcessRecordsAndBulkInsertScores(records);
            }
        }

        private void ProcessRecordsAndBulkInsertScores(List<RecordModelCSV> records)
        {
            const int batchSize = 10000; // Set your batch size

            using (var dbContext = new ExamScoreContext())
            {
                var scoreDraftTable = new DataTable();
                scoreDraftTable.Columns.Add("StudentCode", typeof(string));
                scoreDraftTable.Columns.Add("SubjectId", typeof(int));
                scoreDraftTable.Columns.Add("YearId", typeof(int));
                scoreDraftTable.Columns.Add("score", typeof(double));

                foreach (var record in records)
                {
                    var studentCode = record.SBD;
                    var year = record.Year;
                    int schoolyearId = yearIds[year];

                  
                        ProcessScoreFields(record, scoreDraftTable, studentCode, schoolyearId);

                        // Bulk insert when reaching the batch size
                        if (scoreDraftTable.Rows.Count >= batchSize)
                        {
                            BulkInsertScores(scoreDraftTable);
                        scoreDraftTable.Clear();
                        }
                }

                // Bulk insert any remaining records
                if (scoreDraftTable.Rows.Count > 0)
                {
                    BulkInsertScores(scoreDraftTable);
                }
            }
        }

        private void BulkInsertScores(DataTable scoreTable)
        {
            using (var sqlBulkCopyScore = new SqlBulkCopy("server=.;database=ExamScore;uid=sa;pwd=12345;TrustServerCertificate=True;MultipleActiveResultSets=True"))
            {
                sqlBulkCopyScore.DestinationTableName = "ScoreDrafts"; // Set your table name

                foreach (DataColumn column in scoreTable.Columns)
                {
                    sqlBulkCopyScore.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                sqlBulkCopyScore.WriteToServer(scoreTable);
            }
        }

        private void ProcessScoreFields(RecordModelCSV record, DataTable scoreDraftTable, string id, int schoolYearId)
        {
            int studentIdColumnIndex = scoreDraftTable.Columns["StudentCode"].Ordinal;
            int subjectIdColumnIndex = scoreDraftTable.Columns["SubjectId"].Ordinal;
            int gradeColumnIndex = scoreDraftTable.Columns["score"].Ordinal;
            int schoolColumnIndex = scoreDraftTable.Columns["YearId"].Ordinal;
            DataRow scoreRow = scoreDraftTable.NewRow();
            foreach (var fieldName in subjectIds.Keys)
            {
               
                        // Assuming corresponding properties in RecordModelCSV for each field
                        var grade = (double)(record.GetType().GetProperty(fieldName)?.GetValue(record) ?? -1.0);

                    if (grade >= 0)
                    {
                        scoreRow = scoreDraftTable.NewRow();
                        scoreRow[studentIdColumnIndex] = id;
                        scoreRow[subjectIdColumnIndex] = subjectIds[fieldName];
                        scoreRow[gradeColumnIndex] = grade;
                    scoreRow[schoolColumnIndex] = schoolYearId;
                    scoreDraftTable.Rows.Add(scoreRow);
                    }
            }
        }

        private bool IsExcludedFieldName(string fieldName)
        {
            return fieldName.Equals("SBD") || fieldName.Equals("Year") || fieldName.Equals("MaTinh");
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedYear = YearComboBox.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(selectedYear))
            {
                MessageBox.Show("Please select a year before clearing the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Implement your logic to clear database scores for the selected year
                ClearDatabaseScores(selectedYear);

                MessageBox.Show("Database cleared successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during database clearing: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClearDatabaseScores(string selectedYear)
        {
            if(scores != null && scores.Count > 0)
            {
                using (var dbContext = new ExamScoreContext())
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        int yearId = 0;
                        if(yearIds.ContainsKey(selectedYear))
                        {
                            yearId = yearIds[selectedYear];
                        } else
                        {
                            var schoolyear = await dbContext.SchoolYears.FirstOrDefaultAsync(s => s.ExamYear == int.Parse(selectedYear));
                            if(schoolyear != null)
                            {
                                yearId = schoolyear.Id;
                                yearIds.Add(selectedYear, yearId );
                            }
                        }

                        if(yearId > 0)
                        {
                            var scoresToRemove = dbContext.Scores.Where(s => s.Student.SchoolYearId == yearId).ToList();
                            if (scoresToRemove.Any())
                            {
                                scores.RemoveAll(s => scoresToRemove.Contains(s));
                            }
                        }

                        transaction.Commit();
                        Console.WriteLine("Import completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error during import: {ex.Message}");
                        // Log the exception using a logging framework
                    }
                }
            }
        }
    }



}