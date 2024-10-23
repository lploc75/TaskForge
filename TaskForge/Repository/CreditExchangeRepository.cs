using TaskForge.Models;
using TaskForge.DBContext;
using System.Collections.Generic;
using System.Linq;

namespace TaskForge.Repository
{
    public class CreditExchangeRepository
    {
        private readonly TaskForgeContext _context;

        public CreditExchangeRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả các trao đổi tín dụng
        public List<CreditExchange> GetAllCreditExchanges()
        {
            return _context.CreditExchanges.ToList();
        }

        // Lấy thông tin chi tiết trao đổi tín dụng dựa trên ExchangeId
        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _context.CreditExchanges.FirstOrDefault(e => e.ExchangeId == exchangeId);
        }

        // Cập nhật thông tin trao đổi tín dụng
        public void UpdateCreditExchange(CreditExchange creditExchange)
        {
            _context.CreditExchanges.Update(creditExchange);
            _context.SaveChanges();
        }

        // Tạo một trao đổi tín dụng mới
        public void CreateCreditExchange(CreditExchange creditExchange)
        {
            _context.CreditExchanges.Add(creditExchange);
            _context.SaveChanges();
        }
    }
}
