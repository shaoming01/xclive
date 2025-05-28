using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using Barrage.Enums;
using Barrage.Helper;
using Barrage.Models;
using Barrage.Models.Douyin;
using Barrage.Utils;
using BarrageGrab.Entity.Protobuf.Douyin;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace Barrage.GrabServices;

/// <summary>
/// Douyin barrage grab service
/// </summary>
public class DouyinBarrageGrabService : IBarrageGrabService, IDisposable
{
    private readonly Func<string, string, string> _getWssUrlFunc;

    public DouyinBarrageGrabService(Func<string, string, string> getWssUrlFunc)
    {
        _getWssUrlFunc = getWssUrlFunc;
    }

    #region Attributes & Fields

    /// <summary>
    /// User-Agent
    /// </summary>
    private string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";


    /// <summary>
    /// liveid
    /// if live url is https://live.douyin.com/751990192217
    /// so 751990192217 is the liveid
    /// </summary>
    private string LiveId = string.Empty;


    /// <summary>
    /// websocket client
    /// </summary>
    private ClientWebSocket? clientWebSocket;

    private Cookie[] _cookies = [];
    private WebRoomInfo _roomInfo;
    private Task _recieveTask;
    private Timer _heartbeatTimer;


    /// <summary>
    /// on open
    /// </summary>
    public event EventHandler<WebRoomInfo>? OnOpen;

    /// <summary>
    /// on message
    /// </summary>
    public event EventHandler<OpenBarrageMessage>? OnMessage;

    /// <summary>
    /// on error
    /// </summary>
    public event EventHandler<Exception>? OnError;

    /// <summary>
    /// on close
    /// </summary>
    public event EventHandler? OnClose;

    #endregion


    public void Start(string liveId, Cookie[]? cookies = null)
    {
        if (cookies != null && cookies.Length > 0 && !IsCookieLogin(cookies))
        {
            throw new Exception("当前传入的登录信息是无效的，请登录后再重试");
        }

        _cookies = cookies ?? [];


        LiveId = liveId;
        var roomInfo = GetRoomInfo();
        if (roomInfo == null)
        {
            throw new Exception("直播间信息获取失败");
        }

        if (roomInfo.status != "2")
        {
            OnOpen?.Invoke(this, roomInfo);
            OnClose?.Invoke(this, null);
            return;
        }
        
        var wssUrl = _getWssUrlFunc(roomInfo.roomId, roomInfo.uniqueId);
        if (string.IsNullOrEmpty(wssUrl))
        {
            throw new Exception("直播间信息获取失败");
        }

        _roomInfo = roomInfo;

        ConnectWss(wssUrl, roomInfo.ttwid);
    }

    private bool IsCookieLogin(Cookie[] cookies)
    {
        if (cookies == null || cookies.Length == 0)
        {
            return false;
        }

        return cookies.Any(c => c.Name == "passport_fe_beating_status" && !string.IsNullOrEmpty(c.Value));
    }

    private WebRoomInfo? GetRoomInfo()
    {
        try
        {
            var url = $"https://live.douyin.com/{LiveId}";
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            var h = new HttpClient(handler);
            h.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            var cookieVal = _cookies.Length == 0 ? $"__ac_nonce=0{GenerateMsToken(20)}" : GetCookieStr();
            h.DefaultRequestHeaders.Add("Cookie", cookieVal);
            var resp = h.GetAsync(url).Result;
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception("读取直接间页面出错:" + resp.Content.ReadAsStringAsync().Result);
            }

            var html = resp.Content.ReadAsStringAsync().Result;
            var roomRe = AnalyzeRoomId(html);
            if (!roomRe.Success)
            {
                Log4.Log.Error("解析直播间页面出错:" + roomRe.Message);
                return null;
            }

            var cookie = cookieContainer.GetAllCookies().FirstOrDefault(c => c.Name == "ttwid");
            var twid = cookie?.Value;
            if (string.IsNullOrEmpty(twid))
            {
                Log4.Log.Error("解析直播间twid出错:" + roomRe.Message);
                return null;
            }

            roomRe.Data.ttwid = twid;
            return roomRe.Data;
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
        }

