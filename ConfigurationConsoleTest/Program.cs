using ConfigurationLibrary;

var configReader = new ConfigurationReader("SERVICE-A", 
    "Server=.;Database=ConfigurationDb;Integrated Security=True;" +
    "TrustServerCertificate=True;", 1000);
var siteName = configReader.GetValue<string>("SiteName");

Console.WriteLine(siteName);