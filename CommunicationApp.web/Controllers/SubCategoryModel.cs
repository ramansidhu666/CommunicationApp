using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CommunicationApp.Models
{
    public class SubCategoryModel
    {
        public int SubCategoryId { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(20, ErrorMessage = "length cannot exceed 20 character.")]
        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Required")]
        public int CategoryId { get; set; }
        public string PhotoPath { get; set; }
        public string Price { get; set; }
        public bool IsActive { get; set; }
    }
}
