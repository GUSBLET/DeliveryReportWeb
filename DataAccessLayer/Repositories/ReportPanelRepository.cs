namespace DataAccessLayer.Repositories
{
    public class ReportPanelRepository : IBaseRepository<ReportOfDelivary>
    {
        private readonly ApplicationDbContext _dataBase;

        public ReportPanelRepository(ApplicationDbContext dataBase)
        {
            _dataBase = dataBase;
        }

        public async Task<bool> Add(ReportOfDelivary entity)
        {
            try
            {
                await _dataBase.AddAsync(entity);
                await _dataBase.SaveChangesAsync();
            }
            catch { return await Task.FromResult(false); }

            return await Task.FromResult(true);
        }

        public async Task<bool> Delete(ReportOfDelivary entity)
        {
            try
            {
                _dataBase.Remove(entity);
                await _dataBase.SaveChangesAsync();
            }
            catch { return await Task.FromResult(false); }

            return await Task.FromResult(true);
        }

        public IQueryable<ReportOfDelivary> Select()
        {
            return _dataBase.TableReports;
        }

        public async Task<ReportOfDelivary> Update(ReportOfDelivary entity)
        {
            _dataBase.TableReports.Update(entity);
            await _dataBase.SaveChangesAsync();

            return entity;
        }
    }
}
