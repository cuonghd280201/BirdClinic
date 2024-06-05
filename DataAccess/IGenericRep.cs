namespace DataAccess
{
	public interface IGenericRep<T> where T : class
	{
		void Create(T m);

		void Update(T m);

		void Delete(T m);

		IQueryable<T> All { get; }
	}
}
