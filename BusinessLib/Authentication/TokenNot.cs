namespace BusinessLib.Authentication
{
    public struct TokenNot : ITokenNot
    {
        public System.String Site { get; set; }
        public System.String IP { get; set; }

        public override string ToString() { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }
    }
}