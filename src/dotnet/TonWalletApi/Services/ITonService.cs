using TonWalletApi.Dtos;

namespace TonWalletApi.Services
{
    public interface ITonService
    {
        Task<List<Balance>> GetJettonsAsync(int userId);
        Task<List<TransactionHistory>> GetTonHistoryAsync(int userId);
        Task<List<TransactionHistory>> GetJettonHistoryAsync(int userId, string jettonAddress);
        Task<List<Dtos.Point>> GetJettonChartAsync(string jettonAddress, long startDate);
    }
}