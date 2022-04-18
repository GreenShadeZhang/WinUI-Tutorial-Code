using CommunityToolkit.WinUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI.UseLiteDB.Helpers;
using WinUI.UseLiteDB.Models;
using WinUI.UseLiteDB.Services;

namespace WinUI.UseLiteDB.ViewModels
{
    public class MainViewModel : Observable
    {
        public MainViewModel()
        {

        }

        private ObservableCollection<PersonalInfoDto> infos = new IncrementalLoadingCollection<PersonalInfoSource,PersonalInfoDto>();

        public ObservableCollection<PersonalInfoDto> Infos 
        { 
            get 
            { 
                return infos; 
            }
            set
            {
                Set(ref infos,value);
            }
        }
    }
}
