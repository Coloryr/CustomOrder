using ColoryrSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                    }
                    break;
                case 51:
                    {
                        var pack = JsonConvert.DeserializeObject<FriendMessageEventPack>(data);
                        string temp = pack.message[^1];
                        if (temp.StartsWith(Program.Config.Command.Head))
                        {
                            var arg = temp[Program.Config.Command.Head.Length..].Split(' ');
                            if (pack.id != Program.Config.Admin)
                            {
                                if (arg[0] == Program.Config.Command.Help)
                                {
                                    SendMessage(pack.id, Program.Config.Text.Help);
                                }
                                else if (arg[0] == Program.Config.Command.Start)
                                {
                                    if (now.ContainsKey(pack.id))
                                    {
                                        SendMessage(pack.id, Program.Config.Text.Last);
                                        return;
                                    }
                                    var obj = CustomUtils.Add(pack.id);
                                    now.Add(pack.id, obj.id);
                                    SendMessage(pack.id, Program.Config.Text.Start);
                                }
                                else if (arg[0] == Program.Config.Command.Text)
                                {
                                    if (!now.ContainsKey(pack.id))
                                    {
                                        SendMessage(pack.id, Program.Config.Text.NoLast);
                                        return;
                                    }
                                    if (arg.Length <= 1)
                                    {
                                        SendMessage(pack.id, Program.Config.Text.NoText);
                                        return;
                                    }
                                    string data1 = "";
                                    for (int a = 1; a < arg.Length; a++)
                                    {
                                        data1 += arg[a];
                                    }
                                    CustomUtils.SetText(now[pack.id], data1);
                                    SendMessage(pack.id, string.Format(Program.Config.Text.Start1, now[pack.id]));
                                }
                                else if (arg[0] == Program.Config.Command.Confirm)
                                {
                                    if (!now.ContainsKey(pack.id))
                                    {
                                        SendMessage(pack.id, Program.Config.Text.NoLast);
                                        return;
                                    }
                                    SendMessage(pack.id, string.Format(Program.Config.Text.Start2, now[pack.id], "已确认"));
                                    CustomUtils.Set(now[pack.id], true);
                                    now.Remove(pack.id);
                                }
                                else if (arg[0] == Program.Config.Command.Refuse)
                                {
                                    if (!now.ContainsKey(pack.id))
                                    {
                                        SendMessage(pack.id, Program.Config.Text.NoLast);
                                        return;
                                    }
                                    SendMessage(pack.id, string.Format(Program.Config.Text.Start2, now[pack.id], "已取消"));
                                    CustomUtils.Set(now[pack.id], false);
                                    now.Remove(pack.id);
                                }
                                else if (arg[0] == Program.Config.Command.My)
                                {
                                    var list = CustomUtils.Get(pack.id);
                                    if (list.Count == 0)
                                    {
                                        SendMessage(pack.id, Program.Config.Text.My + "无");
                                    }
                                    else
                                    {
                                        string temp1 = Program.Config.Text.My + "\n";
                                        foreach (var item in list)
                                        {
                                            temp1 += item.id + "\n";
                                        }
                                        temp1 = temp1[..^1];
                                        SendMessage(pack.id, temp1);
                                    }
                                }
                                else if (arg[0] == Program.Config.Command.Now)
                                {
                                    SendMessage(pack.id, Program.Config.Text.Now.Replace("{0}",
                                        Program.Config.NowCustom));
                                }
                                else
                                {
                                    SendMessage(pack.id, "未知指令");
                                }
                            }
                            else
                            {
                                if (temp.StartsWith(Program.Config.Command.Head))
                                {
                                    if (arg[0] == "help")
                                    {
                                        if (arg.Length != 3)
                                        {
                                            SendMessage(pack.id, "cost id cost 设置订单定价");
                                        }
                                    }
                                    else if (arg[0] == "cost")
                                    {
                                        if (arg.Length != 3)
                                        {
                                            SendMessage(pack.id, "错误的参数");
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        public void SendMessage(long qq, string message)
        {
            var pack1 = BuildPack.Build(new SendFriendMessagePack()
            {
                id = qq,
                message = new()
                {
                    message
                }
            }, 54);
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
