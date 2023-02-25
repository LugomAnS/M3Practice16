using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNet.Models
{
    class Client
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }

        public Client() { }
    }
}
