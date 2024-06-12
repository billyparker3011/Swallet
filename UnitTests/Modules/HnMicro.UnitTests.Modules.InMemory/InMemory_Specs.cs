using HnMicro.Framework.UnitTests.Models;
using HnMicro.Modules.InMemory.UnitOfWorks;
using HnMicro.UnitTests.Modules.InMemory.Models;
using HnMicro.UnitTests.Modules.InMemory.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.UnitTests.Modules.InMemory;

[TestClass]
public class InMemory_Specs : BaseSpecs
{
    private IInMemoryUnitOfWork _unitOfWork;
    private IUserInMemoryRepository _userInMemoryRepository;

    [TestInitialize()]
    public void Setup()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
        _unitOfWork = ServiceProvider.GetService<IInMemoryUnitOfWork>();
        _userInMemoryRepository = _unitOfWork.GetRepository<IUserInMemoryRepository>();
    }

    [TestMethod]
    public void Init_Unit_Of_Work()
    {
        Assert.IsNotNull(_unitOfWork);
    }

    [TestMethod]
    public void Add_User()
    {
        var userId = 1;
        var username = "001";
        var password = "Aaaa123123123";

        _userInMemoryRepository.Add(new User
        {
            UserId = userId,
            Username = username,
            Password = password
        });

        var user = _unitOfWork.GetRepository<IUserInMemoryRepository>().FindById(userId);
        Assert.IsNotNull(user);
        Assert.AreEqual(user.UserId, userId);
        Assert.AreEqual(user.Username, username);
        Assert.AreEqual(user.Password, password);
    }
}