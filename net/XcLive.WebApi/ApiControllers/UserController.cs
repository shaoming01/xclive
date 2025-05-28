using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    [HttpGet, HttpPost]
    public R<List<UserVm>> QueryList(PageQueryObject<UserQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = QuerySvc.QueryList<UserQuery, UserInfo, UserVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> QueryCount(PageQueryObject<UserQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var count = QuerySvc.QueryCount<UserQuery, UserInfo, UserVm>(query, user);
        return R.OK(count);
    }

    [HttpGet, HttpPost]
    public R<List<UserRoleDetailVm>> UserRoleDetailQueryList(PageQueryObject<UserRoleDetailQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var list = QuerySvc.QueryList<UserRoleDetailQuery, UserRoleDetail, UserRoleDetailVm>(query, user);
        return R.OK(list);
    }

    [HttpGet, HttpPost]
    public R<int> UserRoleDetailQueryCount(PageQueryObject<UserRoleDetailQuery> query)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var count = QuerySvc.QueryCount<UserRoleDetailQuery, UserRoleDetail, UserRoleDetailVm>(query, user);
        return R.OK(count);
    }

    [HttpGet, HttpPost]
    public R<UserEditVm> Save(UserEditVm item)
    {
        var isNew = item.Id == 0;
        using var db = Db.Open();
        if (isNew)
        {
            var model = new UserInfo
            {
                Id = Id.NewId(),
                Created = DateTime.Now,
                Name = item.Name,
                Memo = item.Memo,
                Password = item.Password,
            };
            item.Id = model.Id;
            db.Insert(model);
        }
        else
        {
            var model = db.SingleById<UserInfo>(item.Id);
            model.Name = item.Name;
            model.Memo = item.Memo;
            db.Update(model);
            db.Delete<UserRoleDetail>(d => d.HeaderId == item.Id);
        }

        var detailList = item.UserRoles?.Select(r => new UserRoleDetail
                             {
                                 HeaderId = item.Id,
                                 RoleId = r.RoleId,
                                 Id = r.Id == 0 ? Id.NewId() : r.Id
                             })
                             .ToList() ??
                         new List<UserRoleDetail>();
        db.InsertAll(detailList);
        return Get(item.Id);
    }


    [HttpGet, HttpPost]
    public R<UserEditVm> Get(long id)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        var reItem = CommonSvc.Get<UserInfo>(id);
        var roleList = QuerySvc.QueryList<UserRoleDetailQuery, UserRoleDetail, UserRoleDetailVm>(
            new PageQueryObject<UserRoleDetailQuery>
            {
                QueryObject = new UserRoleDetailQuery { HeaderId = id, },
                Page = 1,
                PageSize = 1000,
            }, user);
        var roleVm = reItem.Adapt<UserEditVm>();
        roleVm.UserRoles = roleList;
        return R.OK(roleVm);
    }

    [HttpGet, HttpPost]
    public R Delete(string ids)
    {
        var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
        using var db = Db.Open();
        db.Delete<UserInfo>(user => idList.Contains(user.Id));
        db.Delete<UserRoleDetail>(d => idList.Contains(d.HeaderId));
        return R.OK();
    }


    [HttpGet, HttpPost, AllowAnonymous]
    public R<UserLoginInfoVm> Login(string userName, string password)
    {
        return UserSvc.Login(userName, password);
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R<UserLoginInfoVm> Info(string token)
    {
        return UserSvc.GetLoginUserVm(token);
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R Logout(string token)
    {
        return UserSvc.Logout(token);
    }

    [HttpGet, HttpPost]
    public R<List<SearchGroupEditVm>> QuerySearchGroups(PageQueryObject<UserSearchGroupQuery> query)
    {
        if (query.QueryObject == null || (query.QueryObject.Path.IsNullOrWhiteSpace() && query.QueryObject.Id == null))
        {
            return R.Faild<List<SearchGroupEditVm>>("查询条件不能为空");
        }

        using var db = Db.Open();
        if (query.QueryObject.Id != null)
        {
            var list = db.Select<UserSearchGroup>(u => u.Id == query.QueryObject.Id);
            var vmList = list.Select(SearchGroupEditVm.FromModel).ToList();
            return R.OK(vmList);
        }
        else
        {
            var path = query.QueryObject.Path.ClearPath();
            var user = HttpRequestExt.GetLoginUser(Request);
            var list = db.Select<UserSearchGroup>(u => u.UserId == user.UserId && u.Path == path);
            var vmList = list.Select(SearchGroupEditVm.FromModel).ToList();
            return R.OK(vmList);
        }
    }

    [HttpGet, HttpPost]
    public R<SearchGroupEditVm> SaveSearchGroup(SearchGroupEditVm groupEditVm)
    {
        var user = HttpRequestExt.GetLoginUser(Request);
        if (groupEditVm.Name.IsNullOrWhiteSpace())
        {
            return R.Faild<SearchGroupEditVm>("分组名称不能为空");
        }

        var group = SearchGroupEditVm.ToModel(groupEditVm, user.UserId);
        using var db = Db.Open();
        if (group.Id == 0)
        {
            group.Id = Id.NewId();
            db.Insert(group);
        }
        else
        {
            db.Update(group);
        }

        return R.OK(SearchGroupEditVm.FromModel(group));
    }

    [HttpGet, HttpPost]
    public R DeleteSearchGroup(long id)
    {
        using var db = Db.Open();
        db.Delete<UserSearchGroup>(g => g.Id == id);
        return R.OK();
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R Register(UserRegisterVm vm)
    {
        return UserSvc.Register(vm);
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R ResetPassword(UserResetPasswordVm vm)
    {
        return UserSvc.ResetPassword(vm);
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R BindCardKey(string userName, string password, string cardKey)
    {
        return UserSvc.UseCardKey(userName, password, cardKey);
    }
}