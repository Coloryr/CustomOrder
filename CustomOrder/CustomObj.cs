namespace CustomOrder
{
    enum CustomState
    {
        ready, going, done, deny, wating, price, close
    }
    class CustomObj
    {
        public long qq { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public int cost { get; set; }
        public CustomState state { get; set; }
        public int time { get; set; }
        public long group { get; set; }
    }
}