        return null;
    }

    private string GetCookieStr()
    {
        return string.Join("; ", _cookies.Select(c => c.Name + "=" + c.Value));
    }

    private static R<WebRoomInfo> AnalyzeRoomId(string html)
    {
        // 正则匹配目标数据
        Match match = Regex.Match(html,
            @"\{\\""state.*?</script>");

        if (!match.Success)
        {
            return R.Faild<WebRoomInfo>("房间信息解析失败");
        }

        // 获取目标信息编码字符串
        string json = match.Groups[0].Value;

        // 替换转义字符
        // 替换规则列表，与原 JS 中 REGLIST 相同
        var regexReplacements = new (string pattern, string replacement)[]
        {
            // 替换 1 到 7 个反斜杠后跟双引号为单个双引号
            ("\\\\{1,7}\"", "\""),
            // 替换双引号后紧跟左花括号为左花括号
            ("\"\\{", "{"),
            // 替换右花括号后紧跟双引号为右花括号
            ("\\}\"", "}"),
            // 替换双引号后紧跟左中括号为左中括号
            ("\"\\[", "["),
            // 替换右中括号后紧跟双引号为右中括号
            ("\\]\"", "]")
        };

        foreach (var rep in regexReplacements)
        {
            json = Regex.Replace(json, rep.pattern, rep.replacement);
        }

        json = json.Replace("]\\n\"])</script>", "");

        // 解析 JSON，注意可能需要处理编码问题（如 decodeURIComponent 等），此处假设 json 已经是标准格式
        JObject dict = JObject.Parse(json);

        // 提取各项数据，使用 SelectToken 方法方便路径查找
        string roomId = dict.SelectToken("state.roomStore.roomInfo.roomId")?.ToString();
        string roomTitle = dict.SelectToken("state.roomStore.roomInfo.room.title")?.ToString();
        string roomUserCount = dict.SelectToken("state.roomStore.roomInfo.room.user_count_str")?.ToString();
        string status = dict.SelectToken("state.roomStore.roomInfo.room.status")?.ToString();
        string uniqueId = dict.SelectToken("state.userStore.odin.user_unique_id")?.ToString();
        string avatar = dict.SelectToken("state.roomStore.roomInfo.anchor.avatar_thumb.url_list[0]")?.ToString();
        return R.OK(new WebRoomInfo
        {
            roomTitle = roomTitle,
            roomId = roomId,
            roomUserCount = roomUserCount,
            status = status,
            uniqueId = uniqueId,
            avatar = avatar,
        });
    }

    public void Stop()
    {
        try
        {
            clientWebSocket?.Abort();
            clientWebSocket?.Dispose();
            clientWebSocket = null;
        }
        catch (Exception ex)
        {
            // do something
        }
    }

    public void ReStart()
    {
        this.Stop();

        this.Start(LiveId);
    }


    private void ConnectWss(string wssUrl, string ttwid)
    {
        clientWebSocket = new ClientWebSocket();
        var cookieStr = _cookies.Length == 0 ? $"ttwid={ttwid}" : GetCookieStr();
        //cookieStr = $"ttwid={_cookies.First(c => c.Name == "ttwid").Value}";
        clientWebSocket.Options.SetRequestHeader("cookie", cookieStr);
        clientWebSocket.Options.SetRequestHeader("user-agent", UserAgent);


        _recieveTask = Task.Run(async () =>
        {
            try
            {
                //connect
                await clientWebSocket.ConnectAsync(new Uri(wssUrl), CancellationToken.None);

                if (clientWebSocket.State != WebSocketState.Open &&
                    clientWebSocket.State != WebSocketState.Connecting)
                {
                    throw new Exception("连接服务器失败");
                }

                OnOpen?.Invoke(clientWebSocket, _roomInfo);


                #region 发送hb心跳

                try
                {
                    byte[] heartbeat = new byte[] { 0x3a, 0x02, 0x68, 0x62 };
                    await clientWebSocket.SendAsync(new ArraySegment<byte>(heartbeat), WebSocketMessageType.Binary,
                        true, CancellationToken.None);

                    _heartbeatTimer = new System.Timers.Timer(10000);
                    _heartbeatTimer.Enabled = true;
                    _heartbeatTimer.Start();
                    _heartbeatTimer.Elapsed += (sender, e) =>
                    {
                        try
                        {
                            clientWebSocket?.SendAsync(new ArraySegment<byte>(heartbeat),
                                WebSocketMessageType.Binary, true, CancellationToken.None);
                        }
                        catch (Exception exception)
                        {
                            if (exception is ObjectDisposedException)
                            {
                                _heartbeatTimer.Stop();
                                OnClose?.Invoke(this, null);
                            }
                            else
                            {
                                Log4.Log.Error(exception);
                                OnError?.Invoke(this, exception);
                            }
                        }
                    };
                }
                catch (Exception ex)
                {
                    Log4.Log.Error(ex);
                    OnError?.Invoke(this, ex);
                }

                #endregion


                //缓冲写大一些，就不用while循环分多次取
                byte[] buffer = new byte[1024 * 10000];

                //监听Socket信息，接收连接的套接字发来的数据
                WebSocketReceiveResult result =
                    await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                //如果没有关闭，就一直接收
                while (!result.CloseStatus.HasValue)
                {
                    #region 处理消息

                    //将接收到的数据写到缓冲里
                    var package = PushFrame.Parser.ParseFrom(new MemoryStream(buffer, 0, result.Count));
                    var response =
                        Response.Parser.ParseFrom(DecompressHelper.Decompress(package.Payload.ToArray()));

                    #region if NeedAck

                    if (response.NeedAck)
                    {
                        PushFrame ack = new PushFrame()
                        {
                            LogId = package.LogId,
                            PayloadType = "ack",
                            Payload = ByteString.CopyFromUtf8(response.InternalExt)
                        };

                        await clientWebSocket.SendAsync(new ArraySegment<byte>(ack.ToByteString().ToArray()),
                            WebSocketMessageType.Binary, true, CancellationToken.None);
                    }

                    #endregion

                    #region 处理消息数据（这里只是写个例子）

                    if (response.MessagesList != null && response.MessagesList.Count > 0)
                    {
                        foreach (var message in response.MessagesList)
                        {
                            switch (message.Method)
                            {
                                #region WebcastMemberMessage 进入

                                case "WebcastMemberMessage":
                                {
                                    MemberMessage memberMsg = MemberMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Member,
                                        Data = new DouyinMsgMember()
                                        {
                                            MsgId = (long)memberMsg.Common.MsgId,
                                            Content = $"来了",
                                            RoomId = (long)memberMsg.Common.RoomId,
                                            MemberCount = (long)memberMsg.MemberCount,
                                            User = GetUser(memberMsg.User)
                                        }
                                    };
                                    RecievedMessage(obm);
                                    break;
                                }

                                #endregion

                                #region WebcastSocialMessage 关注 & 分享

                                case "WebcastSocialMessage":
                                {
                                    SocialMessage socialMessage = SocialMessage.Parser.ParseFrom(message.Payload);

                                    #region 分享

                                    if (socialMessage.Action == 3)
                                    {
                                        OpenBarrageMessage obm = new OpenBarrageMessage()
                                        {
                                            Type = MessageTypeEnum.Share,
                                            Data = new DouyinMsgShare()
                                            {
                                                MsgId = (long)socialMessage.Common.MsgId,
                                                Content =
                                                    $"分享了直播间到{socialMessage.ShareTarget}",
                                                RoomId = (long)socialMessage.Common.RoomId,
                                                //ShareType = socialMessage.ShareTarget,
                                                User = GetUser(socialMessage.User),
                                                ShareTarget = socialMessage.ShareTarget,
                                            }
                                        };
                                        RecievedMessage(obm);
                                    }

                                    #endregion

                                    #region 关注

                                    else
                                    {
                                        OpenBarrageMessage obm = new OpenBarrageMessage()
                                        {
                                            Type = MessageTypeEnum.Social,
                                            Data = new DouyinMsgSocial()
                                            {
                                                MsgId = (long)socialMessage.Common.MsgId,
                                                Content = $"关注了主播",
                                                RoomId = (long)socialMessage.Common.RoomId,
                                                User = GetUser(socialMessage.User)
                                            }
                                        };
                                        RecievedMessage(obm);
                                    }

                                    #endregion

                                    break;
                                }

                                #endregion

                                #region WebcastChatMessage 弹幕

                                case "WebcastChatMessage":
                                {
                                    ChatMessage chatMessage = ChatMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Chat,
                                        Data = new DouyinMsgChat()
                                        {
                                            MsgId = (long)chatMessage.Common.MsgId,
                                            Content = chatMessage.Content,
                                            RoomId = (long)chatMessage.Common.RoomId,
                                            User = GetUser(chatMessage.User)
                                        }
                                    };

                                    RecievedMessage(obm);

                                    break;
                                }

                                #endregion

                                #region WebcastLikeMessage 点赞

                                case "WebcastLikeMessage":
                                {
                                    LikeMessage likeMessage = LikeMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Like,
                                        Data = new DouyinMsgLike()
                                        {
                                            MsgId = (long)likeMessage.Common.MsgId,
                                            Count = (long)likeMessage.Count,
                                            Total = (long)likeMessage.Total,
                                            Content =
                                                $"为主播点了{likeMessage.Count.ToString()}个赞，总点赞{likeMessage.Total.ToString()}",
                                            RoomId = (long)likeMessage.Common.RoomId,
                                            User = GetUser(likeMessage.User)
                                        }
                                    };

                                    RecievedMessage(obm);

                                    break;
                                }

                                #endregion

                                #region WebcastGiftMessage 礼物

                                case "WebcastGiftMessage":
                                {
                                    GiftMessage giftMessage = GiftMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Gift,
                                        Data = new DouyinMsgGift()
                                        {
                                            MsgId = (long)giftMessage.Common.MsgId,
                                            GiftId = (long)giftMessage.GiftId,
                                            GiftName = giftMessage.Gift.Name,
                                            TotalCount = (long)giftMessage.TotalCount,
                                            RepeatCount = (long)giftMessage.RepeatCount,
                                            RepeatEnd = (int)giftMessage.RepeatEnd,
                                            ComboCount = (long)giftMessage.ComboCount,
                                            GroupCount = (long)giftMessage.GroupCount,
                                            DiamondCount = (int)giftMessage.Gift.DiamondCount,
                                            Content =
                                                $"送出 {giftMessage.Gift.Name}{(giftMessage.Gift.Combo ? "(可连击)" : "")} x {giftMessage.RepeatCount}个", //，增量{count}个
                                            RoomId = (long)giftMessage.Common.RoomId,
                                            User = GetUser(giftMessage.User),
                                            ToUser = GetUser(giftMessage.ToUser)
                                        }
                                    };
                                    RecievedMessage(obm);


                                    break;
                                }

                                #endregion

                                #region WebcastRoomUserSeqMessage 统计

                                case "WebcastRoomUserSeqMessage":
                                {
                                    RoomUserSeqMessage roomUserSeqMessage =
                                        RoomUserSeqMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.RoomUserSeq,
                                        Data = new DouyinMsgRoomUserSeq()
                                        {
                                            MsgId = (long)roomUserSeqMessage.Common.MsgId,
                                            OnlineUserCount = roomUserSeqMessage.Total,
                                            TotalUserCount = roomUserSeqMessage.TotalUser,
                                            TotalUserCountStr = roomUserSeqMessage.TotalPvForAnchor,
                                            OnlineUserCountStr = roomUserSeqMessage.OnlineUserForAnchor,
                                            Content =
                                                $"当前直播间人数 {roomUserSeqMessage.OnlineUserForAnchor}，累计直播间人数 {roomUserSeqMessage.TotalPvForAnchor}",
                                            RoomId = (long)roomUserSeqMessage.Common.RoomId,
                                            User = null,
                                        }
                                    };
                                    RecievedMessage(obm);


                                    break;
                                }

                                #endregion

                                #region WebcastControlMessage 直播间状态变更

                                case "WebcastControlMessage":
                                {
                                    ControlMessage controlMessage =
                                        ControlMessage.Parser.ParseFrom(message.Payload);

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Control,
                                        Data = new DouyinMsgControl()
                                        {
                                            MsgId = (long)controlMessage.Common.MsgId,
                                            Content = controlMessage.Status == 3 ? "直播已结束" : "",
                                            RoomId = (long)controlMessage.Common.RoomId,
                                            User = null
                                        }
                                    };
                                    RecievedMessage(obm);


                                    break;
                                }

                                #endregion

                                #region WebcastFansclubMessage 粉丝团

                                case "WebcastFansclubMessage":
                                {
                                    FansclubMessage fansclubMessage =
                                        FansclubMessage.Parser.ParseFrom(message.Payload);

                                    DouyinMsgFansClub douyinMsgFansClub = new DouyinMsgFansClub()
                                    {
                                        MsgId = (long)fansclubMessage.CommonInfo.MsgId,
                                        Type = fansclubMessage.Type,
                                        Content = fansclubMessage.Content,
                                        RoomId = (long)fansclubMessage.CommonInfo.RoomId,
                                        User = GetUser(fansclubMessage.User)
                                    };

                                    if (douyinMsgFansClub.User != null && douyinMsgFansClub.User.FansClub != null)
                                    {
                                        douyinMsgFansClub.Level = douyinMsgFansClub.User.FansClub.Level;
                                    }

                                    OpenBarrageMessage obm = new OpenBarrageMessage()
                                    {
                                        Type = MessageTypeEnum.Fansclub,
                                        Data = douyinMsgFansClub
                                    };
                                    RecievedMessage(obm);


                                    break;
                                }

                                #endregion

                                default:
                                    break;
                            }
                        }
                    }

                    #endregion

                    #endregion


                    //keep receive
                    if (clientWebSocket.State == WebSocketState.Open)
                    {
                        result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                            CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(clientWebSocket, ex);
                Log4.Log.Error(ex);
            }
        });
    }

    private void RecievedMessage(OpenBarrageMessage obm)
    {
        Console.WriteLine(JsonConvert.SerializeObject(obm.Data));
        OnMessage?.Invoke(clientWebSocket, obm);
    }


    #region 方法

    #region GenerateMsToken

    static Random random = new Random();

    private string GenerateMsToken(int length = 107)
    {
        string baseStr = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789=_";

        StringBuilder str = new StringBuilder();

        int len = baseStr.Length;
        for (int i = 0; i < length; i++)
        {
            str.Append(baseStr[random.Next(0, len)]);
        }

        return str.ToString();
    }

    #endregion


    #region private DouyinUser? GetUser(User data)

    private DouyinUser? GetUser(User data)
    {
        if (data == null)
        {
            return null;
        }

        DouyinUser user = new DouyinUser()
        {
            DisplayId = data.DisplayId,
            ShortId = (long)data.ShortId,
            Gender = (int)data.Gender,
            Id = (long)data.Id,
            Level = (int)data.Level,
            PayLevel = (int)(data.PayGrade?.Level ?? -1),
            NickName = data.NickName ?? "用户" + data.DisplayId,
            Avatar = data.AvatarThumb?.UrlListList?.FirstOrDefault() ?? "",
            SecUid = data.SecUid,
            FollowerCount = (long)(data.FollowInfo?.FollowerCount ?? 0),
            FollowingCount = (long)(data.FollowInfo?.FollowingCount ?? 0),
            FollowStatus = (long)(data.FollowInfo?.FollowStatus ?? 0)
        };

        if (data.FansClub != null && data.FansClub.Data != null)
        {
            user.FansClub = new DouyinFansClub()
            {
                ClubName = data.FansClub.Data.ClubName,
                Level = data.FansClub.Data.Level
            };
        }

        return user;
    }

    #endregion

    #endregion

    public void Dispose()
    {
        try
        {
            _heartbeatTimer?.Dispose();
            clientWebSocket?.Dispose();
            _recieveTask?.Dispose();
        }
        catch (Exception e)
        {
        }
    }
}

public class WebRoomInfo
{
    #region HTML中解析出来

    public string roomId { get; set; }
    public string roomTitle { get; set; }

    /// <summary>
    /// 2，正在播，4停止
    /// </summary>
    public string status { get; set; }

    public string roomUserCount { get; set; }

    public string uniqueId { get; set; }
    public string avatar { get; set; }

    #endregion

    /// <summary>
    /// COOKIE里解析得到
    /// </summary>
    public string ttwid { get; set; }
}