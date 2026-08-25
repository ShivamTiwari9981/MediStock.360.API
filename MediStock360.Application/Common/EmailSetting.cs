using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Common
{
    public class EmailSettings
    {
        public string DisplayName { get; set; }

        public string FromEmail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SmtpServer { get; set; }

        public int Port { get; set; }
    }
}
