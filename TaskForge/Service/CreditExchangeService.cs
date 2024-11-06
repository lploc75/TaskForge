using TaskForge.Models;
using TaskForge.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Service
{
    public class CreditExchangeService
    {
        private readonly CreditExchangeRepository _creditExchangeRepository;

        public CreditExchangeService(CreditExchangeRepository creditExchangeRepository)
        {
            _creditExchangeRepository = creditExchangeRepository;
        }

        // Lấy tất cả các trao đổi tín dụng
        public List<CreditExchange> GetAllCreditExchanges()
        {
            return _creditExchangeRepository.GetAllCreditExchanges();
        }

        // Lấy thông tin chi tiết trao đổi tín dụng
        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _creditExchangeRepository.GetCreditExchangeById(exchangeId);
        }

        // Tạo một trao đổi tín dụng mới
        public void CreateCreditExchange(CreditExchange creditExchange)
        {
            _creditExchangeRepository.CreateCreditExchange(creditExchange);
        }

        // Cập nhật trạng thái trao đổi tín dụng (phê duyệt, từ chối, hoặc trạng thái khác)
        public void UpdateCreditExchangeStatus(int exchangeId, string status)
        {
            var creditExchange = _creditExchangeRepository.GetCreditExchangeById(exchangeId);
            if (creditExchange != null)
            {
                creditExchange.Status = status;
                _creditExchangeRepository.UpdateCreditExchange(creditExchange);
            }
        }

        // Lọc các CreditExchange dựa trên tiêu chí tìm kiếm
        public List<CreditExchange> FilterCreditExchanges(string accountId, string status, int? minCredits, int? maxCredits, decimal? minCash, decimal? maxCash, DateTime? startDate, DateTime? endDate)
        {
            var exchanges = _creditExchangeRepository.GetAllCreditExchanges();

            // Áp dụng các bộ lọc nếu có
            if (!string.IsNullOrEmpty(accountId))
            {
                exchanges = exchanges.Where(e => e.AccountId.Contains(accountId)).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                exchanges = exchanges.Where(e => e.Status == status).ToList();
            }
            if (minCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed >= minCredits).ToList();
            }
            if (maxCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed <= maxCredits).ToList();
            }
            if (minCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount >= minCash).ToList();
            }
            if (maxCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount <= maxCash).ToList();
            }
            if (startDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate <= endDate).ToList();
            }

            return exchanges;
        }
        public List<CreditExchange> FilterCreditExchanges2(string accountId, string status, int? minCredits, int? maxCredits, decimal? minCash, decimal? maxCash, DateTime? startDate, DateTime? endDate)
        {
            var exchanges = _creditExchangeRepository.GetAllCreditExchanges();

            // Bắt đầu lọc với accountId
            exchanges = exchanges.Where(e => e.AccountId == accountId).ToList();

            // Các điều kiện lọc bổ sung khác
            if (!string.IsNullOrEmpty(status))
            {
                exchanges = exchanges.Where(e => e.Status == status).ToList();
            }
            if (minCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed >= minCredits).ToList();
            }
            if (maxCredits.HasValue)
            {
                exchanges = exchanges.Where(e => e.CreditPointsUsed <= maxCredits).ToList();
            }
            if (minCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount >= minCash).ToList();
            }
            if (maxCash.HasValue)
            {
                exchanges = exchanges.Where(e => e.CashAmount <= maxCash).ToList();
            }
            if (startDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate >= startDate).ToList();
            }
            if (endDate.HasValue)
            {
                exchanges = exchanges.Where(e => e.ExchangeDate <= endDate).ToList();
            }

            return exchanges;
        }
    }
}
