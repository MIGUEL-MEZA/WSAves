namespace WSOptimizerAves.Config
{
    public static class AppSetConfig
    {
        public static IConfiguration AppSetting { get; }
        static AppSetConfig()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

        }
    }
}
