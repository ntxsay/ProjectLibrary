using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibShared.ViewModels
{
    public abstract class GenericVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public long Id { get;  protected set; }
        public virtual Guid Guid { get;  protected set; } = Guid.NewGuid();

        public virtual DateTime DateAjout { get;  protected set; } = DateTime.Now;

        public virtual DateTime? DateEdition { get;  protected set; }

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

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
