﻿using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendGridAzureEmail.Models
{
    public class EmailModel
    {
        public List<Dictionary<string, object>> jsonmessage { get; set; }
        public string Messagge { get; set; }
        public List<String> EmailsAddressTO { get; set; }
        public string EmailsAddressFROM { get; set; }
        public string Subject { get; set; }
        public bool displayRecipients { get; set; }
        public string HTML { get; set; }
        public string TEXT { get; set; }

    }
}