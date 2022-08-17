using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace EmailForm.Models
{
    public class Files
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int DocumentId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string FileType { get; set; }
        [MaxLength]
        public byte[] DataFiles { get; set; }
// public DateTime? CreatedOn { get; set; }
    }
    public class BufferedSingleFileUploadDb
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
    public class EmailModel
    {
        [Required(ErrorMessage = "Please enter a email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter a content.")]
        [Display(Name = "Content")]
        public string Content { get; set; }
        
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Please enter a file.")]
        public HttpPostedFileBase File { get; set; }
    }
}