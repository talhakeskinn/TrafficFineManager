using trafficFineManager.Entities;
using trafficFineManager.ViewModels;

namespace trafficFineManager.Services.Abstraction
{
    public interface ITrafficFineService
    {
        Task CreateFineAsync(CreateTrafficFineViewModel model, int creatorUserId);
        Task<List<TrafficFine>> GetAllFinesAsync();
        Task ApproveFineAsync(int id, int userId);
        Task RejectFineAsync(RejectFineViewModel model, int userId);
        Task<List<TrafficFineHistory>> GetFineHistoryAsync(int trafficFineId);
        Task<List<TrafficFineHistory>> GetAllHistoryAsync();
        Task UpdateFineAsync(EditTrafficFineViewModel model, int userId);
    }
}
