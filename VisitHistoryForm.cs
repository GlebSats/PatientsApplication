using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patients
{
    public partial class VisitHistoryForm : Form
    {
        private VisitManager visitManager = null;
        private Patient patient = null;
        public VisitHistoryForm(VisitManager visitManager, Patient patient)
        {
            InitializeComponent();
            this.visitManager = visitManager;
            this.patient = patient;
        }

        private void VisitHistoryForm_Load(object sender, EventArgs e)
        {
            var filteredList = visitManager.visitsList.Where(p => p.patientID.Equals(patient.ID));
            if (filteredList.Count() == 0)
            {
                MessageBox.Show("У этого пациента еще не было посещений", "Здоров", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            // Fill patient information
            idLabel.Text = patient.ID;
            surnameLabel.Text = patient.Surname;
            nameLabel.Text = patient.Name;
            patronymiclLabel.Text = patient.Patronymic;
            BDLabel.Text = patient.Birthday.ToLongDateString();
            phoneLabel.Text = patient.PhoneNumber;
            // Fill visits table
            foreach (var v in filteredList)
            {
                MkbItem mkbDiagnosis = visitManager.mkb10List.FirstOrDefault(item => item.Code == v.codeMKB10);

                int row = VisitsDataGridView.Rows.Add();
                VisitsDataGridView.Rows[row].Cells["IdColumn"].Value = v.ID;
                VisitsDataGridView.Rows[row].Cells["DateColumn"].Value = v.date.ToShortDateString();
                VisitsDataGridView.Rows[row].Cells["DiagnosisColumn"].Value = v.diagnosis;
                VisitsDataGridView.Rows[row].Cells["MKB10Column"].Value = mkbDiagnosis.Name;
            }
        }
        // Export xml file
        private void ExportButton_Click(object sender, EventArgs e)
        {
            DataSet dataSet = new DataSet("История посещений");
            // Fill patient table 
            DataTable patientTable = new DataTable("Данные пациента");
            patientTable.Columns.Add("", typeof(string));
            patientTable.Columns.Add("", typeof(string));
            patientTable.Rows.Add("Ид пациента", idLabel.Text);
            patientTable.Rows.Add("Фамилия", surnameLabel.Text);
            patientTable.Rows.Add("Имя", nameLabel.Text);
            patientTable.Rows.Add("Отчество", patronymiclLabel.Text);
            patientTable.Rows.Add("Дата рождения", BDLabel.Text);
            patientTable.Rows.Add("Телефон", phoneLabel.Text);
            // Fill visits table
            DataTable visitsTable = new DataTable("Посещения пациента");
            foreach (DataGridViewColumn col in VisitsDataGridView.Columns)
            {
                visitsTable.Columns.Add(col.HeaderText, typeof(string));
            }
            foreach (DataGridViewRow row in VisitsDataGridView.Rows)
            {
                DataRow dataRow = visitsTable.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dataRow[cell.ColumnIndex] = cell.Value;
                }
                visitsTable.Rows.Add(dataRow);
            }

            dataSet.Tables.Add(patientTable);
            dataSet.Tables.Add(visitsTable);
            // Write to xml file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string exportFilePath = saveFileDialog.FileName;
                    dataSet.WriteXml(saveFileDialog.FileName);
                    MessageBox.Show("XML файл экспортирован", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
