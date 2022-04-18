using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using WinUI.UseLiteDB.Interfaces;
using WinUI.UseLiteDB.Models;

namespace WinUI.UseLiteDB.Repository
{
    public class PersonalInfoRepository : IPersonalInfoRepository
    {
        private readonly LiteDatabase _liteDatabase;
        public PersonalInfoRepository(string dbDataPath)
        {
            _liteDatabase = new LiteDatabase(dbDataPath);
        }

        public async Task BatchAddAsync(List<PersonalInfo> personalInfos, CancellationToken cancellationToken = default)
        {
            var infos = _liteDatabase.GetCollection<PersonalInfo>();

            infos.InsertBulk(personalInfos);

            if (personalInfos != null && personalInfos.Count > 0)
            {
                var fs = _liteDatabase.GetStorage<string>("dataFiles", "dataChunks");

                foreach (var item in personalInfos)
                {
                    var pathFull = $"{Package.Current.InstalledLocation.Path}\\Assets\\Logo\\{item.AvatarName}";

                    if (!fs.Exists($"$/Data/{item.AvatarName}"))
                    {
                        var ret = await Package.Current.InstalledLocation.GetItemAsync($"Assets\\Logo\\{item.AvatarName}");

                        if (ret != null)
                        {
                            fs.Upload($"$/Data/{item.AvatarName}", pathFull);
                        }
                    }

                }
            }
        }

        public Task<IReadOnlyCollection<PersonalInfo>> GetListAsync(
            int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _liteDatabase.GetCollection<PersonalInfo>().Query();

            var list = query
                .OrderByDescending(p => p.Name)
                .Skip((pageIndex) * pageSize)
                .Limit(pageSize)
                .ToList();


            if (list != null && list.Count > 0)
            {
                var fs = _liteDatabase.GetStorage<string>("dataFiles", "dataChunks");

                foreach (var item in list)
                {
                    if (fs.Exists($"$/Data/{item.AvatarName}"))
                    {
                        LiteFileInfo<string> file = fs.FindById($"$/Data/{item.AvatarName}");

                        Stream stream = new MemoryStream();

                        fs.Download(file.Id, stream);

                        stream.Seek(0, SeekOrigin.Begin);

                        item.AvatarStream = stream;
                    }
                }
            }

            return Task.FromResult((IReadOnlyCollection<PersonalInfo>)list);
        }
    }
}
