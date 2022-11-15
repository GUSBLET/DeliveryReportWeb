namespace Models.ViewModels
{
    public class AccountProfileViewModels
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "FieldEmailErrorRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailLabel")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FieldPasswordErrorRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabel")]
        public string Password { get; set; }
    }

    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "FieldEmailErrorRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailLabel")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FieldPasswordErrorRequired")]
        [MinLength(8, ErrorMessage = "PasswordMinLengthError")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabel")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldPasswordConfirmErrorRequired")]
        [Compare("Password", ErrorMessage = "ErrorPasswordCompare")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirmLabel")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "FieldNameErrorRequired")]
        [DataType(DataType.Text)]
        [Display(Name = "NameLabel")]
        public string Name { get; set; }

        [Required(ErrorMessage = "FieldLastNameErrorRequired")]
        [DataType(DataType.Text)]
        [Display(Name = "LastNameLabel")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "FieldPhoneNumberErrorRequired")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "PhoneNumberLabel")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordRequistViewModel
    {
        [Required(ErrorMessage = "FieldEmailErrorRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailLabel")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public ulong Id { get; set; }

        [Required(ErrorMessage = "FieldPasswordErrorRequired")]
        [MinLength(8, ErrorMessage = "PasswordMinLengthError")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabel")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldPasswordConfirmErrorRequired")]
        [Compare("Password", ErrorMessage = "ErrorPasswordCompare")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirmLabel")]
        public string PasswordConfirm { get; set; }
    }
}
