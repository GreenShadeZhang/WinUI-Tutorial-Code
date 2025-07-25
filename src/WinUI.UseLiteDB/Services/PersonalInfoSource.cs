using CommunityToolkit.Common.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinUI.UseLiteDB.Models;
using CommunityToolkit.WinUI.Collections;

namespace WinUI.UseLiteDB.Services;

public class PersonalInfoSource : CommunityToolkit.WinUI.Collections.IIncrementalSource<PersonalInfoDto>
{
    public async Task<IEnumerable<PersonalInfoDto>> GetPagedItemsAsync(
        int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var data = await App.Repository.GetListAsync(pageIndex, pageSize, cancellationToken);

        var dtos = new List<PersonalInfoDto>();

        if (data != null && data.Count > 0)
        {
            foreach (var item in data)
            {
                var tempData = new PersonalInfoDto
                {
                    Name = item.Name,
                    Desc = item.Desc,
                    Tags = item.Tags,
                    Hobbies = item.Hobbies
                };

                if (item.AvatarStream != null)
                {
                    var bitmapImage = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();

                    await bitmapImage.SetSourceAsync(item.AvatarStream.AsRandomAccessStream());

                    tempData.AvatarBitmap = bitmapImage;
                }

                dtos.Add(tempData);
            }
        }

        return dtos.AsEnumerable();
    }
}
