using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patients
{
    public class Patient // Class Patient
    {
        public string ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public Patient(string id, string surname, string name, string patronymic, DateTime birthday, string phone)
        {
            ID = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Birthday = birthday;
            PhoneNumber = phone;
        }
    }
}
