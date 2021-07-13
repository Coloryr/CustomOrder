using ColoryrSDK;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CustomOrder
{
    class RebotCall
    {
        public static Dictionary<long, string> now = new();

        private Robot robot = new();
        private void Message(byte type, string data)
        {
            switch (type)
            {
                case 50:
                    {
                        var pack = JsonConvert.DeserializeObject<TempMessageEventPack>(data);
                        string temp = pack.message[^1];
                        Message(pack.fid, pack.id, temp, true);
                        break;
                    }
                case 51:
                    {
                        var pack = JsonConvert.DeserializeObject<FriendMessageEventPack>(data);
                        string temp = pack.message[^1];
                        Message(pack.id, 0, temp, false);
                        break;
                    }
            }
        }
        private void Message(long qq, long group, string message, bool s)
        {
            if (message.StartsWith(Program.Config.Command.Head))
            {
                string[] arg;
                if (message == Program.Config.Command.Head)
                {
                    arg = message[Program.Config.Command.Head.Length..].Split(' ');
                }
                else
                {
                    arg = message[(Program.Config.Command.Head.Length + 1)..].Split(' ');
                }
                if (qq != Program.Config.Admin)
                {
                    if (arg[0] == Program.Config.Command.Help)
                    {
                        SendMessage(qq, Program.Config.Text.Help, group, s);
                    }
                    else if (arg[0] == Program.Config.Command.Start)
                    {
                        if (now.ContainsKey(qq))
                        {
                            SendMessage(qq, Program.Config.Text.Last.Replace("{0}", now[qq]), group, s);
                            return;
                        }
                        var obj = CustomUtils.Add(qq, group);
                        now.Add(qq, obj.id);
                        SendMessage(qq, Program.Config.Text.Start, group, s);
                    }
                    else if (arg[0] == Program.Config.Command.Text)
                    {
                        if (!now.ContainsKey(qq))
                        {
                            SendMessage(qq, Program.Config.Text.NoLast, group, s);
                            return;
                        }
                        if (arg.Length <= 1)
                        {
                            SendMessage(qq, Program.Config.Text.NoText, group, s);
                            return;
                        }
                        string data1 = "";
                        for (int a = 1; a < arg.Length; a++)
                        {
                            data1 += arg[a];
                        }
                        CustomUtils.SetText(now[qq], data1);
                        SendMessage(qq, string.Format(Program.Config.Text.Start1, now[qq]), group, s);
                    }
                    else if (arg[0] == Program.Config.Command.Confirm)
                    {
                        if (!now.ContainsKey(qq))
                        {
                            SendMessage(qq, Program.Config.Text.NoLast, group, s);
                            return;
                        }
                        SendMessage(qq, string.Format(Program.Config.Text.Start2, now[qq], "已确认"), group, s);
                        CustomUtils.Set(now[qq], true);
                        now.Remove(qq);
                    }
                    else if (arg[0] == Program.Config.Command.Refuse)
                    {
                        if (!now.ContainsKey(qq))
                        {
                            SendMessage(qq, Program.Config.Text.NoLast, group, s);
                            return;
                        }
                        SendMessage(qq, string.Format(Program.Config.Text.Start2, now[qq], "已取消"), group, s);
                        CustomUtils.Set(now[qq], false);
                        now.Remove(qq);
                    }
                    else if (arg[0] == Program.Config.Command.My)
                    {
                        var list = CustomUtils.Get(qq);
                        if (list.Count == 0)
                        {
                            SendMessage(qq, Program.Config.Text.My + "无", group, s);
                        }
                        else
                        {
                            string temp1 = Program.Config.Text.My + "\n";
                            foreach (var item in list)
                            {
                                temp1 += string.Format(Program.Config.Text.My1, item.id, item.cost, item.state) + "\n";
                            }
                            temp1 = temp1[..^1];
                            SendMessage(qq, temp1, group, s);
                        }
                    }
                    else if (arg[0] == Program.Config.Command.Now)
                    {
                        SendMessage(qq, Program.Config.Text.Now.Replace("{0}",
                            Program.Config.NowCustom), group, s);
                    }
                    else
                    {
                        SendMessage(qq, Program.Config.Text.Help, group, s);
                    }
                }
                else
                {
                    if (message.StartsWith(Program.Config.Command.Head))
                    {
                        if (arg[0] == "help")
                        {
                            if (arg.Length != 3)
                            {
                                SendMessage(qq, "cost id cost 设置订单定价", group, s);
                            }
                        }
                        else if (arg[0] == "cost")
                        {
                            if (arg.Length != 3)
                            {
                                SendMessage(qq, "错误的参数", group, s);
                            }
                            var obj = CustomUtils.Get(arg[1]);
                            if (obj == null)
                            {
                                SendMessage(qq, "不存在的订单", group, s);
                            }
                            if (!int.TryParse(arg[2], out int cost))
                            {
                                SendMessage(qq, "错误的数字", group, s);
                            }
                            CustomUtils.SetCost(arg[1], cost);
                        }
                        else if (arg[0] == "list")
                        {
                            var obj = CustomUtils.Get();
                            if (obj.Count == 0)
                            {
                                SendMessage(qq, "订单表为空", group, s);
                            }
                            else
                            {
                                string a = "";
                                foreach (var item in obj)
                                {
                                    a += $"ID：{item.id} Cost：{item.cost} QQ：{item.qq} State：{item.state}\n";
                                }
                                a = a[..^1];
                                SendMessage(qq, a, group, s);
                            }
                        }
                        else if (arg[0] == "now")
                        {
                            var obj = CustomUtils.Get(Program.Config.NowCustom);
                            if (obj == null)
                            {
                                SendMessage(qq, "没有当前定制", group, s);
                            }
                            else
                            {
                                string a = $"ID：{obj.id} Cost：{obj.cost} QQ：{obj.qq} State：{obj.state}\n{obj.text}";
                                SendMessage(qq, a, group, s);
                            }
                        }
                        else if (arg[0] == "setnow")
                        {
                            if (arg.Length != 2)
                            {
                                SendMessage(qq, "错误的参数", group, s);
                            }
                            else
                            {
                                SendMessage(qq, "已设置", group, s);
                                Program.Config.NowCustom = arg[1];
                                Program.Save();
                            }
                        }
                        else if (arg[0] == "done")
                        {
                            if (arg.Length != 2)
                            {
                                SendMessage(qq, "错误的参数", group, s);
                            }
                            else
                            {
                                CustomUtils.SetDone(arg[1]);
                            }
                        }
                        else if (arg[0] == "close")
                        {
                            if (arg.Length != 2)
                            {
                                SendMessage(qq, "错误的参数", group, s);
                            }
                            else
                            {
                                CustomUtils.SetClose(arg[1]);
                            }
                        }
                    }
                }
            }
        }

        public void SendMessage(long qq, string message, long group, bool s)
        {
            byte[] pack1;
            if (s)
            {
                pack1 = BuildPack.Build(new SendGroupPrivateMessagePack()
                {
                    id = group,
                    fid = qq,
                    message = new()
                    {
                        message
                    }
                }, 53);
            }
            else
            {
                pack1 = BuildPack.Build(new SendFriendMessagePack()
                {
                    id = qq,
                    message = new()
                    {
                        message
                    }
                }, 54);
            }
            robot.AddTask(pack1);
        }

        private void Log(LogType type, string data)
        {

            Program.Log($"{type} {data}");
        }

        private void State(StateType type)
        {
            Program.Log($"{type}");
        }

        public void init()
        {
            RobotConfig config = new()
            {
                IP = Program.Config.Robot.IP,
                Port = Program.Config.Robot.Port,
                Name = "CustomOrder",
                Pack = new() { 50, 51 },
                RunQQ = Program.Config.Robot.RunQQ,
                Time = Program.Config.Robot.Time,
                Check = Program.Config.Robot.Check,
                CallAction = Message,
                LogAction = Log,
                StateAction = State
            };

            robot.Set(config);
            robot.Start();

        }

        public void Stop()
        {
            robot.Stop();
        }
    }
}
