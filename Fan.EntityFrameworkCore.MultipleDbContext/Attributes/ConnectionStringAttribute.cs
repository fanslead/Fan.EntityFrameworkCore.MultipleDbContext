// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Fan.EntityFrameworkCore.MultipleDbContext.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConnectionStringAttribute : Attribute
    {
        public ConnectionStringAttribute(string ConnectionStringName)
        {
            this.ConnectionStringName = ConnectionStringName;
        }

        public string ConnectionStringName { get; set; }
        public string ConnectionString { get; set; }

        public string DbType { get; set; }
    }
}
