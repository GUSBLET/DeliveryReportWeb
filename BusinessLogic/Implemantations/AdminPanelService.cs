namespace BusinessLogic.Implemantations
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly IBaseRepository<Account> _userRepository;
        private readonly ILogger<Account> _logger;
        
        public AdminPanelService(IBaseRepository<Account> userRepository, ILogger<Account> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        
        public async Task<BaseResponse<ChangeUserViewModel>> GetUserCardById(ulong id)
        {
            try
            {
                var response = await(from p in _userRepository.Select()
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
                if (response != null)
                {
                    var userCard = new ChangeUserViewModel()
                    {
                        Email = response.Email,
                        Name = response.Name,
                        LastName = response.LastName,
                        PhoneNumber = response.PhoneNumber,
                        Role = response.Role
                    };
                    return new BaseResponse<ChangeUserViewModel> { Data = userCard, StatusCode = HttpStatusCode.OK };
                }
                return new BaseResponse<ChangeUserViewModel> { StatusCode = HttpStatusCode.InternalServerError };

                    
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error: {ex.Message}");
                return new BaseResponse<ChangeUserViewModel> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<Account>> ManagerChangesRole(ChangeUserViewModel model)
        {
            try
            {
                var response = await(from p in _userRepository.Select()
                                     where p.Email == model.Email
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
                
                if(response is null)
                    return new BaseResponse<Account> { StatusCode = HttpStatusCode.InternalServerError };
                if (model.Role == Role.Admin || model.Role == Role.Manager || response.Role == Role.Admin || response.Role == Role.Manager)
                    return new BaseResponse<Account>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Description = "Access denied"
                    };

                response.Role = model.Role;
                await _userRepository.Update(response);
                return new BaseResponse<Account> { StatusCode = HttpStatusCode.OK };               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change Role: {ex.Message}");
                return new BaseResponse<Account> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<Account>> AdminChangesRole(ChangeUserViewModel model)
        {
            try
            {
                var response = await (from p in _userRepository.Select()
                                      where p.Email == model.Email
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
                if (response is null)
                    return new BaseResponse<Account> { StatusCode = HttpStatusCode.InternalServerError };

                if (model.Role == Role.Admin || response.Role == Role.Admin)
                    return new BaseResponse<Account>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Description = "Access denied"
                    };
                response.Role = model.Role;
                await _userRepository.Update(response);
                return new BaseResponse<Account> { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {

                _logger.LogError($"Change Role: {ex.Message}");
                return new BaseResponse<Account> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<List<Account>>> SelectUserList()
        {
            try
            {
                return new BaseResponse<List<Account>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = await _userRepository.Select().Where(x => x.EmailConfirmed == true).ToListAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new BaseResponse<List<Account>> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<bool>> ManagerDeletesUserById(ulong id)
        {
            try
            {
                var response = await (from p in _userRepository.Select()
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
                if (response is null)
                    return new BaseResponse<bool> { StatusCode = HttpStatusCode.InternalServerError };
                if (response.Role == Role.Admin || response.Role == Role.Manager)
                    return new BaseResponse<bool>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Description = "Access denied"
                    };
                await _userRepository.Delete(response);
                return new BaseResponse<bool> { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change Role: {ex.Message}");
                return new BaseResponse<bool> { StatusCode = HttpStatusCode.InternalServerError };
            }
            
        }

        public async Task<BaseResponse<bool>> AdminDeletesUserById(ulong id)
        {
            try
            {
                var response = await (from p in _userRepository.Select()
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
                if (response is null)
                    return new BaseResponse<bool> { StatusCode = HttpStatusCode.InternalServerError };
                if (response.Role == Role.Admin)
                    return new BaseResponse<bool>
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Description = "Access denied"
                    };
                await _userRepository.Delete(response);
                return new BaseResponse<bool> { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change Role: {ex.Message}");
                return new BaseResponse<bool> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        
    }
}
