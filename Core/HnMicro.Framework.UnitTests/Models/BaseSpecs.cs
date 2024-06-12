using HnMicro.Framework.UnitTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HnMicro.Framework.UnitTests.Models;

[TestClass]
public class BaseSpecs
{
    protected IConfiguration Configuration;
    protected IServiceCollection ServiceCollection;
    protected IServiceProvider ServiceProvider;

    public BaseSpecs()
    {
        Configuration = "appsettings.json".BuildConfiguration();
        ServiceCollection = Configuration.BuildServiceCollection();
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
}
