// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using Fan.EntityFrameworkCore.MultipleDbContext.Attributes;
using Fan.EntityFrameworkCore.MultipleDbContext.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fan.EntityFrameworkCore.MultipleDbContext
{
    public class DbContextBuilder
    {
        private void AddDbContext<T>(IServiceCollection services) where T : DbContext
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            string connectionString = "";
            var connectionStringName = typeof(T).GetCustomAttributeValue<ConnectionStringAttribute, string>(a => a.ConnectionStringName);
            var contextDbType = typeof(T).GetCustomAttributeValue<ConnectionStringAttribute, string>(a => a.DbType);
            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                connectionString = typeof(T).GetCustomAttributeValue<ConnectionStringAttribute, string>(a => a.ConnectionString);
            }
            else
            {
                connectionString = configuration.GetConnectionString(connectionStringName);
            }
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                string dbType;
                if (!string.IsNullOrWhiteSpace(contextDbType))
                {
                    dbType = contextDbType;
                }
                else
                    dbType = ((ConfigurationSection)configuration.GetSection("DbType")).Value;

                if (dbType.ToLower() == "sqlserver") //sqlserver
                {
                    services.AddDbContext<T>(options =>
                        options.UseSqlServer(connectionString));
                }
                else if (dbType.ToLower() == "mysql") //mysql
                {
                    services.AddDbContext<T>(options =>
                        options.UseMySql(connectionString));
                }
                else if (dbType.ToLower() == "pgsql") //pgsql
                {
                    services.AddDbContext<T>(options =>
                        options.UseNpgsql(connectionString));
                }
            }

        }
    }
}
