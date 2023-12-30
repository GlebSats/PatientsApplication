using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using System.Data.Common;

namespace Patients
{
    public class PatientManager // Manage patients class
    {
        public BindingList<Patient> patientsList { get; set; } // List of Patients
        private SqlConnection connection = null;
        public PatientManager(SqlConnection connection)
        {
            patientsList = new BindingList<Patient>();
            this.connection = connection;
            // Import data from database
            SqlDataAdapter adapter = new SqlDataAdapter(
                "SELECT Id, Surname, Name, Patronymic, Birthday, PhoneNumber FROM Patients", connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                Patient patient = new Patient(
                    row["Id"].ToString(), row["Surname"].ToString(), row["Name"].ToString(), row["Patronymic"].ToString(),
                    DateTime.Parse(row["Birthday"].ToString()), row["PhoneNumber"].ToString()
                    );
                patientsList.Add(patient);
            }
        }
        public void addPatient(string surname, string name, string patronymic, DateTime birthday, string phone)
        {
            if (surname.Length < 2)
            {
                throw new ArgumentException("Фамилия должна содержать хотя бы две буквы");
            }
            if (!isValidName(surname))
            {
                throw new ArgumentException("Фамилия должна содержать только буквы из русского алфавита");
            }
            if (name.Length < 2)
            {
                throw new ArgumentException("Имя должно содержать хотя бы две буквы");
            }
            if (!isValidName(name))
            {
                throw new ArgumentException("Имя должно содержать только буквы из русского алфавита");
            }
            if (patronymic.Length < 2)
            {
                throw new ArgumentException("Отчество должно содержать хотя бы две буквы");

            }
            if (!isValidName(patronymic))
            {
                throw new ArgumentException("Отчество должно содержать только буквы из русского алфавита");
            }
            if (birthday > DateTime.Now)
            {
                throw new ArgumentException("Дата рождения не может быть в будущем времени");
            }
            if (!isValidPhone(phone))
            {
                throw new ArgumentException("Неправильный формат номера телефона.\n(Номер телефона может начинаться со знака '+' и должен содержать только цифры)");
            }
            if (phone.Length < 4)
            {
                throw new ArgumentException("Номер телефона слишком короткий");
            } 
            // Add patient to the database
            Guid guid;
            string id;
            do
            {
                guid = Guid.NewGuid();
                id = guid.ToString("D");
            } while (!isUniqueId(id));

            SqlCommand addCommand = new SqlCommand(
                $"INSERT INTO [Patients] (Id, Surname, Name, Patronymic, Birthday, PhoneNumber) VALUES (@Id, @Surname, @Name, @Patronymic, @Birthday, @PhoneNumber)",
                connection);
            addCommand.Parameters.AddWithValue("Id", id);
            addCommand.Parameters.AddWithValue("Surname", surname);
            addCommand.Parameters.AddWithValue("Name", name);
            addCommand.Parameters.AddWithValue("Patronymic", patronymic);
            addCommand.Parameters.AddWithValue("Birthday", birthday);
            addCommand.Parameters.AddWithValue("PhoneNumber", phone);
            addCommand.ExecuteNonQuery();
            // Add patient to the list 
            Patient patient = new Patient(id, surname, name, patronymic, birthday, phone);
            patientsList.Add(patient);
        }
        public void deletePatient(Patient patient)
        {
            // Delete patient from the list
            patientsList.Remove(patient);
            // Delete patient from the database
            SqlCommand deleteCommand = new SqlCommand("DELETE FROM Patients WHERE Id = @Id", connection);
            deleteCommand.Parameters.AddWithValue("Id", patient.ID);
            deleteCommand.ExecuteNonQuery();
        }
        private bool isValidName(string name) // Name/Surname/Patronymic check
        {
            return name.All(c => char.IsLetter(c) && (c >= 'А' && c <= 'я'));
        }
        private bool isValidPhone(string phone) // Phone number check
        {
            if (Regex.IsMatch(phone, @"^[+]?[0-9]+$"))
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
        private bool isUniqueId(string id) // Check id
        {
            SqlCommand checkCommand = new SqlCommand($"SELECT COUNT(*) FROM Patients WHERE Id = @Id", connection);
            checkCommand.Parameters.AddWithValue("Id", id);
            int count = (int)checkCommand.ExecuteScalar();
            return count == 0;
        }
    }
}
