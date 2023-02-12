using SharedLibrary.DTOs;
using System.Linq.Expressions;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity, TDTO> where TEntity : class where TDTO : class
    {
        Task<Response<TDTO>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDTO>>> GetAllAsync();
        Task<Response<IEnumerable<TDTO>>> Where(Expression<Func<TEntity, bool>> predicate);
        Task<Response<TDTO>> AddAsync(TDTO dto);
        Task<Response<NoDataDTO>> Remove(int id);
        Task<Response<NoDataDTO>> Update(TDTO dto,int id);
    }
}