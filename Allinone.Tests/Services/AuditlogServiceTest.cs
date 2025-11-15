using Allinone.BLL;
using Allinone.BLL.Auditlogs;
using Allinone.BLL.Diarys;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Auditlogs;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    //public class AuditlogServiceTest
    //{
    //    private readonly AuditlogService _auditlogService;

    //    private readonly int _memberId = 1;

    //    private readonly int _auditlogId = 1;
    //    private readonly int _auditlogTypeId = (int)EnumAuditlogType.Login;
    //    private readonly int _auditlogActionTypeId = (int)EnumAuditlogActionType.New;
    //    private readonly string _name = "name";
    //    private readonly DateTime _createdTime = DateTime.Now;
    //    private readonly string _oldValue = "oldValue";
    //    private readonly string _newValue = "newValue";


    //    public AuditlogServiceTest()
    //    {
    //        BaseBLL.MemberId = _memberId;

    //        var services = new ServiceCollection();

    //        var options = new DbContextOptionsBuilder<DSContext>()
    //            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    //            .Options;

    //        var context = new DSContext(options);

    //        context.Auditlog.AddRange(
    //            new Auditlog
    //            {
    //                ID = 1,
    //                TypeID = _auditlogTypeId,
    //                ActionTypeID = _auditlogActionTypeId,
    //                Name = _name,
    //                CreatedTime = _createdTime,
    //                OldValue = _oldValue,
    //                NewValue = _newValue,
    //                MemberID = _memberId
    //            }
    //        );
    //        context.SaveChanges();

    //        services.AddAutoMapper(typeof(MappingProfile));
    //        services.AddTransient<IMapModel, MapModel>();
    //        services.AddMemoryCache();
    //        services.AddSingleton<MemoryCacheHelper>();

    //        var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();
    //        var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

    //        var auditlogRepository = new AuditlogRepository(context);

    //        _auditlogService = new AuditlogService(auditlogRepository, mapModel);
    //    }

    //    [Fact]
    //    public async Task GetAllByMember_Returns_Success()
    //    {
    //        // Act
    //        var result = await _auditlogService.GetAllByMemberAsync();

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(_diaryBookName, result!.FirstOrDefault().Name);
    //        Assert.Equal(_diaryBookDesc, result!.FirstOrDefault().Description);
    //    }

    //    [Fact]
    //    public async Task GetAllByMember_id_Returns_Success()
    //    {
    //        // Act
    //        var result = await _auditlogService.GetAllByMemberAsync(_diaryBookId);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(_diaryBookName, result!.Name);
    //        Assert.Equal(_diaryBookDesc, result!.Description);
    //    }

    //    [Fact]
    //    public async Task Add_Returns_Success()
    //    {
    //        // Assign
    //        var req = new DiaryBookAddReq
    //        {
    //            Name = "new name",
    //            Description = "new desc"
    //        };

    //        // Act
    //        var result = await _auditlogService.AddAsync(req);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(req.Name, result!.Name);
    //        Assert.Equal(req.Description, result!.Description);
    //    }

    //    [Fact]
    //    public async Task Update_Returns_Success()
    //    {
    //        // Assign
    //        var req = new DiaryBookAddReq
    //        {
    //            Name = "update name",
    //            Description = "update desc"
    //        };

    //        // Act
    //        var result = await _auditlogService.UpdateAsync(_diaryBookId, req);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(req.Name, result!.Name);
    //        Assert.Equal(req.Description, result!.Description);
    //    }

    //    [Fact]
    //    public async Task Delete_Returns_Success()
    //    {
    //        // Act
    //        var result = await _auditlogService.DeleteAsync(_diaryBookId);

    //        // Assert
    //        Assert.NotNull(result);

    //        // Act & Assert
    //        await Assert.ThrowsAsync<DiaryBookNotFoundException>(async () =>
    //        {
    //            await _auditlogService.GetAllByMemberAsync(_diaryBookId);
    //        });
    //    }
    //}
}
