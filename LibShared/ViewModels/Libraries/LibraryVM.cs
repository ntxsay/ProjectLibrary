namespace LibShared.ViewModels.Libraries;

public class LibraryVM : GenericVM
{
    protected Guid _Guid = System.Guid.NewGuid();
    public Guid Guid
    {
        get => _Guid;
        set
        {
            if (_Guid != value)
            {
                _Guid = value;
                OnPropertyChanged();
            }
        }
    }

    protected DateTime _DateAjout = DateTime.Now;
    public DateTime DateAjout
    {
        get => _DateAjout;
        set
        {
            if (_DateAjout != value)
            {
                _DateAjout = value;
                OnPropertyChanged();
            }
        }
    }

    protected DateTime? _DateEdition = null;
    public DateTime? DateEdition
    {
        get => _DateEdition;
        set
        {
            if (_DateEdition != value)
            {
                _DateEdition = value;
                OnPropertyChanged();
            }
        }
    }

}
