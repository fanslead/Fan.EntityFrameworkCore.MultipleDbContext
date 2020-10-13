// ***********************************************************************
// Assembly         : Fan.EntityFrameworkCore.MultipleDbContext
// Author           : LiuJunFan
// Created          : 2020-10-13
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fan.EntityFrameworkCore.MultipleDbContext
{
    public static class Common
    {
        public static readonly IDictionary<Type, Type> ContextDictionary = new Dictionary<Type, Type>();

        public static IList<Type> GetSubClass(Type baseType)
        {
            var subTypeQuery = from t in Assembly.GetCallingAssembly().GetTypes()
                               where baseType.Equals(t.BaseType)
                               select t;
            return subTypeQuery.ToList();
        }
    }
}
