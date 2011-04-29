using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Net.NetworkInformation;

namespace SceneStudioApp
{
    class SceneUploader
    {
        static public void UploadScene(string data, string filename)
        {
            MailMessage message = new MailMessage("scene.studio.collector@gmail.com", "scene.studio.collector@gmail.com", "subject", "body");

            message.Sender = new MailAddress("scene.studio.collector@gmail.com");
            SmtpClient emailClient = new SmtpClient("smtp.gmail.com");
            System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("scene.studio.collector@gmail.com", "*verysecret*");

            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            Int64 timestamp  = (Int64) t.TotalSeconds;
            
            string userID = getUserID();

            string finalID = userID.Replace('_', '-') + "_" + timestamp.ToString() + "_" + filename.Replace('_', '-').Replace(' ','-');

            message.Subject = "submission " + finalID;
            message.Body = finalID;

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            MemoryStream stream = new MemoryStream(bytes);
            Attachment attachment = new Attachment(stream, finalID, "text/plain");
            message.Attachments.Add(attachment);
                
            emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            emailClient.EnableSsl = true;
            emailClient.Port = 587;
            emailClient.UseDefaultCredentials = false;
            emailClient.Credentials = SMTPUserInfo;

            emailClient.Send(message);
        }

        static private string getUserID()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            string result = computerProperties.HostName + "_";
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
            {
                return result + "xxxxxxxxxxxxxxxx";
            }

            PhysicalAddress address = nics[0].GetPhysicalAddress();
            byte[] bytes = address.GetAddressBytes();
            for (int i = 0; i < bytes.Length; i++)
            {
                result += bytes[i].ToString("X2");
            }

            return result;
        }
    }
}
