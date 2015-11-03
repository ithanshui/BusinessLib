using BusinessLib.Mark;

namespace BusinessLib.Result
{
    /*
    public class ResultRedis<ICache> : ResultBase<ICache, string, object, System.Collections.Generic.KeyValuePair<string, object>>
        where ICache : class, BusinessLib.Cache.ICache, new()
    {
        public ResultRedis(int state)
            : base(state, string.Format("{0}_{1}", MarkBase.GetObject<string>(MarkEnum.Result_State), state)) { }

        public ResultRedis(string messag) : base(messag) { }

        public ResultRedis(int state = 1, object messag = null, object data = null) : base(state, messag, data) { }
    }
    */
}
