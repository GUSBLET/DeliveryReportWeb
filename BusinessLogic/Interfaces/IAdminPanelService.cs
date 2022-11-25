namespace BusinessLogic.Interfaces
{
    public interface IAdminPanelService
    {
        Task<BaseResponse<List<Account>>> SelectUserList();
        Task<BaseResponse<Account>> AdminChangesRole(ChangeUserViewModel model);
        Task<BaseResponse<Account>> ManagerChangesRole(ChangeUserViewModel model);
        Task<BaseResponse<ChangeUserViewModel>> GetUserCardById(ulong id);
        Task<BaseResponse<bool>> AdminDeletesUserById(ulong id);
        Task<BaseResponse<bool>> ManagerDeletesUserById(ulong id);
    }
}
