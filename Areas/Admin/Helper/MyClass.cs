using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
namespace Website.Areas.Admin.Helper
{
    public class MyClass
    {
        public static string GetConnectionString()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            // Lấy tag MyConnectring bên appsetting.json
            var ConnectionString = config.GetConnectionString("MyConectionString");
            return ConnectionString;
        }
        
    }
}
