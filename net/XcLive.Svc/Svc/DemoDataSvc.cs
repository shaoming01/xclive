using System.Diagnostics;
using Newtonsoft.Json;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Schema;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc.SchemaBuild;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Svc.Svc;

public class DemoDataSvc
{
    public static Tenant[] CreateDemoTenant()
    {
        return
        [
            new Tenant
            {
                Id = 1,
                Name = "星晨Ai直播",
                ProductName = "星晨Ai直播",
                ProductVersion = "4.20",
            },
        ];
    }

    public static CardKey[] CreateDemoCardKey()
    {
        return
        [
            new CardKey
            {
                Id = 1,
                Created = DateTime.Now,
                TenantId = 1,
                Days = 365,
                InValid = false,
                Expiry = DateTime.Now.AddDays(365),
                ActiveDate = DateTime.Now,
                ActiveUserId = 1,
                CurrentUserId = 1,
            },
            new CardKey
            {
                Id = 2,
                Created = DateTime.Now,
                TenantId = 1,
                Days = 30,
                InValid = false,
                Expiry = DateTime.Now.AddDays(365),
                ActiveDate = DateTime.Now,
                ActiveUserId = 1,
                CurrentUserId = 1,
            },
        ];
    }

    public static ModelAuthInfo[] CreateDemoModelAuthInfo()
    {
        return
        [
            new ModelAuthInfo
            {
                Id = 1,
                PlatformType = ModelPlatformType.Doubao,
                Name = "豆包语言模型",
                TextModelId = "", //这里添加自己的豆包模型授权信息
                ImageModelId = "",//这里添加自己的豆包模型授权信息
                EndPoint = "https://ark.cn-beijing.volces.com/api/v3/",
                ApiKey = "",//这里添加自己的豆包模型授权信息
            },
        ];
    }

