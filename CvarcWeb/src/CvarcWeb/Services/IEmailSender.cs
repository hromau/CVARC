using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CvarcWeb.Services
{
    public interface IEmailSender
    {
        void SendEmail(string email, string subject, string message);
    }
}
