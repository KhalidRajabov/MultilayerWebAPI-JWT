using AuthServer.Core.Repository;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDTO> : IServiceGeneric<TEntity, TDTO> where TEntity : class where TDTO : class
    {
        private readonly IUnitOfWork? _unitOfWork;
        private readonly IGenericRepository<TEntity>? _genericRepository;

        public ServiceGeneric(IUnitOfWork? unitOfWork, IGenericRepository<TEntity>? genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDTO>> AddAsync(TDTO dto)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDTO>(newEntity);
            return Response<TDTO>.Success(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDTO>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDTO>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDTO>>.Success(products, 200);
        }

        public async Task<Response<TDTO>> GetByIdAsync(int id)
        {
            var products = await _genericRepository.GetByIdAsync(id);
            if (products==null)
            {
                return Response<TDTO>.Fail("Id not found", 404, true);
            }

            return Response<TDTO>.Success(ObjectMapper.Mapper.Map<TDTO>(products), 200);
        }

        public async Task<Response<NoDataDTO>> Remove(int id)
        {
            var isExist = await _genericRepository.GetByIdAsync(id);
            if (isExist==null)
            {
                return Response<NoDataDTO>.Fail("Id not found", 404, true);
            }
            _genericRepository.Remove(isExist);
            await _unitOfWork.CommitAsync();


            //204 stands for "No content", means body of response will not have data
            return Response<NoDataDTO>.Success(204);
        }

        public async Task<Response<NoDataDTO>> Update(TDTO dto, int id)
        {
            var isExist = await _genericRepository.GetByIdAsync(id);
            if (isExist == null)
            {
                return Response<NoDataDTO>.Fail("Id not found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(dto);
            _genericRepository.Update(updateEntity);
            await _unitOfWork.CommitAsync();

            //204 stands for "No content", means body of response will not have data
            return Response<NoDataDTO>.Success(200);
        }

        public async Task<Response<IEnumerable<TDTO>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            //for example: where(x=>x.id>5)
            var list = _genericRepository.Where(predicate);
            return Response<IEnumerable<TDTO>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDTO>>(await list.ToListAsync()), 200);
        }
    }
}