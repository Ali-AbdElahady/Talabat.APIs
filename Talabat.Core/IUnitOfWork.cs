using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Core
{
	// get dispose function by inherat from IAsyncDisposable
	public interface IUnitOfWork : IAsyncDisposable
	{
		// any constrain on T in generic repo must be on the T here
		IGenericReppository<TEntity> Reppository<TEntity>() where TEntity : BaseEntity;
		Task<int> CompleteAsync();
	}
}
