using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StoreContext dbContext;
		private Hashtable _repositorys;

		public UnitOfWork(StoreContext dbContext)
        {
			this.dbContext = dbContext;
			_repositorys = new Hashtable();
		}
		public async Task<int> CompleteAsync() { return await dbContext.SaveChangesAsync(); }
		

		public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
		// i want to store every object create by this function and before i create it check if already exist
		// to do that we need to key value pair of Collection type
		// Dictionary => allow you to specify the key and value but needs alot of casting
		// HashTable => the key and value is object
		public IGenericReppository<TEntity> Reppository<TEntity>() where TEntity : BaseEntity
		{
			var type = typeof(TEntity).Name;
			if (!_repositorys.ContainsKey(type))
			{
				var Repository = new GenericRepository<TEntity>(dbContext);
				_repositorys.Add(type, Repository);
			}
			return _repositorys[type] as IGenericReppository<TEntity>;
		}
	}
}
