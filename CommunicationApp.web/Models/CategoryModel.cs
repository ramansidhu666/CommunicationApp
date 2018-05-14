using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommunicationApp.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [RegularExpression("^([a-zA-Z .&'-]+)$", ErrorMessage = "Invalid Category Name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(20 ,ErrorMessage ="length cannot exceed 20 character.")]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public List<SubCategoryModel> SubCategoryModelList { get; set; }
    }
}