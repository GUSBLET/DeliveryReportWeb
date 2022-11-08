namespace BusinessLogic.Implemantations
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IBaseRepository<Account> _userRepository;

        public AccountService(ILogger<AccountService> logger, IBaseRepository<Account> userRepository) 
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                var user = await (from p in _userRepository.Select()
                                  where p.Email == model.Email
                                  select new Account
                                  {
                                      Email = p.Email,
                                      Password = p.Password,
                                      Role = p.Role,
                                  }).FirstOrDefaultAsync();

                if(user == null)
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "User not found"
                    };

                if (user.Password == HashPasswordHelper.HashPassowrd(model.Password))
                {
                    var result = Authenticate(user);
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Data = result,
                        StatusCode = HttpStatusCode.OK
                    };

                }
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Password is wrong"
                };
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Registration(RegistrationViewModel model)
        {
            try
            {
                var userExist = await (from p in _userRepository.Select()
                                where p.Email == model.Email
                                select new
                                {
                                    Email = model.Email
                                }).FirstOrDefaultAsync();

                if(userExist == null)
                {
                    if(MaskPassword(model.Password))
                    {
                        var account = new Account()
                        {
                            Email = model.Email,
                            Role = Role.User,
                            Password = HashPasswordHelper.HashPassowrd(model.Password),
                            Name = model.Name,
                            LastName = model.LastName,
                            PhoneNumber = model.PhoneNumber
                        };
                            
                        if(await _userRepository.Add(account)) 
                            return new BaseResponse<ClaimsIdentity>()
                            {
                                Description = "User Added",
                                StatusCode = HttpStatusCode.OK,
                            };

                        _logger.LogError("Error Registration", "[Register]: Account didn't add to database");
                        return new BaseResponse<ClaimsIdentity>()
                        {
                            Description = "Account didn't add",
                            StatusCode = HttpStatusCode.InternalServerError,
                        };
                    }
                    return new BaseResponse<ClaimsIdentity>()
                        {
                            Description = "Password must have numbers and latin letters",
                        };
                }
                return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "User exist", 
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Register]: {ex.Message}");
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        private ClaimsIdentity Authenticate(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private bool MaskPassword(string password)
        {
            bool onlyEn = true; // variable to check for only en key
            bool number = false; // variable to check for exist number
            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] >= 'А' && password[i] <= 'Я') onlyEn = false;
                if (password[i] >= '0' && password[i] <= '9') number = true;
            }
            if (onlyEn && number)
                return true;
            return false;
        }
    }
}
