using SendGridAzureEmail.Class;
using SendGridAzureEmail.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SendGridAzureEmail.Controllers
{
    public class SendEmailController : Controller
    {
        SecurityClass security = new SecurityClass();
        ResponseModel response = new ResponseModel();
        AzureClass azure = new AzureClass();
        MailMessage mail = new MailMessage();
        SmtpClient smtpClient = new SmtpClient();
        String usermail, passwordmail;
        string v_Html0, v_Html1, v_Html2, v_Html3;
        string sendfile;
        string port;
        Attachment data;
        Stream streamAzure;
        public string SendEmailContainer([FromBody] EmailModel parametros)
        {
            //Obtención de archivos
            string urlcont = azure.GetUrl(parametros.Container);
            List<string> ContainersFiles = azure.ListBlobFile(PathBlob: urlcont, ContainerBlobName: parametros.Container);
            //Inicialización de remitente
            usermail = security.DesEncriptar("awBlAHYAOAAxAC4AZwB0AHoAQABnAG0AYQBpAGwALgBjAG8AbQA=");
            passwordmail = security.DesEncriptar("QwBWADAAOABzAHgANwBJAHEAOQB6AFIAUQB5AGcAawA=");
            mail.From = new MailAddress(usermail);
            // Añadiendo destinatarios
            for (int i = 0; i < parametros.EmailsAddressTO.Count; i++)
            {
                mail.To.Add(parametros.EmailsAddressTO[i]);
            }
            for (int i = 0; i < parametros.EmailsAddressCC.Count; i++)
            {
                mail.CC.Add(parametros.EmailsAddressCC[i]);
            }
            mail.Subject = parametros.Subject.ToString();//Titulo del correo            
            //Definiendo estructura del mensaje
            v_Html0 = "<p>" + parametros.Messagge + "</p>" + "<br>";
            v_Html1 = v_Html0 + "<table class='table align-content-start table-bordered shadow'> " + "<tr>" + "<td colspan=\"2\" style=\"background-color: #3366CC; color: #FFFFFF; font-weight: bold; text-align: center;\">Archivos procesados en el contenedor " + parametros.Container + "</td> " + "</tr> ";
            if (ContainersFiles.Count > 0)
            {
                v_Html2 = " <tr> " + " <td style=\"background-color: #E8E8EC; font-weight: bold\">NOMBRE</td>" + " <td style=\"background-color: #E8E8EC; font-weight: bold\">URL</td> " + " </tr> ";
                for (int z = 0; z < ContainersFiles.Count; z++)
                {
                    v_Html2 = v_Html2 + " <tr> " + " <td>" + ContainersFiles[z] + "</td> " + " <td>" + urlcont + ContainersFiles[z] + "</td>" + " </tr> ";
                }
                v_Html3 = " </table>";
                parametros.Messagge = v_Html1 + v_Html2 + v_Html3;
                mail.IsBodyHtml = true;
            }
            else
            {
                mail.Body = parametros.Messagge; //"<h2 style=\"color:red;\">" + "HOLA" + "</h2>";
                mail.IsBodyHtml = false;
            }
            //Añaiendo el mensaje
            var htmlView = AlternateView.CreateAlternateViewFromString(parametros.Messagge, null, "text/html");
            mail.AlternateViews.Add(htmlView);

            //Configuración de envio
            mail.Priority = MailPriority.Normal;
            smtpClient.Host = "smtp-relay.sendinblue.com";
            port = security.DesEncriptar("NQA4ADcA");
            smtpClient.Port = Convert.ToInt32(port);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            smtpClient.UseDefaultCredentials = false;
            NetworkCredential credential = new NetworkCredential(usermail, passwordmail);
            smtpClient.Credentials = credential;

            try
            {
                //await smtpClient.SendMailAsync(mail);//Envio del correo
                smtpClient.Send(mail);//Envio del correo
                response.CodeResponse = 1;
                response.MessageResponse = "Email enviado correctamente";
            }
            catch (Exception ex)
            {
                response.CodeResponse = 0;
                response.MessageResponse = "Email NO Enviado " + ex.Message + " " + ex.InnerException;
            }
            return response.MessageResponse;
        }

        public string SendNotify([FromBody] EmailModel parametros)
        {
            //definiendo remitente
            usermail = security.DesEncriptar("awBlAHYAOAAxAC4AZwB0AHoAQABnAG0AYQBpAGwALgBjAG8AbQA=");
            passwordmail = security.DesEncriptar("QwBWADAAOABzAHgANwBJAHEAOQB6AFIAUQB5AGcAawA=");
            mail.From = new MailAddress(usermail);
            //añdiendo destinatarios
            for (int i = 0; i < parametros.EmailsAddressTO.Count; i++)
            {
                mail.To.Add(parametros.EmailsAddressTO[i]);
            }
            for (int i = 0; i < parametros.EmailsAddressCC.Count; i++)
            {
                mail.CC.Add(parametros.EmailsAddressCC[i]);
            }
            mail.Subject = parametros.Subject.ToString();
            //Definiendo mensaje y estructura
            if (parametros.Type)
            {
                mail.Body = "<h3 style=\"color:green;\">" + parametros.Messagge + "</h3>";
                mail.Priority = MailPriority.Normal;
            }
            else
            {
                mail.Body = "<h3 style=\"color:red;\">" + parametros.Messagge + "</h3>";
                mail.Priority = MailPriority.High;
            }
            mail.IsBodyHtml = true;

            //Configuración de envio            
            smtpClient.Host = "smtp-relay.sendinblue.com";
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            port = security.DesEncriptar("NQA4ADcA");
            smtpClient.Port = Convert.ToInt32(port);

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            NetworkCredential credential = new NetworkCredential(usermail, passwordmail);
            smtpClient.Credentials = credential;

            try
            {
                //await smtpClient.SendMailAsync(mail);//Envio del correo
                smtpClient.Send(mail);//Envio del correo
                response.CodeResponse = 1;
                response.MessageResponse = "Email enviado correctamente";
            }
            catch (Exception ex)
            {
                response.CodeResponse = 0;
                response.MessageResponse = "Email NO Enviado " + ex.Message + " " + ex.InnerException;
            }
            return response.MessageResponse;
        }

        public string SendDocumentsReports([FromBody] EmailModel parametros)
        {
            ////Obtención de archivos
            string urlcont = azure.GetUrl(parametros.Container);
            List<string> ContainersFiles = azure.ListBlobFile(PathBlob: urlcont, ContainerBlobName: parametros.Container);
            //definiendo remitente
            usermail = security.DesEncriptar("awBlAHYAOAAxAC4AZwB0AHoAQABnAG0AYQBpAGwALgBjAG8AbQA=");
            passwordmail = security.DesEncriptar("QwBWADAAOABzAHgANwBJAHEAOQB6AFIAUQB5AGcAawA=");
            mail.From = new MailAddress(usermail);
            //añdiendo destinatarios
            for (int i = 0; i < parametros.EmailsAddressTO.Count; i++)
            {
                mail.To.Add(parametros.EmailsAddressTO[i]);
            }
            for (int i = 0; i < parametros.EmailsAddressCC.Count; i++)
            {
                mail.CC.Add(parametros.EmailsAddressCC[i]);
            }
            mail.Subject = parametros.Subject.ToString();
            //Definiendo mensaje y estructura
            mail.Body = "<h4>" + parametros.Messagge + "</h4>";
            mail.IsBodyHtml = true;

            for (int s = 0; s < ContainersFiles.Count; s++)
            {
                sendfile = urlcont + ContainersFiles[s];
                string FName = ContainersFiles[s].ToString();
                if (parametros.NombresArchivos.Count == 1)
                {
                    streamAzure = azure.StreamGetStream(parametros.Container, FName);
                    // Añadiendo archivos.
                    data = new Attachment(streamAzure, FName, MediaTypeNames.Application.Octet);
                    mail.Attachments.Add(data);
                }
                else if (parametros.NombresArchivos.Count > 1)
                {
                    for (int z = 0; z < parametros.NombresArchivos.Count; z++)
                    {
                        if (parametros.NombresArchivos[z].ToString() == FName)
                        {
                            streamAzure = azure.StreamGetStream(parametros.Container, FName);
                            // Añadiendo archivos.
                            data = new Attachment(streamAzure, FName, MediaTypeNames.Application.Octet);
                            mail.Attachments.Add(data);
                        }
                    }
                }
                else
                {
                    streamAzure = azure.StreamGetStream(parametros.Container, FName);
                    // Añadiendo archivos.
                    data = new Attachment(streamAzure, FName, MediaTypeNames.Application.Octet);
                    mail.Attachments.Add(data);
                }

            }
            //Configuración de envio
            mail.Priority = MailPriority.High;
            smtpClient.Host = "smtp-relay.sendinblue.com";
            port = security.DesEncriptar("NQA4ADcA");
            smtpClient.Port = Convert.ToInt32(port);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            smtpClient.UseDefaultCredentials = false;
            NetworkCredential credential = new NetworkCredential(usermail, passwordmail);
            smtpClient.Credentials = credential;

            try
            {
                //await smtpClient.SendMailAsync(mail);//Envio del correo
                smtpClient.Send(mail);//Envio del correo
                response.CodeResponse = 1;
                response.MessageResponse = "Email enviado correctamente";
            }
            catch (Exception ex)
            {
                response.CodeResponse = 0;
                response.MessageResponse = "Email NO Enviado " + ex.Message + " " + ex.InnerException;
            }
            return response.MessageResponse;
        }
    }
}