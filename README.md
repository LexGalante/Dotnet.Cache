# Dotnet.Cache
Implementações de cache utilizando .net core 3.0, redis, memory cache

Para configurar o memory cache na classe Startup.cs no metódo ConfigureServices
```
services.AddMemoryCache();
```

Para preparar a conexão com redis na classe Startup.cs no metódo ConfigureServices
```
services.AddDistributedRedisCache(options => 
{
    options.Configuration = Configuration.GetConnectionString("Redis");
    options.InstanceName = Configuration.GetConnectionString("RedisInstance");
    options.ConfigurationOptions = new ConfigurationOptions()//Várias opções da conexão podem ser configuradas aqui
    {
        AbortOnConnectFail = true,
        Ssl = false,
        ConnectTimeout = 10
    };
});
```

