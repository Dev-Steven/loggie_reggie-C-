    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;   // Add this for [NotMapped]

    namespace loggie_reggie.Models
    {
        public class User
        {
            [Key]
            public int Id {get;set;}

            [Required]
            [MinLength(2)]
            public string FirstName {get;set;}

            [Required]
            [MinLength(2)]
            public string LastName {get;set;}

            [Required]
            [EmailAddress]
            public string Email {get;set;}

            [Required]
            [MinLength(8)]
            [DataType(DataType.Password)]
            public string Password {get;set;}

            public DateTime CreatedAt {get;set;}
            public DateTime UpdatedAt {get;set;}

            [NotMapped]
            [Required]
            [Compare("Password")]
            [DataType(DataType.Password)]
            public string Confirm {get;set;}

        }
    }