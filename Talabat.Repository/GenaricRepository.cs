using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericReppository<T> where T : BaseEntity
	{
		private readonly StoreContext dbContext;

		public GenericRepository(StoreContext dbContext)
        {
			this.dbContext = dbContext;
		}
        public async Task<IReadOnlyList<T>> GetAllAsync()
		{
			if(typeof(T) == typeof(Product))
			{
				return (IReadOnlyList<T>) await dbContext.Products.Include(p=>p.ProductBrand).Include(p=>p.ProductType).ToListAsync();

			}
			return await dbContext.Set<T>().ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id)
		{
			if (typeof(T) == typeof(Product))
			{
				return (T)(object)await dbContext.Products
					.Include(p => p.ProductBrand)
					.Include(p => p.ProductType)
					.FirstOrDefaultAsync(p=>p.Id == id);

			}
			return await dbContext.Set<T>().FindAsync(id) ;
		}

		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(Ispecification<T> spec)
		{
			var hamo = await SpeificationEvalutor<T>.GetQuery(dbContext.Set<T>(),spec).ToListAsync();
			return hamo;
		}

		public async Task<T> GetEntityWithSpecAsync(Ispecification<T> spec)
		{
			return await SpeificationEvalutor<T>.GetQuery(dbContext.Set<T>(), spec).FirstOrDefaultAsync();
		}
		private IQueryable<T> ApplySpecification(Ispecification<T> spec)
		{
			return SpeificationEvalutor<T>.GetQuery(dbContext.Set<T>(), spec);
		}

		public async Task<int> GetCountWithSpecAsync(Ispecification<T> spec)
		{
			return await ApplySpecification(spec).CountAsync();
		}

		public async Task AddAsync(T item) => await dbContext.Set<T>().AddAsync(item);

		public void Delete(T item)
		{
			dbContext.Set<T>().Remove(item);
		}

		public void update(T itemAmer)
		{
			dbContext.Set<T>().Update(itemAmer);
		}
	}
}
