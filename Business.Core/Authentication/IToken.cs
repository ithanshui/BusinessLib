﻿namespace Business.Authentication
{
    public interface IToken : ISerialize
    {
        //user key
        System.String Key { get; set; }
        //front clinet
        System.String IP { get; set; }
    }
}
