using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class Message
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Обязательное поле для ввода")]
        public string Text { get; set; }

        public string Format { get; set; }

        public byte[] FileString { get; set; }

        public DateTime When { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        public string UserId { get; set; }

        public User Sender { get; set; }

        public bool PersonalMessage { get; set; }


    }
}
