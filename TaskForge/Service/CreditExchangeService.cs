using TaskForge.Models;
using TaskForge.Repository;
using System.Collections.Generic;

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

        // Cập nhật trạng thái trao đổi tín dụng (ví dụ: phê duyệt)
        public void ApproveCreditExchange(int exchangeId)
        {
            var creditExchange = _creditExchangeRepository.GetCreditExchangeById(exchangeId);
            if (creditExchange != null)
            {
                creditExchange.Status = "Approved";
                _creditExchangeRepository.UpdateCreditExchange(creditExchange);
            }
        }
    }
}
