using Mapster;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Core.QueryHelper.Data;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ModelAuthInfoController : ControllerBase
    {
        [HttpGet, HttpPost]
        public R<List<ModelAuthInfoVm>> ModelAuthInfoQueryList(PageQueryObject<ModelAuthInfoQuery> query)
        {
            var user = HttpRequestExt.GetLoginUser(Request);
            var list = QuerySvc.QueryList<ModelAuthInfoQuery, ModelAuthInfo, ModelAuthInfoVm>(query, user);
            return R.OK(list);
        }

        [HttpGet, HttpPost]
        public R<int> ModelAuthInfoQueryCount(PageQueryObject<ModelAuthInfoQuery> query)
        {
            var user = HttpRequestExt.GetLoginUser(Request);
            var cnt = QuerySvc.QueryCount<ModelAuthInfoQuery, ModelAuthInfo, ModelAuthInfoVm>(query, user);
            return R.OK(cnt);
        }

        [HttpGet, HttpPost]
        public R<ModelAuthInfoEditVm> ModelAuthInfoGetEditVm(long id)
        {
            using var db = Db.Open();
            var user = HttpRequestExt.GetLoginUser(Request);
            var model = db.Single<ModelAuthInfo>(a => a.Id == id && a.TenantId == user.TenantId);
            if (model == null)
            {
                return R.Faild<ModelAuthInfoEditVm>("未查找到数据");
            }

            return R.OK(model.Adapt<ModelAuthInfoEditVm>());
        }

        [HttpGet, HttpPost]
        public R<ModelAuthInfoEditVm> ModelAuthInfoSaveEditVm(ModelAuthInfoEditVm vm)
        {
            using var db = Db.Open();

            var model = vm.Adapt<ModelAuthInfo>();
            var user = HttpRequestExt.GetLoginUser(Request);
            model.TenantId = user.TenantId;
            if (model.Id == 0)
            {
                model.Id = Id.NewId();
                db.Insert(model);
            }
            else
            {
                db.Update(model);
            }

            return R.OK(model.Adapt<ModelAuthInfoEditVm>());
        }

        [HttpGet, HttpPost]
        public R ModelAuthInfoDelete(string ids)
        {
            var idList = ids.SplitEx().Select(id => id.ToLong() ?? 0).ToList();
            using var db = Db.Open();
            var user = HttpRequestExt.GetLoginUser(Request);
            db.Delete<ModelAuthInfo>(x => idList.Contains(x.Id) && x.TenantId == user.TenantId);
            return R.OK();
        }
    }
}
