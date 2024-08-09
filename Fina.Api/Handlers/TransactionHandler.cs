using Fina.Api.Data;
using Fina.Core.Common;
using Fina.Core.Enums;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Transactions;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers
{
    public class TransactionHandler(AppDbContext db) : ITransactionHandler
    {
        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            try
            {
                if (request is { Type: ETransactionType.withdraw, Amount: > 0 })
                    request.Amount *= -1;

                var transaction = new Transaction
                {
                    UserId = request.UserId,
                    CategoryId = request.categoryId,
                    CreatedAt = DateTime.UtcNow,
                    Amount = request.Amount,
                    PaidOrReceivedAt = request.PaidOrReceiveAt,
                    Title = request.Title,
                    Type = request.Type

                };
                await db.Transaction.AddAsync(transaction);
                await db.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, message: "Transação criada com sucesso!");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possivel criar a transação");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = await db
                    .Transaction
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
                if (transaction == null)
                    return new Response<Transaction?>(null, 404, "Transação não encontrada!");


                db.Transaction.Remove(transaction);
                await db.SaveChangesAsync();

                return new Response<Transaction?>(transaction);

            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possivel remover a transação");

            }
        }
        public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        {
            try
            {
                var transaction = await db
                    .Transaction
                    .FirstOrDefaultAsync(x => x.Id == x.Id && x.UserId == request.UserId);

                return transaction is null
                    ? new Response<Transaction?>(null, message: "Transação não encontrada")
                    : new Response<Transaction?>(transaction, message: "Transação encontrada!");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possivel encontrar a transação");

            }
        }

        public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
        {
            try
            {
                request.StartDate ??= DateTime.Now.GetFirstDay();
                request.EndDate ??= DateTime.Now.GetLastDay();


            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possivel determinar a data");
            }

            try
            {
                var query = db
                 .Transaction
                 .AsNoTracking()
                 .Where(x => 
                     x.PaidOrReceivedAt >= request.StartDate &&
                     x.PaidOrReceivedAt <= request.EndDate &&
                     x.UserId == request.UserId)
                    .OrderBy(x => x.PaidOrReceivedAt);
                    
                var transactions = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var count = await query.CountAsync();

                return new PagedResponse<List<Transaction>?>(
                    transactions,
                    count,
                    request.PageNumber,
                    request.PageSize
                    );
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Houve um problema ao listar as transações");
            }
        }

        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
        {
            if (request is { Type: ETransactionType.withdraw, Amount: > 0 })
                request.Amount *= -1;

            try
            {
                var transaction = await db
                    .Transaction
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
                if (transaction == null)
                    return new Response<Transaction?>(null, 404, "Transação não encontrada!");

                transaction.CategoryId = request.categoryId;
                transaction.Amount = request.Amount;
                transaction.Title = request.Title;
                transaction.Type = request.Type;
                transaction.PaidOrReceivedAt = request.PaidOrReceiveAt;

                db.Transaction.Update(transaction);
                await db.SaveChangesAsync();

                return new Response<Transaction?>(transaction);

            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possivel atualizar a transação");

            }
        }
    }
}