    public static Menu[] CreateDemoMenu()
    {
        return
        [
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 1,
                Title = "菜单管理",
                Url = "/module/101",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 2,
                Title = "模块管理",
                Url = "/module/102",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 3,
                Title = "Vue组件浏览",
                Url = "/module/103",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 4,
                Title = "话术生成",
                Url = "/module/LiveScriptVm",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 5,
                Title = "话术模板",
                Url = "/module/LiveScriptTemplateVm",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 6,
                Title = "AI行业主播",
                Url = "/module/AiVerticalAnchorVm",
                ParentId = 0,
                TenantId = 1,
            },
            new Menu
            {
                Desc = "",
                Icon = "",
                Id = 7,
                Title = "直播助手",
                Url = "/liveIndex",
                ParentId = 0,
                TenantId = 1,
            },
        ];
    }

    public static Module[] CreateDemoModules()
    {
        return
        [
            new Module
            {
                Id = 101,
                Name = "菜单管理",
                SysModuleName = nameof(Menu),
                ComPath = "/src/components/pages/MenuDataBrowser.vue"
            },
            new Module
            {
                Id = 102,
                Name = "模块管理",
                SysModuleName = nameof(Module),
                ComPath = "/src/components/pages/ModuleDataBrowser.vue"
            },
            new Module
            {
                Id = 103,
                Name = "Vue组件浏览",
                ComPath = "/src/components/pages/VueComDataBrowser.vue"
            },
            new Module
            {
                Id = 104,
                Name = "菜单编辑",
                SysModuleName = nameof(MenuEditVm) + "_ModalObjectEditor",
                ComPath = "/src/components/base/ModalObjectEditor.vue"
            },
        ];
    }

    public static Module[] GetNewSystemModules()
    {
        var modules = CreteSystemModules();
        modules.Where(m => m.SysModuleId == "LiveScriptVm")
            .ForEach(m => m.ComPath = "/src/components/pages/LiveScriptDataBrowser.vue");
        using var db = Db.Open();
        var sql = string.Format("select SysModuleId from Module where SysModuleId IN ({0})",
            modules.Select(m => m.SysModuleId)!.ToSqlPar());
        var extIds = db.Select<string>(sql).ToList();
        return modules.Where(m => m.SysModuleId != null && !extIds.Contains(m.SysModuleId)).ToArray();
    }

    public static Module[] CreteSystemModules()
    {
        var list = SysModuleBuilder.List();

        return list.Select(l =>
        {
            var module = new Module
            {
                Id = Id.NewId(),
                SysModuleName = l.Value,
                SysModuleId = l.Value,
                Name = l.Value,
                ComPath = "",
                CategoryPath = CalcCategoryPath(l.Value),
            };
            if (l.Label.Contains(nameof(DataBrowserProp)))
            {
                module.ComPath = "/src/components/base/DataBrowser.vue";
            }
            else if (l.Label.Contains(nameof(ModalObjectEditorProp)))
            {
                module.ComPath = "/src/components/base/ModalObjectEditor.vue";
            }
            else if (l.Label.Contains(nameof(ObjectEditorProp)))
            {
                module.ComPath = "/src/components/base/ObjectEditor.vue";
            }
            else if (l.Label.Contains(nameof(ModalDataSelectProp)))
            {
                module.ComPath = "/src/components/base/ModalDataSelect.vue";
            }

            return module;
        }).ToArray();
    }

    private static string CalcCategoryPath(string sysModuleName)
    {
        if (sysModuleName.EndsWith("EditVm_ModalObjectEditor"))
        {
            return sysModuleName.Replace("EditVm_ModalObjectEditor", "");
        }
        else if (sysModuleName.EndsWith("Vm"))
        {
            return sysModuleName.Replace("Vm", "");
        }

        return "";
    }

    public static List<UserInfo> CreateDemoUser()
    {
        var list = new List<UserInfo>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Name = "admin",
                Password = "admin",
                Created = DateTime.Now,
                CardKey = "1",
            }
        };
        return list;
    }

    public static List<LiveScriptTemplate> CreateDemoScriptTemplate()
    {
        var list = new List<LiveScriptTemplate>
        {
            new()
            {
                Id = 1,
                Name = "话术规则1",
                Usage = UsageType.LiveScript,
                TenantId = 1,
                UserId = 1,
                SystemTemplate = "你是一个专业的直播话术生成专员，以一整个文章的方式返回给我，按用户要求编写，特别是文字数量。",
                UserTemplate =
                    "商品名称的标题\n以下是直播间的基本话术，可能是一个或多个商品的介绍：\n<basic_script>直播间的基本情况为：\n{{直播间描述}}</basic_script>\n在编写话术时，请遵循以下要求：\n1、字数一定要满足我的要求：4000个文字左右,和上一次生成内容都不能重复\n2、在编写之前，你需要参考同类商品优秀主播的话术，汲取其中的优点和经验来完善你的话术。\n3、整体的话术框架环节为：拉新互动(强调前面已经发放过)---核心卖点(综合结合产品信息)---用户痛点(增强代入感)---促单环节(促进下单,强调稀缺性)---保障话术(简单明了)，注意：环节名称不要写在话术中。\n4、在这个讲解的过程中可以一些穿插真实案例\n5、所有的框架环节都需加入互动的指令，增加与观众的互动，如：可以在例如用户痛点环节问有没有情况？有的打个有字这个的等。\n6、不能出现有关优先发货的内容和话术\n7、规避功效、医疗、疗效方面的说法，特别是医疗和疗效类的内容，如：能让人**样，**疗效，**功效\n8、不能出现关于优先发货的内容和话术。\n9、不要出现诱导性的话术，如：只要**样就**样，你**样我就**样\n10、一定不能出现任何极限词，如：第一时间、超级、顶级、绝对、顶级、顶峰、绝无仅有、国家级、万能、首位、世界级、第一、唯一、最好、最佳、顶尖、无可比拟、无与伦比、极致、独一无二、完美、无敌、极好、极佳、超级、超强、最强、最棒、最牛、最先、最火、最热、最酷、最高、最低、绝佳、最优、100%、首个、首选、首家、免检、一流、治疗、功效、疾病、 - 、神器、加急发货、优先、\n、原价。\n11、当库存数量低于100个的时候才提库存的事情\n{{个性描述}}\n",
            },
            new()
            {
                Id = 2,
                Name = "话术规则2",
                Usage = UsageType.LiveScript,
                TenantId = 1,
                UserId = 1,
                SystemTemplate = "你是一个专业的直播话术生成专员，以一整个文章的方式返回给我，按用户要求编写，特别是文字数量。",
                UserTemplate =
                    "商品名称的标题\n以下是直播间的基本话术，可能是一个或多个商品的介绍：\n<basic_script>直播间的基本情况为：\n{{直播间描述}}</basic_script>\n在编写话术时，请遵循以下要求：\n1、字数一定要满足我的要求：4000个文字左右,和上一次生成内容都不能重复\n2、在编写之前，你需要参考同类商品优秀主播的话术，汲取其中的优点和经验来完善你的话术。\n3、整体的话术框架环节为：拉新询问(强调前面已经发放过)---人性话术(挖掘痛点)---规格选择互动(增加停留)---产品介绍(以使用场景为主)---保障话术(简单明了)---商品福利(下单伏笔)---促单话术(表达稀缺)，注意：环节名称不要写在话术中。\n4、所有的框架环节都需加入互动的指令，尽量用单个数字的方式要指令。推荐常用指令为：1、6、8，要考虑到指令的反应时间；假如是《规格选择互动》的时候用1、2、3的方式:只选择3-5个规格就可以了。\n5、符合抖音直播的规则话术，不能出现违规的内容。\n6、不能出现有关优先发货的内容和话术\n7、规避功效、医疗、疗效方面的说法，特别是医疗和疗效类的内容，如：能让人**样，**疗效，**功效\n8、不能出现关于优先发货的内容和话术。\n9、不要出现诱导性的话术，如：只要**样就**样，你**样我就**样\n10、一定不能出现任何极限词，如：第一时间、超级、顶级、绝对、顶级、顶峰、绝无仅有、国家级、万能、首位、世界级、第一、唯一、最好、最佳、顶尖、无可比拟、无与伦比、极致、独一无二、完美、无敌、极好、极佳、超级、超强、最强、最棒、最牛、最先、最火、最热、最酷、最高、最低、绝佳、最优、100%、首个、首选、首家、免检、一流、治疗、功效、疾病、 - 、神器、加急发货、优先、\n、原价。\n11、当库存数量低于100个的时候才提库存的事情\n{{个性描述}}\n",
            },
            new()
            {
                Id = 3,
                Name = "话术规则3",
                Usage = UsageType.LiveScript,
                TenantId = 1,
                UserId = 1,
                SystemTemplate = "你是一个专业的直播话术生成专员，以一整个文章的方式返回给我，按用户要求编写，特别是文字数量。",
                UserTemplate =
                    "商品名称的标题\n以下是直播间的基本话术，可能是一个或多个商品的介绍：\n<basic_script>直播间的基本情况为：\n{{直播间描述}}</basic_script>\n在编写话术时，请遵循以下要求：\n1、字数一定要满足我的要求：4000个文字左右,和上一次生成内容都不能重复\n2、在编写之前，你需要参考同类商品优秀主播的话术，汲取其中的优点和经验来完善你的话术。\n3、整体的话术框架环节为：开场环节(强调前面已经发放过)---产品详细(促销价格环节)---用户痛点(解决方案)---互动环节（增加观众参与感）---促单环节(如时间稀缺，库存稀缺)---保障话术(简单明了)，注意：环节名称不要写在话术中。\n4、在这个讲解的过程中可以一些穿插真实案例\n5、所有的框架环节都需加入简单的互动指令(1-2个字)，指令要简单需要考虑观众打字速度，增加与观众的互动，用福利，痛点留住观众。\n6、不能出现有关优先发货的内容和话术\n7、规避功效、医疗、疗效方面的说法，特别是医疗和疗效类的内容，如：能让人**样，**疗效，**功效\n8、不能出现关于优先发货的内容和话术。\n9、不要出现诱导性的话术，如：只要**样就**样，你**样我就**样\n10、一定不能出现任何极限词，如：第一时间、超级、顶级、绝对、顶级、顶峰、绝无仅有、国家级、万能、首位、世界级、第一、唯一、最好、最佳、顶尖、无可比拟、无与伦比、极致、独一无二、完美、无敌、极好、极佳、超级、超强、最强、最棒、最牛、最先、最火、最热、最酷、最高、最低、绝佳、最优、100%、首个、首选、首家、免检、一流、治疗、功效、疾病、 - 、神器、加急发货、优先、\n、原价。\n11、当库存数量低于100个的时候才提库存的事情\n{{个性描述}}\n",
            },
            new()
            {
                Id = 4,
                Name = "回复规则",
                Usage = UsageType.Chat,
                TenantId = 1,
                UserId = 1,
                SystemTemplate =
                    "你是一个专业的抖音主播，每次输出的时候不能超过80个字符,根据下面的要求回复，特别是文字数量。\n以下是直播间的基本话术，可能是一个或多个商品的介绍：\n<basic_script>\n直播间的基本情况为：\n{{直播间描述}}\n</basic_script>\n在编写话术时，请遵循以下要求：\n1、输出的字数一定要满足我的要求：80个文字以内,同样的问题要和上一次生成内容都不能重复\n2、尽量商品2个字，用别的文字代替：如：宝贝、直播间链接、货品。\n3、话术要口语化，有亲和力、有幽默感，多一些自问自答的话术，如：对的、没错、怎么这一类的内容。\n4、符合抖音直播的规则话术，不能出现违规的内容。\n5、不能出现有关优先发货的内容和话术\n6、规避功效、医疗、疗效方面的说法，特别是医疗类的内容，如：能让人**样，**疗效，**功效\n7、不能出现关于优先发货的内容和话术。\n8、不要出现诱导性的话术，如：只要**样就**样，你**样我就**样\n9、一定不能出现任何极限词，如：第一时间、超级、顶级、绝对、顶级、顶峰、绝无仅有、国家级、万能、首位、世界级、第一、唯一、最好、最佳、顶尖、无可比拟、无与伦比、极致、独一无二、完美、无敌、极好、极佳、超级、超强、最强、最棒、最牛、最先、最火、最热、最酷、最高、最低、绝佳、最优、100%、首个、首选、首家、免检、一流、治疗、功效、疾病、 - 、神器、加急发货、优先、\n、原价。\n10、当库存数量低于100个的时候才提库存的事情\n11、对于刚进入直播间的粉丝，每次要做到不一样的回答，不要重复回答，可以多询问有没有发放过今天的福利\n{{个性描述}}\n",
                UserTemplate = "{{互动消息}}",
            },
            new()
            {
                Id = 5,
                Name = "互动规则",
                Usage = UsageType.InteractScriptTemplate,
                TenantId = 1,
                UserId = 1,
                SystemTemplate =
                    "你是一个专业的抖音主播和助播，需要和直播间的观众互动，根据下面的要求生成两句话，第一句是助播提醒主播直播间里的情况，这个提醒内容要短，20个字以内；第二句是主播响应话术，控制在80字内，每次需要是不同的内容，不一定每次都要助手提醒的。",
                UserTemplate = "{{互动消息}}",
            },
        };

        return list;
    }


    public static List<LiveRoom> CreateDemoLiveRoom()
    {
        var list = new List<LiveRoom>
        {
            new()
            {
                Id = 1,
                Name = "直播间",
                UserId = 1,
                TenantId = 1,
                ProductText = "",
                PersonaText =
                    "13、禁提微信、QQ 等，勿直接让他人加联系方式，这是平台规定。\n14、回答用户问题控制在 80 字内，且带提问人名字。\n15、直播间无电商链接，禁引导相关内容。\n16、话术需变换表达，口语化，禁极限词。\n17、含用户痛点、互动等，规避极限词。\n18、用多种方式润色话术。\n19、用多种直播话术技巧写顺彩烫画内容，有逻辑。\n20、专业术语用大白话幽默表达。\n21、引导有兴趣者私信或看主页联系方式。\n22、话术不含语气词。",
            }
        };

        return list;
    }

    public static List<AiVerticalAnchor> CreateDemoAiVerticalAnchor()
    {
        var list = new List<AiVerticalAnchor>
        {
            new AiVerticalAnchor
            {
                Id = 1,
                TenantId = 1,
                Name = "行业主播1",
                ChatTemplateIds = "4",
                UserId = 1,
                ScriptTemplateIds = "1",
                PrimaryTtsModelId = 1,
                SecondaryTtsModelId = 11,
                InteractTemplateIds = "5",
            },
            new AiVerticalAnchor
            {
                Id = 2,
                TenantId = 1,
                Name = "行业主播2",
                ChatTemplateIds = "4",
                UserId = 1,
                ScriptTemplateIds = "2",
                PrimaryTtsModelId = 2,
                SecondaryTtsModelId = 12,
                InteractTemplateIds = "5",
            },
            new AiVerticalAnchor
            {
                Id = 3,
                TenantId = 1,
                Name = "行业主播3",
                ChatTemplateIds = "4",
                UserId = 1,
                ScriptTemplateIds = "3",
                PrimaryTtsModelId = 3,
                SecondaryTtsModelId = 13,
                InteractTemplateIds = "5",
            },
            new AiVerticalAnchor
            {
                Id = 4,
                TenantId = 1,
                Name = "全能行业主播",
                ChatTemplateIds = "4",
                UserId = 1,
                ScriptTemplateIds = "1,2,3",
                PrimaryTtsModelId = 4,
                SecondaryTtsModelId = 14,
                InteractTemplateIds = "5",
            },
        };

        return list;
    }

    public static List<TtsModel> CreateDemoTtsModel()
    {
        var json =
            "[{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康周周姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康周周姐\\\\健康周周姐_e22_s1628.pth\",\"speaker_name\":\"健康周周姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康宣萱姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康宣萱姐\\\\健康宣萱姐_e20_s520.pth\",\"speaker_name\":\"健康宣萱姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小乐姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小乐姐\\\\健康小乐姐_e20_s240.pth\",\"speaker_name\":\"健康小乐姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小亮姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小亮姐\\\\健康小亮姐_e20_s480.pth\",\"speaker_name\":\"健康小亮姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小古姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康小古姐\\\\健康小古姐_e20_s620.pth\",\"speaker_name\":\"健康小古姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康王山哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\健康王山哥\\\\健康王山哥_e18_s1008.pth\",\"speaker_name\":\"健康王山哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学大王哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学大王哥\\\\国学大王哥_e18_s882.pth\",\"speaker_name\":\"国学大王哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学大真哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学大真哥\\\\国学大真哥_e18_s792.pth\",\"speaker_name\":\"国学大真哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学王老师\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\国学王老师\\\\国学王老师_e10_s570.pth\",\"speaker_name\":\"国学王老师\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\娱乐妖姬姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\娱乐妖姬姐\\\\娱乐妖姬姐_e18_s756.pth\",\"speaker_name\":\"娱乐妖姬姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电大清哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电大清哥\\\\家电大清哥_e16_s96.pth\",\"speaker_name\":\"家电大清哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电小平姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电小平姐\\\\家电小平姐_e20_s500.pth\",\"speaker_name\":\"家电小平姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电小灯哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电小灯哥\\\\家电小灯哥_e18_s612.pth\",\"speaker_name\":\"家电小灯哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电老孟哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\家电老孟哥\\\\家电老孟哥_e20_s160.pth\",\"speaker_name\":\"家电老孟哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝凤翔姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝凤翔姐\\\\珠宝凤翔姐_e18_s396.pth\",\"speaker_name\":\"珠宝凤翔姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝周福姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝周福姐\\\\珠宝周福姐_e18_s504.pth\",\"speaker_name\":\"珠宝周福姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝小金哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\珠宝小金哥\\\\珠宝小金哥_e16_s512.pth\",\"speaker_name\":\"珠宝小金哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品大鹏哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品大鹏哥\\\\通品大鹏哥_e20_s400.pth\",\"speaker_name\":\"通品大鹏哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小允哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小允哥\\\\通品小允哥_e20_s1300.pth\",\"speaker_name\":\"通品小允哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小兰姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小兰姐\\\\通品小兰姐_e20_s540.pth\",\"speaker_name\":\"通品小兰姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小王哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品小王哥\\\\通品小王哥_e18_s738.pth\",\"speaker_name\":\"通品小王哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品柔柔姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品柔柔姐\\\\通品柔柔姐_e15_s705.pth\",\"speaker_name\":\"通品柔柔姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品柚子姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品柚子姐\\\\通品柚子姐_e18_s522.pth\",\"speaker_name\":\"通品柚子姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品格格姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品格格姐\\\\通品格格姐_e18_s468.pth\",\"speaker_name\":\"通品格格姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品灵林姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品灵林姐\\\\通品灵林姐_e15_s450.pth\",\"speaker_name\":\"通品灵林姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品琳琳姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品琳琳姐\\\\通品琳琳姐_e18_s2304.pth\",\"speaker_name\":\"通品琳琳姐\"},{\"config_path\":\"\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品百货姐\\\\通品百货姐_e18_s648.pth\",\"speaker_name\":\"通品百货姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品粉粉姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\通品粉粉姐\\\\通品粉粉姐_e15_s345.pth\",\"speaker_name\":\"通品粉粉姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小奇姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小奇姐\\\\鞋服小奇姐_e18_s486.pth\",\"speaker_name\":\"鞋服小奇姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小奥哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小奥哥\\\\鞋服小奥哥_e16_s192.pth\",\"speaker_name\":\"鞋服小奥哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小妮姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小妮姐\\\\鞋服小妮姐_e12_s300.pth\",\"speaker_name\":\"鞋服小妮姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小迪姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服小迪姐\\\\鞋服小迪姐_e20_s480.pth\",\"speaker_name\":\"鞋服小迪姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服恩恩姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服恩恩姐\\\\鞋服恩恩姐_e15_s480.pth\",\"speaker_name\":\"鞋服恩恩姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服悠悠姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服悠悠姐\\\\鞋服悠悠姐_e18_s702.pth\",\"speaker_name\":\"鞋服悠悠姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服毛衫姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服毛衫姐\\\\鞋服毛衫姐_e18_s738.pth\",\"speaker_name\":\"鞋服毛衫姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服百伦姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服百伦姐\\\\鞋服百伦姐_e18_s198.pth\",\"speaker_name\":\"鞋服百伦姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服美袜姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服美袜姐\\\\鞋服美袜姐_e18_s1476.pth\",\"speaker_name\":\"鞋服美袜姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服金玲姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服金玲姐\\\\鞋服金玲姐_e17_s680.pth\",\"speaker_name\":\"鞋服金玲姐\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服阿汉哥\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服阿汉哥\\\\鞋服阿汉哥_e15_s570.pth\",\"speaker_name\":\"鞋服阿汉哥\"},{\"config_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服飞飞姐\\\\verify.json\",\"model_path\":\"D:\\\\软件\\\\gpt_tts\\\\models\\\\鞋服飞飞姐\\\\鞋服飞飞姐_e18_s378.pth\",\"speaker_name\":\"鞋服飞飞姐\"}]";
        var list = JsonConvert.DeserializeObject<ModelPathInfo[]>(json);
        var index = 1;
        Debug.Assert(list != null, nameof(list) + " != null");
        return list.Select(l => new TtsModel()
        {
            Id = index++,
            Name = l.speaker_name,
            ConfigPath = l.config_path,
            ModelPath = l.model_path,
            TenantId = 1,
        }).ToList();
    }
}