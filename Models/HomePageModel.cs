using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUp.Models
{
    public class HomepageView
    {
        public User User { get; set; }
        public LoginView Login { get; set; }
    }
}