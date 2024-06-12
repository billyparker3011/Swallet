namespace Lottery.Data.Helper
{
    public static class MigrationHelper
    {
        public static string GetBaseScriptDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
        }
    }
}
