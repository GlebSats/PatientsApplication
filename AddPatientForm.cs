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
    public partial class AddPatientForm : Form
    {
        private PatientManager patientManager;
        public AddPatientForm(PatientManager patientManager)
        {
            InitializeComponent();
            this.patientManager = patientManager;
        }
        // Cleaning help string of phone number 
        private void addPhoneTextBox_Enter(object sender, EventArgs e)
        {
            addPhoneTextBox.Text = string.Empty;
            addPhoneTextBox.ForeColor = SystemColors.WindowText;
        }
        // Ok button event
        private void addOkButton_Click(object sender, EventArgs e)
        {
            try
            {
                patientManager.addPatient(
                    addSurnameTextBox.Text,
                    addNameTextBox.Text,
                    addPatTextBox.Text,
                    addDateTimePicker.Value,
                    addPhoneTextBox.Text
                );
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
