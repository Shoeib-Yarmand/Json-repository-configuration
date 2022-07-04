# JsonRepositoryConfiguration
JsonRepositoryConfiguration is a `ConfigurationProvider` based implementation which enables you to have your **Repository** as the refreshable source of your configuration.

It is very similar to Microsoft.Extensions.Configuration.Json. The only difference is that instead of using a appsettings.json file with `AddJsonFile()`, you can use your own Repository with `AddJsonRepository()`. Your JSON Repository can provide the data from any internal/external source(s) including SQL/NoSQL Database calls, API calls, etc. a JSON Repository implements `IJsonConfigurationRepository` which only has `GetByKey(string key)` method.

This package targets .NET Standard 2.0.

* Fully works with any mix of other Configuration Providers.
* Supports **Refresh On Change**.
* Overwrites configs that has been registered before it, and will be overwritten by those that has been registered after it (exactly like all other configuration providers in .NET).
* You still have all those good stuff like `IOptions<>`, `IOptionsMonitor<>`, `IOptionsSnapshot<>` etc.

## Installing via NuGet
    Install-Package JsonRepositoryConfiguration
   
## Example
```
public class MyJsonRepository : IJsonConfigurationRepository
{
    private readonly string _connectionString;

    public MyJsonRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string Get(string key)
    {
        //Make some DB calls
        return value;
        /*
        {
          "myConfig": {
            "cofnig1": "someString",
            "config2": 5,
            "config3": true
          }
        }      
        */
    }
}
```

```
// Optionaly add your appsettings files as usual:
builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json");

// Add the JSON Repository
// Assuming this has been set in the previous line.
var connectionString = builder.Build().GetSection("ConnectionStrings:MyConnectionString").Value;
builder.AddJsonRepository("appsettings.NZ.json", new MyJsonRepository(connectionString), optional: false, reloadOnChange: true, changeCheckInterval: 20);
```

>**Important**: To enable **Reload On Change** feature you MUST also register needed services as following:
```
public void ConfigureServices(IServiceCollection services){
    // Registering needed services for Reload On Change feature:
    services.AddJsonRepositoryReloadOnChange(Configuration);
    
    // Binding:
    services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
}
```

>Note: The return string from your JSON Repository must be a valid JSON **string** (serialized/stringified) starting with "{". Exactly like appsettings.json.

>Note: You can provide all your appsettings or only parts of it. This is the same as what you can do with appsettings.json and appsettings.development.json.