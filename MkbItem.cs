using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patients
{
    public class MkbItem
    {
        public string Code { get; }
        public string Name { get; }
        public MkbItem(string code, string name) 
        {
            Code = code;
            Name = name;
        }
    }
}
