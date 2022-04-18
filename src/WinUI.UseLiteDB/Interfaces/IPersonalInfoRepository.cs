using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinUI.UseLiteDB.Models;

namespace WinUI.UseLiteDB.Interfaces
{
    public interface IPersonalInfoRepository
    {
        Task BatchAddAsync(List<PersonalInfo> personalInfos, CancellationToken cancellationToken = default);
        public Task<IReadOnlyCollection<PersonalInfo>> GetListAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    }
}
