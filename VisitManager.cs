using Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Patients
{
    public class VisitManager
    {
        public BindingList<Visit> visitsList { get; set; } // List of Visits
        public BindingList<MkbItem> mkb10List = null;
        private SqlConnection connection = null;
        public VisitManager(SqlConnection connection, BindingList<MkbItem> mkb10list)
        {
            visitsList = new BindingList<Visit>();
            mkb10List = mkb10list;
            this.connection = connection;
            // Import data from database
            SqlDataAdapter adapter = new SqlDataAdapter(
                "SELECT Id, Date, Diagnosis, CodeMKB10, PatientId FROM Visits", connection);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                Visit visit = new Visit(
                    row["Id"].ToString(), DateTime.Parse(row["Date"].ToString()), row["Diagnosis"].ToString(),
                    row["CodeMKB10"].ToString(), row["PatientId"].ToString()
                    );
                visitsList.Add(visit);
            }
        }
        public void addVisit(DateTime date, string diagnosis, string codeMKB10, string patientID)
        {
            // Add visit to the database
            Guid guid;
            string id;
            do
            {
                guid = Guid.NewGuid();
                id = guid.ToString("D");
            } while (!isUniqueId(id));

            SqlCommand addCommand = new SqlCommand(
                $"INSERT INTO [Visits] (Id, Date, Diagnosis, CodeMKB10, PatientId) VALUES (@Id, @Date, @Diagnosis, @CodeMKB10, @PatientId)",
                connection);
            addCommand.Parameters.AddWithValue("Id", id);
            addCommand.Parameters.AddWithValue("Date", date);
            addCommand.Parameters.AddWithValue("Diagnosis", diagnosis);
            addCommand.Parameters.AddWithValue("CodeMKB10", codeMKB10);
            addCommand.Parameters.AddWithValue("PatientID", patientID);
            addCommand.ExecuteNonQuery();
            // Add visit to the list 
            Visit visit = new Visit(id, date, diagnosis, codeMKB10, patientID);
            visitsList.Add(visit);
        }
        private bool isUniqueId(string id) //Check id
        {
            SqlCommand checkCommand = new SqlCommand($"SELECT COUNT(*) FROM Visits WHERE Id = @Id", connection);
            checkCommand.Parameters.AddWithValue("Id", id);
            int count = (int)checkCommand.ExecuteScalar();
            return count == 0;
        }
    }
}
