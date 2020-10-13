// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using Fan.EntityFrameworkCore.MultipleDbContext.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fan.EntityFrameworkCore.MultipleDbContext.Extensions
{

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, Action<DbContextBuilder> action)
        {
            InitDbContextDir();
            var dbContextBuilder = new DbContextBuilder();
            action?.Invoke(dbContextBuilder);
            services.AddScoped<DBContextFactory>();
            return services;
        }

        private static void InitDbContextDir()
        {
            var dbContexts = Common.GetSubClass(typeof(DbContext));
            foreach (var dbContext in dbContexts)
            {
                foreach (var property in dbContext.GetProperties())
                {
                    if (property.PropertyType.Name.Equals(typeof(DbSet<>).Name))
                    {
                        Common.ContextDictionary.TryAdd(property.PropertyType.GenericTypeArguments[0], dbContext);
                    }
                }
            }
        }
    }
}
