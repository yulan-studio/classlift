
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;





namespace Core.Models
{
    

    public class User : IdentityUser<int> // Use int as primary key
    {
        //[Key]
        //public int UserID { get; set; } // Primary Key


        //[StringLength(45)]
        //public required string Email { get; set; } // User Email


        //[StringLength(255)]
        //public required string Password { get; set; } // User Password
        //public int Id { get; set; }

        public required string Role { get; set; } // User Role (Admin, Staff, Coach, Child)

        public int? CreatedBy { get; set; } // Created By User ID (nullable)

        public int? UpdatedBy { get; set; } // Updated By User ID (nullable)

        public DateTime? CreatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Created Timestamp (nullable)

        public DateTime? UpdatedDate { get; set; } = DateTimeHelper.GetTorontoTime(); // Updated Timestamp (nullable)
    }

    // Enum for UserRole
    //public enum UserRole
    //{
    //    Admin,
    //    Staff,
    //    Coach,
    //    Child
    //}

    
}
