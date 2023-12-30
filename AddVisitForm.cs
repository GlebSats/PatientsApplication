using Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patients
{
    public partial class AddVisitForm : Form
    {
        private BindingSource bindingSource = null;
        private VisitManager visitManager = null;
        private Patient patient = null;
        public AddVisitForm(VisitManager visitManager, Patient patient)
        {
            InitializeComponent();
            this.visitManager = visitManager;
            this.patient = patient;
        }

        private void addVisit_Load(object sender, EventArgs e)
        {
            dateLabel.Text = DateTime.Now.ToLongDateString();
            //Binding mkb10 list to listBox
            bindingSource = new BindingSource();
            bindingSource.DataSource = visitManager.mkb10List;
            diagnosisListBox.DataSource = bindingSource;
            diagnosisListBox.DisplayMember = "Name";
        }
        // Search name of diagnosis
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
                string searchText = searchTextBox.Text.ToLower();
                var filteredList = visitManager.mkb10List.Where(i => i.Name.ToLower().Contains(searchText)).ToList();
                bindingSource.DataSource = new BindingList<MkbItem>(filteredList);
        }
        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "Поиск диагноза в МКБ-10 по названию")
            {
                searchTextBox.Text = string.Empty;
                searchTextBox.ForeColor = SystemColors.WindowText;
            }
        }
        private void diagnosisTextBox_Enter(object sender, EventArgs e)
        {
            if (diagnosisTextBox.Text == "Введите диагноз")
            {
                diagnosisTextBox.Text = string.Empty;
                diagnosisTextBox.ForeColor = SystemColors.WindowText;
            }
        }
        private void diagnosisTextBox_Leave(object sender, EventArgs e)
        {
            if (diagnosisTextBox.Text == string.Empty)
            {
                diagnosisTextBox.Text = "Введите диагноз";
                diagnosisTextBox.ForeColor = SystemColors.WindowFrame;
            }
        }
        // Add new visit
        private void saveVisitButton_Click(object sender, EventArgs e)
        {
            if (diagnosisTextBox.Text == string.Empty || diagnosisTextBox.Text == "Введите диагноз")
            {
                MessageBox.Show("Введите диагноз пациента", "Диагноз отсутствует", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (diagnosisListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите диагноз пациента из справочника МКБ-10", "Диагноз не выбран", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else
            {
                MkbItem diagnosis = (MkbItem)diagnosisListBox.SelectedItem;
                visitManager.addVisit(DateTime.Now, diagnosisTextBox.Text, diagnosis.Code, patient.ID);
                Close();
            }
        }
    }
}
