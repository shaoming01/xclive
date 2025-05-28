using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Cache;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Dto;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public class UserSvc
{
    public static R Register(UserRegisterVm vm)
    {
        using var db = Db.Open();
        var user = db.Single<UserInfo>(u => u.Name == vm.UserName);
        if (user != null)
        {
            return R.Faild("用户名已存在");
        }

        user = new UserInfo
        {
            Id = Id.NewId(),
            TenantId = 0,
            Name = vm.UserName,
            Password = vm.Password,
            Created = DateTime.Now,
            CardKey = vm.CardKey,
        };
        var cardKeyCheckRe = CheckCardKey(vm.CardKey, user.Id, user.TenantId);
        if (!cardKeyCheckRe.Success) return R.Faild(cardKeyCheckRe.Message);
        user.TenantId = cardKeyCheckRe.Data.TenantId;
        db.Update<UserInfo>(new { CardKey = "" }, u => u.CardKey == vm.CardKey);
        db.Insert(user);
        CrateUserDemoData(user);
        return R.OK();
    }

    public static string GetCardKeyDays(CardKey cardKey)
    {
        string daysStr;
        if (!cardKey.Days.HasValue)
        {
            daysStr = "永久";
        }
        else
        {
            var expiry = (cardKey.ActiveDate ?? DateTime.Now).AddDays(cardKey.Days.Value);
            var span = expiry - DateTime.Now;
            daysStr = $"{(int)span.TotalDays}天{span.Hours}小时";
        }

        return daysStr;
    }

    public static R<CardKey> CheckCardKey(string? cardKey, long userId, long tenantId)
    {
        if (!cardKey.Has())
        {
            return R.Faild<CardKey>("卡密为空");
        }

        var cardLong = cardKey.ToLong() ?? 0;

        using var db = Db.Open();
        //校验卡密
        var card = db.Single<CardKey>(c => c.Id == cardLong);
        if (card == null || card.InValid)
        {
            return R.Faild<CardKey>("卡密无效");
        }

        if (card.ActiveDate.HasValue) //校验卡密过期
        {
            if (card.Days.HasValue && (DateTime.Now - card.ActiveDate).Value.TotalMinutes >
                new TimeSpan(card.Days ?? 0, 0, 0).TotalMinutes)
            {
                return R.Faild<CardKey>("卡密已过期");
            }

            card.CurrentUserId = userId;
            db.Update(card);
            return R.OK(card);
        }
        else //激活卡密
        {
            if (card.Expiry.HasValue && card.Expiry.Value < DateTime.Now)
            {
                return R.Faild<CardKey>("卡密已过期");
            }

            card.ActiveDate = DateTime.Now;
            card.ActiveUserId = userId;
            card.CurrentUserId = userId;
            db.Update(card);
            return R.OK(card);
        }
    }

    public static R ResetPassword(UserResetPasswordVm vm)
    {
        using var db = Db.Open();
        var user = db.Single<UserInfo>(u => u.Name == vm.UserName && u.CardKey == vm.CardKey);
        if (user == null) return R.Faild("用户名或卡密不正确，请确保之前此用户名绑定的卡密就是现在填入的卡密");

        var cardKeyCheckRe = CheckCardKey(vm.CardKey, user.Id, user.TenantId);
        if (!cardKeyCheckRe.Success) return R.Faild(cardKeyCheckRe.Message);
        db.Update<UserInfo>(new { CardKey = "" }, u => u.CardKey == vm.CardKey);
        user.Password = vm.Password;
        db.Update(user);
        return R.OK();
    }

    public static R UseCardKey(string userName, string password, string cardKey)
    {
        using var db = Db.Open();
        var user = db.Single<UserInfo>(u => u.Name == userName && u.Password == password);
        if (user == null) return R.Faild("用户名或密码不正确");
        var cardKeyCheckRe = CheckCardKey(cardKey, user.Id, user.TenantId);
        if (!cardKeyCheckRe.Success) return R.Faild(cardKeyCheckRe.Message);
        user.CardKey = cardKey;
        db.Update(user);
        return R.OK();
    }

    public static R CrateUserDemoData(UserInfo user)
    {
        var templateList = DemoDataSvc.CreateDemoScriptTemplate();
        templateList.ForEach(t =>
        {
            t.Id = Id.NewId();
            t.UserId = user.Id;
            t.TenantId = user.TenantId;
        });
        var anchorList = DemoDataSvc.CreateDemoAiVerticalAnchor();
        anchorList.ForEach(a =>
        {
            a.Id = Id.NewId();
            a.TenantId = user.TenantId;
            a.UserId = user.Id;
        });
        using var db = Db.Open();
        db.InsertAll(templateList);
        db.InsertAll(anchorList);
        return R.OK();
    }

    public static R<UserLoginInfoVm> Login(string userName, string password)
    {
        if (userName.IsNullOrEmpty() || password.IsNullOrEmpty())
        {
            return R.Faild<UserLoginInfoVm>("用户名或密码不能为空");
        }

        using var db = Db.Open();
        var user = db.Single<UserInfo>(u => u.Name == userName && u.Password == password);
        if (user == null)
        {
            return R.Faild<UserLoginInfoVm>("用户名或密码不存在");
        }

        var cardRe = UserSvc.CheckCardKey(user.CardKey, user.Id, user.TenantId);
        if (!cardRe.Success)
        {
            return R.Faild<UserLoginInfoVm>(cardRe.Message);
        }

        var token = $"{user.Id}:{Id.NewId()}";
        var loginInfo = CacheHelper.Get<UserLoginInfo>(0, "Login:" + user.Id);
        if (loginInfo == null)
        {
            loginInfo = new UserLoginInfo
            {
                UserId = user.Id,
                Name = user.Name,
                TenantId = user.TenantId,
                Tokens = new List<string>()
                {
                    token
                },
            };
        }
        else
        {
            //超过限制数量，删除最早的一个
            if (loginInfo.Tokens.Count >= 5)
                loginInfo.Tokens.RemoveAt(0);
            loginInfo.Tokens.Add(token);
        }

        CacheHelper.SetKey(0, "Login:" + user.Id, loginInfo);
        var vm = new UserLoginInfoVm
        {
            UserId = user.Id,
            Name = user.Name,
            Token = token,
            TenantId = user.TenantId,
            days = UserSvc.GetCardKeyDays(cardRe.Data),
        };

        return R.OK(vm);
    }

    public static R<UserLoginInfoVm> GetLoginUserVm(string token)
    {
        if (token.IsNullOrWhiteSpace())
        {
            return R.Faild<UserLoginInfoVm>("登录信息为空");
        }

        var arr = token.SplitEx(':', true);
        if (arr.Length != 2)
        {
            return R.Faild<UserLoginInfoVm>("登录信息无效");
        }

        var tokenObj = CacheHelper.Get<UserLoginInfo>(0, "Login:" + arr[0]);
        if (tokenObj == null)
        {
            return R.Faild<UserLoginInfoVm>("登录信息已经失效");
        }

        if (!tokenObj.Tokens.Has(t => t == token))
        {
            return R.Faild<UserLoginInfoVm>("登录失效，用户已在其他地方登录");
        }

        using var db = Db.Open();
        var userInfo = db.SingleById<UserInfo>(tokenObj.UserId)!;
        var cardRe = CheckCardKey(userInfo.CardKey, tokenObj.UserId, tokenObj.TenantId);
        if (!cardRe.Success)
        {
            return R.Faild<UserLoginInfoVm>(cardRe.Message);
        }

        var vm = new UserLoginInfoVm
        {
            UserId = userInfo.Id,
            Name = userInfo.Name,
            Token = token,
            TenantId = userInfo.TenantId,
            days = GetCardKeyDays(cardRe.Data),
        };
        return R.OK(vm);
    }

    public static UserLoginInfo GetLoginUser(string token)
    {
        if (token.IsNullOrWhiteSpace())
        {
            throw new Exception("登录信息为空");
        }

        var arr = token.SplitEx(':', true);
        if (arr.Length != 2)
        {
            throw new Exception("登录信息无效");
        }

        var tokenObj = CacheHelper.Get<UserLoginInfo>(0, "Login:" + arr[0]);
        if (tokenObj == null)
        {
            throw new Exception("登录信息已经失效");
        }

        if (!tokenObj.Tokens.Has(t => t == token))
        {
            throw new Exception("登录失效，用户已在其他地方登录");
        }

        using var db = Db.Open();
        var userInfo = db.SingleById<UserInfo>(tokenObj.UserId)!;
        var cardRe = CheckCardKey(userInfo.CardKey, tokenObj.UserId, tokenObj.TenantId);
        if (!cardRe.Success)
        {
            throw new Exception(cardRe.Message);
        }

        return tokenObj;
    }

    public static R Logout(string token)
    {
        var arr = token.SplitEx(':', true);
        if (arr.Length != 2)
        {
            return R.Faild<UserLoginInfo>("token 无效");
        }

        var tokenObj = CacheHelper.Get<UserLoginInfo>(0, "Login:" + arr[0]);
        if (tokenObj == null) return R.OK();
        tokenObj.Tokens.Remove(token);
        CacheHelper.SetKey(0, "Login:" + arr[0], tokenObj);
        return R.OK();
    }
}