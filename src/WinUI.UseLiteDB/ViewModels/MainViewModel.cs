using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Collections;
using WinUI.UseLiteDB.Helpers;
using WinUI.UseLiteDB.Models;
using WinUI.UseLiteDB.Services;

namespace WinUI.UseLiteDB.ViewModels;

public class MainViewModel : Observable
{
    public MainViewModel()
    {

    }

    private ObservableCollection<PersonalInfoDto> infos = new IncrementalLoadingCollection<PersonalInfoSource, PersonalInfoDto>(new PersonalInfoSource());

    public ObservableCollection<PersonalInfoDto> Infos
    {
        get
        {
            return infos;
        }
        set
        {
            Set(ref infos, value);
        }
    }
}
