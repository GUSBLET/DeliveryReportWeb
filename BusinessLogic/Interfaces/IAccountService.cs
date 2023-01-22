namespace BusinessLogic.Interfaces;

public interface IAccountService
{


    Task<bool> ChangeEmail(string email, ulong id);
    Task<BaseResponse<ClaimsIdentity>> Registration(RegistrationViewModel model);
    Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
    Task<BaseResponse<Account>> GetUserByEmail(string email);
    Task<BaseResponse<Account>> GetUserById(ulong id);
    Task<BaseResponse<Account>> UpdateUser(ChangeUserViewModel account);
    Task<BaseResponse<bool>> ResetPassword(ulong id, string password);
    Task<BaseResponse<bool>> ConfirmEmailAsync(ulong id, string key);
    Task<BaseResponse<List<Account>>> SelectUserList();
}
