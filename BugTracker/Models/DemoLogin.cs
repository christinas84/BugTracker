using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class DemoLogin
    {
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }

        [Required]
        [EmailAddress]
        public string PMEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PMPassword { get; set; }

        [Required]
        [EmailAddress]
        public string DevEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string DevPassword { get; set; }

        [Required]
        [EmailAddress]
        public string SubEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string SubPassword { get; set; }
    }
}