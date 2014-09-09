using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace ErpContent.Models
{

    public class ModelBase
    {

        public ModelBase()
        {
        }

        #region --- Properties ---

        /// <summary>
        /// Gets or sets return URL
        /// </summary>
        public string ReturnUrl { get; set; }
        #endregion --- Properties ---
    }



    public class LoginModel : ModelBase
    {
        public LoginModel()
        {
            RememberMe = false;
        }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[RegularExpression(Code.Helpers.StaticHelper._RegExp_UPwd_, ErrorMessage = "Password may consists of letters, numbers and special characters (!, #, $...)")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }



}
