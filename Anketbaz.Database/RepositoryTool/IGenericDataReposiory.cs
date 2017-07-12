using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Anketbaz.Database.RepositoryTool
{
    public interface IGenericDataRepository<T> where T : class
    {
        /// <summary>
        /// Verilen repo dan liste getirir.
        /// </summary>
        /// <param name="where">sart expression u </param>
        /// <returns></returns>
        IList<T> GetList(Expression<Func<T, bool>> @where);

        /// <summary>
        /// Verilen repo dan liste getirir.
        /// </summary>
        /// <param name="where">sart expression u </param>
        /// <returns></returns>
        IList<T> GetDataPart(Expression<Func<T, bool>> where, Expression<Func<T, dynamic>> sort,
            Expression<Func<T, dynamic>> thenby, string sortType, int takeCount);

        /// <summary>
        /// Verilen repo sayi getirir.
        /// </summary>
        /// <param name="where">sart expression u </param>
        /// <returns></returns>
      int GetDataCount(Expression<Func<T, bool>> where);

        /// <summary>
        /// Kolonlara select ceker
        /// </summary>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select);
        IList<T> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> sort, SortType sortType);
        IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select, Expression<Func<T, dynamic>> sort);
        IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select, Expression<Func<T, dynamic>> sort,int take);

        /// <summary>
        /// Verilen repo dan tek item getirir.
        /// </summary>
        /// <param name="where">sart expression u </param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> @where);

        /// <summary>
        /// Repoya item ekler
        /// </summary>
        /// <param name="items">eklenecek itemlar </param>
        /// <returns></returns>
        bool Add(params T[] items);

        /// <summary>
        /// Repodan item duzenler
        /// </summary>
        /// <param name="items">duzenlenecek itemlar </param>
        /// <returns></returns>
        bool Update(params T[] items);

        /// <summary>
        /// Repodan item siler
        /// </summary>
        /// <param name="items">silincek itemlar </param>
        /// <returns></returns>
        bool Remove(params T[] items);
    }

    public enum SortType
    {
        Ascending,
        Descending
    }
}
