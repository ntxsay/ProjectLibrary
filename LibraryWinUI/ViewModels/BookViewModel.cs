using LibShared.ViewModels.Books;
using LibShared.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWinUI.ViewModels
{
    internal sealed class BookViewModel : BookVM
    {
        private string _AuteursStringList;
        [JsonIgnore]
        public string AuteursStringList
        {
            get => _AuteursStringList;
            set
            {
                if (_AuteursStringList != value)
                {
                    _AuteursStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _TitresOeuvreStringList;
        [JsonIgnore]
        public string TitresOeuvreStringList
        {
            get => _TitresOeuvreStringList;
            set
            {
                if (_TitresOeuvreStringList != value)
                {
                    _TitresOeuvreStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CollectionVM> _Collections = new ObservableCollection<CollectionVM>();
        public ObservableCollection<CollectionVM> Collections
        {
            get => _Collections;
            set
            {
                if (_Collections != value)
                {
                    _Collections = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CollectionsStringList;
        [JsonIgnore]
        public string CollectionsStringList
        {
            get => _CollectionsStringList;
            set
            {
                if (_CollectionsStringList != value)
                {
                    _CollectionsStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _EditeursStringList;
        [JsonIgnore]
        public string EditeursStringList
        {
            get => _EditeursStringList;
            set
            {
                if (_EditeursStringList != value)
                {
                    _EditeursStringList = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
