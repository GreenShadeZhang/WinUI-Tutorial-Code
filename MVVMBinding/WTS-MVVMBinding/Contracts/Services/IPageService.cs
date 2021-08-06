using System;

namespace WTS_MVVMBinding.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
