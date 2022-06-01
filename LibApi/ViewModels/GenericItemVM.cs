using AppHelpers.Serialization;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibApi.ViewModels
{
    public class GenericItemVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        public long Id { get; protected set; }
        public Guid Guid { get; protected set; } = Guid.NewGuid();

        public DateTime DateAjout { get; protected set; } = DateTime.Now;

        public DateTime? DateEdition { get; protected set; }

        protected string _Name = string.Empty;
        public string Name
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

        protected string _Description = string.Empty;
        public string Description
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

        public virtual async Task<bool> CreateAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual async Task<bool> UpdateAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual async Task<bool> DeleteAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        public virtual string? GetJsonDataStringAsync()
        {
            return JsonSerialization.SerializeClassToString(this);
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

