using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;
using Human.Chrs.Domain.IRepository.Base;
using Human.Repository.EF;

namespace Human.Repository.Repository.Base
{
    public abstract class BaseRepository<TEntity, TDTO, TIdentity> : IRepository<TDTO, TIdentity>
        where TEntity : class
        where TDTO : IDTO
    {
        protected readonly IMapper _mapper;
        protected HumanChrsContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(IMapper mapper, HumanChrsContext context)
        {
            _mapper = mapper;
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        protected void SetDbContext(HumanChrsContext context)
        {
            _context = context;
        }

        public virtual async Task DeleteAsync(TIdentity id)
        {
            var data = await _dbSet.FindAsync(id);
            if (data != null)
            {
                _dbSet.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<TDTO> GetAsync(TIdentity id)
        {
            var entity = await _dbSet.FindAsync(id);

            return _mapper.Map<TDTO>(entity);
        }

        public async Task<IEnumerable<TDTO>> GetListAsync()
        {
            var data = await _dbSet.ToListAsync();

            return data.Select(_mapper.Map<TDTO>);
        }

        public virtual async Task<TDTO> InsertAsync(TDTO dto)
        {
            var entity = _mapper.Map<TEntity>(dto);

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            dto = _mapper.Map<TDTO>(entity);

            return dto;
        }

        public virtual async Task<TDTO> UpdateAsync(TDTO dto)
        {
            var existingEntity = await _dbSet.FindAsync(dto.Id);
            if (existingEntity == null)
            {
                throw new ArgumentException("Entity with the given ID does not exist");
            }

            _mapper.Map(dto, existingEntity);

            _dbSet.Update(existingEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TDTO>(existingEntity);
        }

        public virtual async Task<IEnumerable<TDTO>> UpdateAsync(IEnumerable<TDTO> dtos)
        {
            List<TDTO> updatedDtos = new List<TDTO>();

            foreach (var dto in dtos)
            {
                var existingEntity = await _dbSet.FindAsync(dto.Id);
                if (existingEntity == null)
                {
                    throw new ArgumentException($"Entity with ID {dto.Id} does not exist");
                }

                _mapper.Map(dto, existingEntity);
                _dbSet.Update(existingEntity);

                updatedDtos.Add(_mapper.Map<TDTO>(existingEntity));
            }

            await _context.SaveChangesAsync();

            return updatedDtos;
        }
    }
}