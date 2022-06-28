using Newtonsoft.Json;
using System;
namespace LibShared.ViewModels.Collections
{
	public class CollectionVM : GenericVM
	{
        public long IdLibrary { get; set; } = -1;

        protected string _Name = string.Empty;
        public virtual string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string? _Description = string.Empty;
        public virtual string? Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

