using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets("fb603ff5-AzAIServices")
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();
