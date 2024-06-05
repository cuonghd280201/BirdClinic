using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
	public class GenericRep<C, T> : IGenericRep<T> where T : class where C : DbContext, new()
	{
		public C Context { get; set; }

		public GenericRep()
		{
			Context = new C();
		}

		public IQueryable<T> All
		{
			get
			{
				return Context.Set<T>();
			}
		}

		public void Create(T m)
		{
			using (var tran = Context.Database.BeginTransaction())
			{
				try
				{
					var result = Context.Set<T>().Add(m).Entity;
					Context.SaveChanges();
					tran.Commit();
				}
				catch (Exception ex)
				{
					tran.Rollback();
				}
			}
		}

		public void Update(T m)
		{
			using (var tran = Context.Database.BeginTransaction())
			{
				try
				{
					var result = Context.Set<T>().Update(m).Entity;
					Context.SaveChanges();
					tran.Commit();
				}
				catch (Exception ex)
				{
					tran.Rollback();
				}
			}

		}

		public void Delete(T m)
		{
			using (var tran = Context.Database.BeginTransaction())
			{
				try
				{
					Context.Set<T>().Remove(m);
					Context.SaveChanges();
					tran.Commit();
				}
				catch (Exception ex)
				{
					tran.Rollback();
				}
			}
		}
	}
}
