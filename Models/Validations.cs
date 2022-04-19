using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeetUp.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        private DateTime _Now;
        private string _Message;
        public FutureDateAttribute() : base()
        {
            _Now = DateTime.Now;
            _Message = "Dates must be in the future!";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime EventDate = (DateTime)value;
            if (DateTime.Compare(_Now, EventDate) >= 0)
            {
                return new ValidationResult("Choose a Future Date!");
            }

            return ValidationResult.Success;
        }
    }
}
