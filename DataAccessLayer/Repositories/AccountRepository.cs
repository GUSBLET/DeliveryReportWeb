namespace DataAccessLayer.Repositories
{
    public class AccountRepository : IBaseRepository<Account>
    {
        private readonly ApplicationDbContext _dataBase;

        public AccountRepository(ApplicationDbContext context) =>  _dataBase = context;

        public async Task<bool> Add(Account entity)
        {
            try
            {
                await _dataBase.AddAsync(entity);
                await _dataBase.SaveChangesAsync();
            }
            catch { return await Task.FromResult(false); }

            return await Task.FromResult(true);
        }

        public async Task<bool> Delete(Account entity)
        {
            try
            {
                _dataBase.Remove(entity);
                await _dataBase.SaveChangesAsync();
            }
            catch { return await Task.FromResult(false); }

            return await Task.FromResult(true);
        }

        public IQueryable<Account> Select()
        {
            return _dataBase.TableUsers;
        }

        public async Task<Account> Update(Account entity)
        {
            _dataBase.TableUsers.Update(entity);
            await _dataBase.SaveChangesAsync();

            return entity;
        }
    }
}
