using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SadnaApi.Models
{
    /// <summary>
    /// Represents a user model for storing user account information with model included but without password and private data.
    /// </summary>
    /// 
    public class UserWithRoleModel : UserWithoutRoleModel
    {

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [Required(ErrorMessage = "The Role field is required.")]
        public string Role { get; set; }


    }
}
