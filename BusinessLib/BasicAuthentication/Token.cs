namespace BusinessLib.BasicAuthentication
{
    public struct Token : IToken
    {
        public System.String Key { get; set; }
        public System.String IP { get; set; }

        public override string ToString() { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }
    }
}
