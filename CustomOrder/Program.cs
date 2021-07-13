using System;

namespace CustomOrder
{
    class Program
    {
        public static string RunLocal { get; private set; }
        public static ConfigObj Config { get; private set; }
        public static RebotCall robot { get; private set; }

        private static Logs logs;

        static void Main()
        {
            RunLocal = AppContext.BaseDirectory;
            logs = new Logs(RunLocal);
            Config = ConfigUtils.Config(new ConfigObj()
            {
                Robot = new()
                {
                    IP = "127.0.0.1",
                    Port = 23333,
                    Time = 10000,
                    Check = false,
                    RunQQ = 0
                },
                Admin = 0,
                Command = new()
                {
                    Head = "#定制",
                    Help = "帮助",
                    Start = "开始定制",
                    Text = "定制内容",
                    My = "我的订单",
                    Now = "当前订单",
                    Confirm = "确定",
                    Refuse = "取消"
                },
                Text = new()
                {
                    Help = "定制帮助菜单\n" +
                    "#定制 开始定制 新建一个新的定制\n" +
                    "#定制 我的订单 查看我的订单\n" +
                    "#定制 当前订单 查看正在进行的订单",
                    Start = "输入[#定制 定制内容 内容]来说明定制需求",
                    Start1 = "请等待订单{0}定价",
                    Cost = "你的订单{0}已定价为{1}，输入[#定制 确定]确定订单，如果你不需要该订单请输入[#定制 取消]",
                    Start2 = "订单{0}{1}",
                    My = "我的订单:",
                    My1 = "ID：{0} 价格：{1} 状态：{2}",
                    Now = "正在进行的订单ID：{0}",
                    Last = "你的订单{0}未处理",
                    NoText = "你没有输入定制内容",
                    NoLast = "你没有待处理的订单",
                    Done = "订单{0}已完成",
                    Close = "订单{0}已被关闭"
                }
            }, RunLocal + "config.json");

            CustomUtils.Start();

            robot = new();
            robot.init();

            while (true)
            {
                string temp = Console.ReadLine();
                var arg = temp.Split(' ');
                if (arg[0] == "stop")
                {
                    CustomUtils.Stop();
                    robot.Stop();
                    return;
                }
            }
        }
        public static void Save()
        {
            ConfigUtils.Save(Config, RunLocal + "config.json");
        }

        public static void Log(string data)
        {
            logs.LogOut(data);
        }
        public static void Error(string data)
        {
            logs.LogError(data);
        }
        public static void Error(Exception e)
        {
            logs.LogError(e);
        }
    }
}
