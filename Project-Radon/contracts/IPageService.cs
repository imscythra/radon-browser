using System;
using System.Collections.Generic;

namespace Project_Radon.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
        Dictionary<string, Type> Pages { get; }
    }
}
