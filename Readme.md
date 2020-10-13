# Fan.EntityFrameworkCore.MultipleDbContext
1. DbContext配置连接字符串特性头部自动注册
2. EFCore多个DbContext动态获取调用

### 配置appsetting.json默认数据库类型
```
{
    "ConnectionStrings": {
        "XXXDBContext": "Data Source=.;Initial Catalog=Db;User=sa;Password=xxxx",
    },
    "DbType": "sqlserver" // mysql pgsql
}
```
### 设置DbContext连接字符串头部
```
    [ConnectionString("XXXDBContext", DbType = "MySql")]
    public partial class XXXDBContext : DbContext
```
### Startup.cs中注入AddDbContexts
```
    services.AddDbContexts(builder=>{
        builder.AddDbContext<XXXDbContext1>(services);
        builder.AddDbContext<XXXDbContext2>(services);
    });
```
### 仓储中注入DBContextFactory
```
    private DBContextFactory _factory;
    public Repository(DBContextFactory factory)
    {
        _factory = factory;
    }
```
### 操作数据库
```
    // T是实体类型
    GetDbContext<T>().Set<T>().XXX();
    await GetDbContext<T>().Set<T>().XXXAsync();

    // 执行SQL contextType是DbContext类型
    GetDbContext(contextType).Database.ExecuteSqlRaw()
    await GetDbContext(contextType).Database.ExecuteSqlRawAsync()

    // SaveChanges
    GetDbContext<T>().SaveChanges();
    await GetDbContext<T>().SaveChangesAsync();
    _factory.Save();
    await _factory.SaveAsync();
```
### Ending
本数据库纯粹为了方便使用多个DbContext，并未考虑其他状况。如有问题欢迎提出。