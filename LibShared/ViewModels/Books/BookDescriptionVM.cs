using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LibShared.ViewModels.Books
{
    public class BookDescriptionVM : GenericVM
    {
        private string? _Resume;

        [DisplayName("Résumé")]
        public string? Resume
        {
            get => _Resume;
            set
            {
                if (_Resume != value)
                {
                    _Resume = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _Notes;
        public string? Notes
        {
            get => _Notes;
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

