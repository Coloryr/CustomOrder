using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomOrder
{
    class CustomUtils
    {
        public static Dictionary<string, CustomObj> Customs { get; set; } = new();

        private static string Local;
        public static void Start()
        {
            Local = Program.RunLocal + "Customs/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }

            foreach (var item in Directory.GetFiles(Local))
            {
                if (item.EndsWith(".json"))
                {
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<CustomObj>(File.ReadAllText(item));
                        if (obj != null)
                        {
                            if (obj.state == CustomState.wating || obj.state == CustomState.price)
                            {
                                RebotCall.now.Add(obj.qq, obj.id);
                            }
                            Customs.Add(obj.id, obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Program.Error(e);
                    }
                }
            }
        }

        public static int DateTimeNow()
        {
            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts3 = new TimeSpan(DateTime.Parse("1970-01-01").Ticks);
            TimeSpan ts_1 = ts2.Subtract(ts3).Duration();
            return (int)ts_1.TotalSeconds;
        }

        public static void SetDone(string uuid)
        {
            Customs.TryGetValue(uuid, out var obj);
            if (obj == null)
            {
                Program.robot.SendMessage(Program.Config.Admin, "找不到订单：" + uuid, 0, false);
                return;
            }
            obj.state = CustomState.done;
            if (obj.group != 0)
            {
                Program.robot.SendMessage(obj.qq, Program.Config.Text.Done.Replace("{0}", uuid), obj.group, true);
            }
            else
            {
                Program.robot.SendMessage(obj.qq, Program.Config.Text.Done.Replace("{0}", uuid), 0, false);
            }
            Program.robot.SendMessage(Program.Config.Admin, "已设置", 0, false);
            Save(uuid);
        }

        public static void SetClose(string uuid)
        {
            Customs.TryGetValue(uuid, out var obj);
            if (obj == null)
            {
                Program.robot.SendMessage(Program.Config.Admin, "找不到订单：" + uuid, 0, false);
                return;
            }
            obj.state = CustomState.close;
            if (obj.group != 0)
            {
                Program.robot.SendMessage(obj.qq, Program.Config.Text.Close.Replace("{0}", uuid), obj.group, true);
            }
            else
            {
                Program.robot.SendMessage(obj.qq, Program.Config.Text.Close.Replace("{0}", uuid), 0, false);
            }
            Program.robot.SendMessage(Program.Config.Admin, "已设置", 0, false);
            Save(uuid);
        }

        public static void Set(string uuid, bool now)
        {
            var obj = Customs[uuid];
            obj.state = now ? CustomState.ready : CustomState.deny;
            Save(uuid);
        }

        public static void SetText(string uuid, string text)
        {
            var obj = Customs[uuid];
            obj.text = text;
            obj.state = CustomState.price;
            Program.robot.SendMessage(Program.Config.Admin, "你有新的订单:\n" +
                $"ID：{uuid}\n" +
                $"QQ：{obj.qq}\n" +
                text, 0, false);
            Save(uuid);
        }

        public static void SetCost(string uuid, int cost)
        {
            var obj = Customs[uuid];
            obj.cost = cost;
            obj.state = CustomState.wating;
            if (obj.group != 0)
            {
                Program.robot.SendMessage(obj.qq, string.Format(Program.Config.Text.Cost, uuid, cost), obj.group, true);
            }
            else
            {
                Program.robot.SendMessage(obj.qq, string.Format(Program.Config.Text.Cost, uuid, cost), 0, false);
            }
            Program.robot.SendMessage(Program.Config.Admin, "已定价", 0, false);
            Save(uuid);
        }

        public static List<CustomObj> Get(long qq)
        {
            var list = from item in Customs orderby item.Value.time where item.Value.qq == qq select item.Value;
            return list.ToList();
        }

        public static List<CustomObj> Get()
        {
            var list = from item in Customs orderby item.Value.time where item.Value.state is CustomState.ready or CustomState.price select item.Value;
            return list.ToList();
        }

        public static CustomObj Get(string uuid)
        {
            if (uuid == null)
                return null;
            Customs.TryGetValue(uuid, out CustomObj obj);
            return obj;
        }

        public static CustomObj Add(long qq, long group)
        {
            while (true)
            {
                Guid guid = Guid.NewGuid();
                string uuid = guid.ToString().ToLower().Replace("-", "")[..16];
                if (!Customs.ContainsKey(uuid))
                {
                    CustomObj obj = new()
                    {
                        qq = qq,
                        id = uuid,
                        group = group,
                        state = CustomState.wating,
                        time = DateTimeNow()
                    };
                    Customs.Add(uuid, obj);
                    return obj;
                }
            }
        }

        public static void Save(string uuid)
        {
            if (Customs.TryGetValue(uuid, out var obj))
            {
                File.WriteAllText(Local + uuid + ".json",
                    JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
        }

        public static void Stop()
        {
            foreach (var item in Customs)
            {
                File.WriteAllText(Local + item.Key + ".json",
                    JsonConvert.SerializeObject(item.Value, Formatting.Indented));
            }
        }
    }
}
