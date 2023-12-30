using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patients
{
    public class Visit
    {
        public string ID { get; set; }
        public DateTime date { get; set; }
        public string diagnosis { get; set; }
        public string codeMKB10 { get; set; }
        public string patientID { get; set; }
        public Visit (string ID, DateTime date, string diagnosis, string codeMKB10, string patientID)
        {
            this.ID = ID;
            this.date = date;
            this.diagnosis = diagnosis;
            this.codeMKB10 = codeMKB10;
            this.patientID = patientID;
        }
    }
}
