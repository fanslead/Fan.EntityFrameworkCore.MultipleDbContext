// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
