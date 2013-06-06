using System.IO;
using System.Reflection;

namespace SilAPI.Tests
{
    public static class TestHelper
    {
        public static string GetTestsPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty);
        }
    }
}