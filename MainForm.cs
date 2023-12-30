using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Subjects;
using System.IO;
using OfficeOpenXml;


namespace Patients
{
    public partial class MainForm : Form
    {
        private string mkb10Path = "D:\\C++ projects\\Subjects\\Subjects\\bin\\Debug\\mkb10.xlsx";
        private BindingList<MkbItem> mkb10List = null;
        private List<Patient> patientsSortedList = null;
        private SqlConnection sqlConnection = null;
        private PatientManager patientManager = null;
        private VisitManager visitManager = null;
        private BindingSource bindingSource = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Open connections to the database
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PatientsDB"].ConnectionString);
            try
            {
                sqlConnection.Open();
            }
            catch 
            {
                MessageBox.Show("Не удалось подключиться к базе данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            //Load MKB10 Catalog
            mkb10List = new BindingList<MkbItem>();
            loadMKB10();

            patientManager = new PatientManager(sqlConnection); // Create PatientManager class instance
            visitManager = new VisitManager(sqlConnection, mkb10List); // Create VisitManager class instance
            //Binding patientsList to listBox
            bindingSource = new BindingSource();
            FilterComboBox.SelectedIndex = 0;
            bindingSource.DataSource = patientsSortedList;
            listBox.DataSource = bindingSource;
            listBox.DisplayMember = "Surname";
        }
        // Add patient button event
        private void addPatientButton_Click(object sender, EventArgs e)
        {
            AddPatientForm addPatient = new AddPatientForm(patientManager);
            addPatient.ShowDialog();
            refreshPatientList();
            bindingSource.DataSource = patientsSortedList;
            refreshForm();
        }
        // Add visit button event
        private void addVisitButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                AddVisitForm visitForm = new AddVisitForm(visitManager, (Patient)listBox.SelectedItem);
                visitForm.ShowDialog();
            }
        }
        // Delete button event
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show("Вы действительно хотите удалить пациента?", "Удаление пациента", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    Patient patientToDelete = (Patient)listBox.SelectedItem;
                    patientManager.deletePatient(patientToDelete);
                    refreshPatientList();
                    bindingSource.DataSource = patientsSortedList;
                    refreshForm();
                }
            }
        }
        // Binding labels to patients parameters
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshForm();
        }
        private void refreshForm()
        {
            if (listBox.SelectedItem != null)
            {
                Patient patient = (Patient)listBox.SelectedItem;
                idLabel.Text = patient.ID;
                surnameLabel.Text = patient.Surname;
                nameLabel.Text = patient.Name;
                patronymiclLabel.Text = patient.Patronymic;
                BDLabel.Text = patient.Birthday.ToLongDateString();
                phoneLabel.Text = patient.PhoneNumber;
            } else
            {
                idLabel.Text = string.Empty;
                surnameLabel.Text = string.Empty;
                nameLabel.Text = string.Empty;
                patronymiclLabel.Text = string.Empty;
                BDLabel.Text = string.Empty;
                phoneLabel.Text = string.Empty;
            }
        }
        // Search surname from list
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (searchTextBox.Text != "Поиск по фамилии")
            {
                string searchText = searchTextBox.Text.ToLower();
                var filteredList = patientsSortedList.Where(p => p.Surname.ToLower().Contains(searchText)).ToList();
                bindingSource.DataSource = new BindingList<Patient>(filteredList);
                refreshForm();
            }
        }
        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "Поиск по фамилии")
            {
                searchTextBox.Text = string.Empty;
                searchTextBox.ForeColor = SystemColors.WindowText;
            }
        }
        private void searchTextBox_Leave(object sender, EventArgs e)
        {
            searchTextBox.Text = "Поиск по фамилии";
            searchTextBox.ForeColor = SystemColors.WindowFrame;
        }
        // Load xlsx file with mkb10 table
        private void loadMKB10()
        {
            if (!File.Exists(mkb10Path))
            {
                MessageBox.Show("Не удалось найти файл mkb10.xlsx", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(mkb10Path)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 5; row <= rowCount; row++)
                    {
                        MkbItem mkbItem = new MkbItem(worksheet.Cells[row, 1].Text, worksheet.Cells[row, 2].Text);
                        mkb10List.Add(mkbItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении Excel файла: {ex.Message}");
            }
        }
        // Open visit history
        private void visitsHistoryButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                VisitHistoryForm visitHistoryForm = new VisitHistoryForm(visitManager, (Patient)listBox.SelectedItem);
                visitHistoryForm.ShowDialog();
            }
        }
        // Sort list
        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshPatientList();
        }
        private void refreshPatientList()
        {
            if (FilterComboBox.SelectedIndex == 0)
            {
                patientsSortedList = patientManager.patientsList.OrderBy(p => p.Surname).ToList();
                bindingSource.DataSource = new BindingList<Patient>(patientsSortedList);
            }
            else if (FilterComboBox.SelectedIndex == 1)
            {
                patientsSortedList = patientManager.patientsList.OrderByDescending(p => p.Birthday).ToList();
                bindingSource.DataSource = new BindingList<Patient>(patientsSortedList);
            }
            else if (FilterComboBox.SelectedIndex == 2)
            {
                patientsSortedList = patientManager.patientsList.OrderBy(p => p.Birthday).ToList();
                bindingSource.DataSource = new BindingList<Patient>(patientsSortedList);
            }
            refreshForm();
        }
    }
}
