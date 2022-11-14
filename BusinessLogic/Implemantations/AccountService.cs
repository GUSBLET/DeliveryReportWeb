using System;

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

        public async Task<BaseResponse<bool>> ConfirmEmailAsync(ulong id, string key)
        {
            var user = GetUserById(id).Result.Data;
            if(user == null) 
            { 
                return new BaseResponse<bool>
                {
                    Data = false
                };
            }
            if(id == user.Id && key == user.EmailConfirmedToken.ToString())
            {
                user.EmailConfirmed = true;
                _userRepository.Update(user);
                return new BaseResponse<bool>
                {
                    Data = true
                };
            }
            return new BaseResponse<bool>
            {
                Data = false
            };

        }

        public async Task<BaseResponse<Account>> GetUserByEmail(string email)
        {
            var result = await (from p in _userRepository.Select()
                                where p.Email == email
                                select new Account
                                {
                                    Id = p.Id,
                                    Email = p.Email,
                                    Password = p.Password,
                                    Name = p.Name,
                                    LastName = p.LastName,
                                    PhoneNumber = p.PhoneNumber,
                                    Role = p.Role,
                                    EmailConfirmed = p.EmailConfirmed,
                                    EmailConfirmedToken = p.EmailConfirmedToken
                                }).FirstOrDefaultAsync();
            return new BaseResponse<Account>
            {
                Data = result
            };            
        }

        public async Task<BaseResponse<Account>> GetUserById(ulong id)
        {
            var result = await(from p in _userRepository.Select()
                               where p.Id == id
                               select new Account
                               {
                                   Id = p.Id,
                                   Email = p.Email,
                                   Password = p.Password,
                                   Name = p.Name,
                                   LastName = p.LastName,
                                   PhoneNumber = p.PhoneNumber,
                                   Role = p.Role,
                                   EmailConfirmed = p.EmailConfirmed,
                                   EmailConfirmedToken = p.EmailConfirmedToken
                               }).FirstOrDefaultAsync();
            return new BaseResponse<Account>
            {
                Data = result
            };
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
                if (!user.EmailConfirmed)
                {
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
                else
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Email didn't confirm"
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
                            PhoneNumber = model.PhoneNumber,
                            EmailConfirmedToken = Guid.NewGuid(),
                            EmailConfirmed = false
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

        public async Task<BaseResponse<bool>> ResetPassword(ulong id , string password)
        {
            try
            {
                var user = GetUserById(id).Result.Data;
                if (user == null)
                {
                    return new BaseResponse<bool>
                    {
                        Data = false,
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                if (user.Password == password)
                    return new BaseResponse<bool>
                    {
                        Data = false,
                        Description = "New password is very similar",
                        StatusCode = HttpStatusCode.InternalServerError

                    };
                if(MaskPassword(password))
                {
                    if (id == user.Id)
                    {

                        user.Password = HashPasswordHelper.HashPassowrd(password);
                        _userRepository.Update(user);
                        return new BaseResponse<bool>
                        {
                            Data = true,
                            StatusCode = HttpStatusCode.OK,
                            Description = "Password must have numbers and latin letters"
                        };
                    }
                }
                return new BaseResponse<bool>
                {
                    Data = false,
                    Description = "User not found",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login]: {ex.Message}");
                return new BaseResponse<bool>
                {
                    Data = false,
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
