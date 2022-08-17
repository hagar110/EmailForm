using EmailForm.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmailForm.Controllers
{
    public class EmailController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //[HttpGet]
       public void DeleteFile(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send email service: cannot delete file {path} {ex.Message}");
            }
        }
        public async Task SendEmail(string toEmailAddress, string emailSubject, string emailMessage, HttpPostedFileBase file)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.To.Add(toEmailAddress);
                message.From = new MailAddress("hagar.elsherif999@gmail.com");
                message.Subject = emailSubject;
                message.Body = emailMessage;
                string Uplodefile = Request.PhysicalApplicationPath + "UploadedFiles\\" + file.FileName;
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("hagar.elsherif999@gmail.com", "your app password"),
                    EnableSsl = true,
                  //  SmtpClient client = new SmtpClient();
                DeliveryMethod = SmtpDeliveryMethod.Network,
  
            };
                Attachment attachment = new Attachment(Uplodefile);
                message.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(message);
               if (attachment != null)
                {
                    attachment.Dispose();
                }
               /* if (Uplodefile != null)
                {
                    DeleteFile(Uplodefile);
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception" + ex);
                // ShowMessage(ex.Message);
            }



        }

        [HttpPost]
        //Task<IActionResult>
        public async Task<ViewResult> Index(EmailModel emailModel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (emailModel.File.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(emailModel.File.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                        emailModel.File.SaveAs(_path);
                    }

                    ViewBag.Message = "File Uploaded Successfully!!";
                    sendDataToDataBase(emailModel);
                    await SendEmail(emailModel.Email, emailModel.Subject, emailModel.Content, emailModel.File);

                    return View();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception" + ex);
                    ViewBag.Message = "File upload failed!!";
                    return View();
                }
            }
            return View(emailModel);
        }

        public void sendDataToDataBase(EmailModel emailModel)
        {
            SqlConnection con;
            con = new SqlConnection("Data Source=DESKTOP-S691FHC\\SQLEXPRESS;Initial Catalog=EmailsDB;Integrated Security=True");
            //byte[] buffer = new byte[Request.Files["File"].ContentLength];
            // cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = TextBox1.Text;
            int filelength = emailModel.File.ContentLength;
            Stream filestream = emailModel.File.InputStream;
            byte[] filedata = new byte[filelength];
            string filename = Path.GetFileName(emailModel.File.FileName);
            filestream.Read(filedata, 0, filelength);
            SqlCommand cmd = new SqlCommand("insertEmail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = emailModel.Email;
            cmd.Parameters.Add("@Subject", SqlDbType.NVarChar).Value = emailModel.Subject;
            cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = emailModel.Content;
            cmd.Parameters.Add("@File", SqlDbType.VarBinary).Value = filedata;
            con.Open();
            // SqlCommand comm = new SqlCommand("EXEC insertEmail @Email=" + emailModel.Email + ", @Subject=" + emailModel.Subject + ", @Content=" + emailModel.Content + ", @File=" + emailModel.File);
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}