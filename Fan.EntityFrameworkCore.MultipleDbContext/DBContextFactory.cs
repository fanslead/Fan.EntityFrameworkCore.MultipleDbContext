// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fan.EntityFrameworkCore.MultipleDbContext
{
    public class DBContextFactory
    {
        private readonly IDictionary<Type, Type> ContextTypes = new Dictionary<Type, Type>();

        private readonly IDictionary<Type, DbContext> DbContexts = new Dictionary<Type, DbContext>();

        private readonly IServiceProvider _serviceProvider;

        public DBContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// 获取对应实体类所在的DbContext
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <returns></returns>
        public DbContext GetDbContext<T>() where T : class
        {
            if (ContextTypes.TryGetValue(typeof(T), out Type contextType))
            {
                if (DbContexts.TryGetValue(contextType, out DbContext context))
                    return context;
            }
            else
            {
                if (Common.ContextDictionary.TryGetValue(typeof(T), out Type ctType))
                {
                    if (DbContexts.TryGetValue(ctType, out DbContext ct))
                        return ct;
                    ct = (DbContext)_serviceProvider.CreateScope().ServiceProvider.GetRequiredService(ctType);
                    ContextTypes.TryAdd(typeof(T), ctType);
                    DbContexts.TryAdd(ctType, ct);
                    return ct;
                }
            }
            throw new Exception("当前实体未包含在任一DbCotext.");
        }
        /// <summary>
        /// 根据DbContext类型获取对应DbContext
        /// </summary>
        /// <param name="contextType">DbContext类型</param>
        /// <returns></returns>
        public DbContext GetDbContext(Type contextType)
        {
            if (DbContexts.TryGetValue(contextType, out DbContext context))
                return context;
            else
            {
                var ct = (DbContext)_serviceProvider.CreateScope().ServiceProvider.GetRequiredService(contextType);
                DbContexts.TryAdd(contextType, ct);
                return ct;
            }
        }

        public void Save()
        {
            foreach (var context in DbContexts)
            {
                var _context = context.Value;
                try
                {
                    var entities = _context.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added
                                    || e.State == EntityState.Modified)
                        .Select(e => e.Entity);

                    foreach (var entity in entities)
                    {
                        var validationContext = new ValidationContext(entity);
                        Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
                    }

                    _context.SaveChanges();
                }
                catch (ValidationException exc)
                {
                    Console.WriteLine($"{nameof(Save)} validation exception: {exc?.Message}");
                    throw (exc.InnerException as Exception ?? exc);
                }
                catch (Exception ex) //DbUpdateException 
                {
                    throw (ex.InnerException as Exception ?? ex);
                }
            }
        }
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            foreach (var context in DbContexts)
            {
                var _context = context.Value;
                try
                {
                    var entities = _context.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added
                                    || e.State == EntityState.Modified)
                        .Select(e => e.Entity);

                    foreach (var entity in entities)
                    {
                        var validationContext = new ValidationContext(entity);
                        Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (ValidationException exc)
                {
                    Console.WriteLine($"{nameof(SaveAsync)} validation exception: {exc?.Message}");
                    throw (exc.InnerException as Exception ?? exc);
                }
                catch (Exception ex) //DbUpdateException 
                {
                    throw (ex.InnerException as Exception ?? ex);
                }
            }
        }
    }
}
