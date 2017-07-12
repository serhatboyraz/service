using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;

namespace Anketbaz.Database.RepositoryTool
{
    public class GenericDataRepository<T> : IGenericDataRepository<T> where T : class
    {
        private static readonly PropertyInfo CrtimProperty = typeof(T).GetProperty("crtim"); //Tablo Alanlari
        private static readonly PropertyInfo CrdatProperty = typeof(T).GetProperty("crdat"); //Tablo Alanlari

        public IList<T> GetList(Expression<Func<T, bool>> @where)
        {
            List<T> list;
            using (var context = new anketbazEntities())
            {
                context.Database.CreateIfNotExists();
                IQueryable<T> dbQuery = context.Set<T>();

                list = dbQuery
                    .AsNoTracking()
                    .Where(where)
                    .ToList<T>();
            }
            return list;
        }

        public IList<T> GetDataPart(Expression<Func<T, bool>> where, Expression<Func<T, dynamic>> sort, Expression<Func<T, dynamic>> thenby, string sortType, int takeCount)
        {
            List<T> list = new List<T>();
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();
                if (sortType == "D")
                {
                    list = dbQuery
                        .AsNoTracking()
                        .OrderByDescending(sort)
                        .ThenByDescending(thenby)
                        .Where(where)
                        .Take(takeCount)
                        .ToList<T>();
                }
                else
                {
                    list = dbQuery
                        .AsNoTracking()
                        .OrderBy(sort)
                        .ThenBy(thenby)
                        .Where(where)
                        .Take(takeCount)
                        .ToList<T>();
                }
                list = list.ToList();
            }
            return list;
        }

        public int GetDataCount(Expression<Func<T, bool>> @where)
        {
            int result = 0;
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                result = dbQuery
                    .AsNoTracking()
                    .Count(@where);
            }
            return result;
        }

        public IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select)
        {
            List<dynamic> list = new List<dynamic>();
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();
                list = dbQuery
                    .AsNoTracking()
                    .Where(where)
                    .Select(select)
                    .ToList();
            }
            return list;
        }

        public IList<T> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> sort, SortType sortType)
        {
            List<T> list = new List<T>();
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();
                if (sortType == SortType.Descending)
                {
                    list = dbQuery
                        .AsNoTracking()
                        .OrderByDescending(sort)
                        .Where(where)
                        .ToList<T>();
                }
                else
                {
                    list = dbQuery
                        .AsNoTracking()
                        .OrderBy(sort)
                        .Where(where)
                        .ToList<T>();
                }
                list = list.ToList();
            }
            return list;
        }

        public IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select, Expression<Func<T, dynamic>> sort)
        {
            List<dynamic> list = new List<dynamic>();
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();
                list = dbQuery
                    .AsNoTracking()
                    .OrderByDescending(sort)
                    .Where(where)
                    .Select(select)
                    .ToList();
            }
            return list;
        }
        public IList<dynamic> GetDataPart(Expression<Func<T, bool>> @where, Expression<Func<T, dynamic>> @select, Expression<Func<T, dynamic>> sort, int take)
        {
            if (take == 0)
                take = 20;
            List<dynamic> list = new List<dynamic>();
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();
                list = dbQuery
                    .AsNoTracking()
                    .OrderByDescending(sort)
                    .Where(where)
                    .Select(select)
                    .Take(take)
                    .ToList();
            }
            return list;
        }

        public T GetSingle(Expression<Func<T, bool>> @where)
        {
            T item = null;
            using (var context = new anketbazEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                item = dbQuery
                    .AsNoTracking()
                    .FirstOrDefault(where);
            }
            return item;
        }

        public bool Add(params T[] items)
        {
            try
            {
                using (var context = new anketbazEntities())
                {
                    foreach (T item in items)
                    {
                        if (CrtimProperty != null)
                        {
                            CrtimProperty.SetValue(item, DateTime.Now.ToStringHhmmss(), null);
                        }
                        if (CrdatProperty != null)
                        {
                            CrdatProperty.SetValue(item, DateTime.Now.ToStringYyyyMMdd(), null);
                        }

                        context.Entry(item).State = System.Data.EntityState.Added;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessage = string.Empty;
                foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                {

                    if (!dbEntityValidationResult.IsValid)
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                        {
                            errorMessage += string.Format("{0} : {1} \n", dbValidationError.ErrorMessage,
                                dbValidationError.PropertyName);
                        }
                }
                Log.Log.Error(ex.Message + errorMessage);
                return false;
            }

        }

        public bool Update(params T[] items)
        {
            try
            {
                using (var context = new anketbazEntities())
                {
                    foreach (T item in items)
                    {
                        context.Entry(item).State = System.Data.EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Log.Error(ex.Message);
                return false;
            }
        }

        public bool Remove(params T[] items)
        {
            try
            {
                using (var context = new anketbazEntities())
                {
                    foreach (T item in items)
                    {
                        context.Entry(item).State = System.Data.EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Log.Error(ex.Message);
                return false;
            }
        }
    }
}
