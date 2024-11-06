using TaskForge.Models;
using TaskForge.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TaskForge.Repository
{
    public class CreditExchangeRepository
    {
        private readonly TaskForgeContext _context;

        public CreditExchangeRepository(TaskForgeContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả các trao đổi tín dụng, sắp xếp theo ngày mới nhất giảm dần
        public List<CreditExchange> GetAllCreditExchanges()
        {
            return _context.CreditExchanges
                           .FromSqlRaw("SELECT * FROM CreditExchange ORDER BY exchange_date DESC")
                           .ToList();
        }

        // Lấy thông tin chi tiết trao đổi tín dụng dựa trên ExchangeId
        public CreditExchange GetCreditExchangeById(int exchangeId)
        {
            return _context.CreditExchanges
                           .FromSqlRaw("SELECT * FROM CreditExchange WHERE exchange_id = {0}", exchangeId)
                           .FirstOrDefault();
        }

        // Cập nhật thông tin trao đổi tín dụng
        public void UpdateCreditExchange(CreditExchange creditExchange)
        {
            _context.Database.ExecuteSqlRaw(
                "UPDATE CreditExchange SET account_id = {0}, exchange_date = {1}, credit_points_used = {2}, cash_amount = {3}, status = {4} WHERE exchange_id = {5}",
                creditExchange.AccountId, creditExchange.ExchangeDate, creditExchange.CreditPointsUsed, creditExchange.CashAmount, creditExchange.Status, creditExchange.ExchangeId
            );
        }

        // Tạo một trao đổi tín dụng mới
        public void CreateCreditExchange(CreditExchange creditExchange)
        {
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO CreditExchange (account_id, exchange_date, credit_points_used, cash_amount, status) VALUES ({0}, {1}, {2}, {3}, {4})",
                creditExchange.AccountId, creditExchange.ExchangeDate, creditExchange.CreditPointsUsed, creditExchange.CashAmount, creditExchange.Status
            );
        }
    }
}