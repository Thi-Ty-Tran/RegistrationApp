//    Name: Thi Ty Tran
//    Date Created: Nov 28, 2024
//    Description: DC Registration App - Assignment 5
//    Last modified: Dec 5, 2024

using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace RegistrationApp
{
    public partial class MainWindow : Window
    {
        // ObservableCollection to hold the list of students
        private ObservableCollection<Student> students = new ObservableCollection<Student>();

        // Constructor for MainWindow, initializes components and event handlers
        public MainWindow()
        {
            InitializeComponent();
            StudentData.SelectionChanged += StudentData_SelectionChanged; // Handle selection change
        }

        // Event handler for the Check button click
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate user inputs before proceeding
                if (!ValidateInputs())
                {
                    return; // Exit if validation fails
                }

                // Define the path for the student records JSON file
                string filePath = Path.Combine(Environment.CurrentDirectory, "StudentRecords.json");

                if (File.Exists(filePath))
                {
                    // Read student records from JSON file
                    string json = File.ReadAllText(filePath);
                    students = JsonConvert.DeserializeObject<ObservableCollection<Student>>(json);
                    LoadSinsToComboBox(); // Load SINs into ComboBox

                    string sin = txtSIN.Text.Trim();
                    // Find existing student by SIN
                    Student existingStudent = students.FirstOrDefault(s => s.sin == sin);

                    if (existingStudent != null)
                    {
                        // Populate fields with the found student's information
                        PopulateFields(existingStudent);
                        MessageBox.Show("Student found! Information populated.");
                    }
                    else
                    {
                        MessageBox.Show("No student record found with this SIN. You can register!");
                    }
                }
                else
                {
                    MessageBox.Show("No student records found! You can register!");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Event handler for the Register button click
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate user inputs before proceeding
                if (!ValidateInputs())
                {
                    return; // Exit if validation fails
                }

                string newSin = txtSIN.Text.Trim();
                // Check for unique SIN
                if (students.Any(s => s.sin == newSin))
                {
                    MessageBox.Show("A student with this SIN already exists. Please enter a unique SIN.");
                    return; // Exit if SIN is not unique
                }

                // Create a new student object
                Student newStudent = new Student
                {
                    firstName = txtFirstName.Text.Trim(),
                    lastName = txtLastName.Text.Trim(),
                    sin = newSin,
                    email = txtEmail.Text.Trim(),
                    highSchoolGrade = ((ComboBoxItem)cboHighSchoolGrade.SelectedItem)?.Content.ToString(),
                    admissionTestScore = ((ComboBoxItem)cboAdmissionTestScore.SelectedItem)?.Content.ToString(),
                    campusLocation = cboCampusLocation.Text,
                    program = cboProgram.Text,
                };

                // Add the new student to the collection and ComboBox
                students.Add(newStudent);
                SINComboBox.Items.Add(newStudent.sin); // Add SIN to ComboBox

                SaveStudentsToJson(); // Save updated student list to JSON
                ClearForm();
                txtFirstName.Focus(); // Set focus back to the first name field
                MessageBox.Show("Student registered successfully!");

                // Refresh the DataGrid to display the new student
                RefreshStudentDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Event handler for DataGrid selection change
        private void StudentData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentData.SelectedItem is Student selectedStudent)
            {
                // Populate fields with selected student's information
                PopulateFields(selectedStudent);
            }
        }

        // Populate input fields with student information
        private void PopulateFields(Student student)
        {
            txtFirstName.Text = student.firstName;
            txtLastName.Text = student.lastName;
            txtEmail.Text = student.email;
            txtSIN.Text = student.sin;
            cboHighSchoolGrade.SelectedItem = student.highSchoolGrade;
            cboAdmissionTestScore.SelectedItem = student.admissionTestScore;
            cboCampusLocation.SelectedItem = student.campusLocation;
            cboProgram.SelectedItem = student.program;
        }

        // Validates the user inputs before saving
        private bool ValidateInputs()
        {
            bool isValid = true;

            // Clear previous error highlights
            txtFirstName.ClearValue(TextBox.BorderBrushProperty);
            txtLastName.ClearValue(TextBox.BorderBrushProperty);
            txtEmail.ClearValue(TextBox.BorderBrushProperty);
            txtSIN.ClearValue(TextBox.BorderBrushProperty);

            // Validate each input field and show error messages if necessary
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.BorderBrush = Brushes.Red;
                // Display error when first name is missing
                MessageBox.Show("First Name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.BorderBrush = Brushes.Red;
                // Display error when last name is missing
                MessageBox.Show("Last Name is required.");
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.(com|ca)$"))
            {
                txtEmail.BorderBrush = Brushes.Red;
                // Display error for wrong email format
                MessageBox.Show("Email must contain '@' and end with '.com' or '.ca'!");
                isValid = false;
            }

            // Check if SIN is empty, not a number, or already exists in the students list
            if (string.IsNullOrWhiteSpace(txtSIN.Text) || !int.TryParse(txtSIN.Text, out _) || students.Any(s => s.sin == txtSIN.Text.Trim()))
            {
                txtSIN.BorderBrush = Brushes.Red;
                MessageBox.Show("SIN is already existed.");
                return false;
            }

            // Check if High School Grade is selected
            if (cboHighSchoolGrade.SelectedIndex == -1)
            {
                MessageBox.Show("High School Grade is required.");
                return false;
            }

            // Check if Admission Test Score is selected
            if (cboAdmissionTestScore.SelectedIndex == -1)
            {
                MessageBox.Show("Admission Test Score is required.");
                isValid = false;
            }

            // Validate SIN format to ensure it is exactly 9 digits
            if (!Regex.IsMatch(txtSIN.Text, @"^\d{9}$"))
            {
                txtSIN.Background = Brushes.Red; // Highlight invalid field
                MessageBox.Show("SIN must be a positive 9-digit number!");
                isValid = false;
            }

            return isValid; // Return the validation result
        }

        // Load SINS into ComboBox
        private void LoadSinsToComboBox()
        {
            SINComboBox.Items.Clear(); // Clear existing items
            foreach (var student in students)
            {
                SINComboBox.Items.Add(student.sin); // Add each student's SIN to ComboBox
            }
        }

        // Clear all input fields and reset the form
        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtSIN.Clear();
            cboHighSchoolGrade.SelectedIndex = -1;
            cboAdmissionTestScore.SelectedIndex = -1;
            cboCampusLocation.SelectedIndex = -1;
            cboProgram.SelectedIndex = -1;
        }

        // Method to save students to a JSON file
        private void SaveStudentsToJson()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "StudentRecords.json");
            try
            {
                // Serialize the students collection to JSON format
                string json = JsonConvert.SerializeObject(students, Formatting.Indented);
                File.WriteAllText(filePath, json); // Write JSON to file
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving to file: {ex.Message}");
            }
        }

        // Update record button click event
        private void btnUpdateRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SINComboBox.SelectedItem is string selectedSin)
                {
                    // Find the student to update using the selected SIN
                    Student studentToUpdate = students.FirstOrDefault(s => s.sin == selectedSin);

                    if (studentToUpdate != null)
                    {
                        // Update student details with input values
                        studentToUpdate.firstName = txtFirstName.Text.Trim();
                        studentToUpdate.lastName = txtLastName.Text.Trim();
                        studentToUpdate.email = txtEmail.Text.Trim();
                        studentToUpdate.sin = txtSIN.Text.Trim();
                        studentToUpdate.highSchoolGrade = ((ComboBoxItem)cboHighSchoolGrade.SelectedItem)?.Content.ToString();
                        studentToUpdate.admissionTestScore = ((ComboBoxItem)cboAdmissionTestScore.SelectedItem)?.Content.ToString();
                        studentToUpdate.campusLocation = cboCampusLocation.Text;
                        studentToUpdate.program = cboProgram.Text;

                        SaveStudentsToJson(); // Save updated students to JSON
                        MessageBox.Show("Student record updated successfully!");

                        // Refresh the DataGrid to reflect changes
                        RefreshStudentDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("No student record found with this SIN.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Event handler for deleting a student record
        private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SINComboBox.SelectedItem is string selectedSin)
                {
                    // Find the student to delete using the selected SIN
                    Student studentToDelete = students.FirstOrDefault(s => s.sin == selectedSin);

                    if (studentToDelete != null)
                    {
                        // Remove the student from the collection and ComboBox
                        students.Remove(studentToDelete);
                        SINComboBox.Items.Remove(selectedSin);

                        SaveStudentsToJson(); // Save updated students to JSON
                        MessageBox.Show("Student record deleted successfully!");

                        // Refresh the DataGrid to reflect changes
                        RefreshStudentDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("No student record found with this SIN.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Clear all records button click event
        private void btnRemoveAllRecords_Click(object sender, RoutedEventArgs e)
        {
            // Confirm deletion of all records
            if (MessageBox.Show("Are you sure you want to remove all records?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                students.Clear(); // Clear the student collection
                SINComboBox.Items.Clear(); // Clear the SIN ComboBox

                ClearForm(); // Clear the input fields
                MessageBox.Show("All records removed successfully!");
            }
        }

        // Load records to server button click event
        private void btnLoadToServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveStudentsToJson(); // Save current records to JSON file

                students.Clear(); // Clear student collection
                SINComboBox.Items.Clear(); // Clear SIN ComboBox
                ClearForm(); // Clear input fields

                MessageBox.Show("Data successfully loaded to server!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Method to refresh the DataGrid with current student data
        private void RefreshStudentDataGrid()
        {
            StudentData.ItemsSource = null; // Clear current ItemsSource
            StudentData.ItemsSource = students; // Set the updated list as the new data source
        }

        // Event handler for the Exit button
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close the application
        }

        // Event handler for SIN ComboBox selection change
        private void SINComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SINComboBox.SelectedItem is string selectedSin)
            {
                // Find the selected student using the selected SIN
                Student selectedStudent = students.FirstOrDefault(s => s.sin == selectedSin);
                if (selectedStudent != null)
                {
                    // Populate fields with selected student's information
                    PopulateFields(selectedStudent);
                }
            }
        }
    }
}