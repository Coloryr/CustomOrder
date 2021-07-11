using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace CustomOrder
{
    class CustomUtils
    {
        public static Dictionary<string, CustomObj> Customs { get; set; }

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
            Program.robot.SendMessage(Program.Config.Admin, "你有新的订单:\n" + text);
            Save(uuid);
        }

        public static List<CustomObj> Get(long qq)
        {
            var list = from item in Customs where item.Value.qq == qq select item.Value;
            return list.ToList();
        }

        public static CustomObj Add(long qq)
        {
            while (true)
            {
                Guid guid = Guid.NewGuid();
                string uuid = guid.ToString().ToLower();
                if (!Customs.ContainsKey(uuid))
                {
                    CustomObj obj = new()
                    {
                        qq = qq,
                        id = uuid,
                        state = CustomState.wating
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
