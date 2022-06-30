using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibShared
{
    #region Enums
    public enum GroupBy : byte
    {
        None,
        Letter,
        CreationYear,
        ParutionYear,
    }

    public enum SortBy : byte
    {
        /// <summary>
        /// Trie l'objet par son nom.
        /// </summary>
        /// <remarks>Remarques : Ce paramètre fonctionne avec <see cref="Tlibrary.Name"/>, <see cref="Tbook.MainTitle"/></remarks>
        Name,
        /// <summary>
        /// Trie l'objet par date de création.
        /// </summary>
        /// <remarks>Remarques : Ce paramètre fonctionne avec <see cref="Tlibrary.DateAjout"/>, <see cref="Tbook.DateAjout"/></remarks>
        DateCreation,
    }

    public enum OrderBy : byte
    {
        /// <summary>
        /// Trie les éléments par ordre croissant
        /// </summary>
        Ascending,

        /// <summary>
        /// Trie les éléments par ordre décroissant
        /// </summary>
        Descending
    }

    public enum ContactType : byte
    {
        Human = 0,
        Society = 1,
    }

    public enum ContactRole : byte
    {
        Adherant = 0,
        Author = 1,
        EditorHouse = 2,
        Translator = 3,
        Illustrator = 4
    }

    public enum BookFormat : byte
    {
        Relie,
        Broche,
        Cartonne,
        Poche,
        Audio,
        Ebook,
    }

    public enum ClassificationAge : byte
    {
        ToutPublic,
        ApartirDe,
        Jusqua,
        DeTantATant,
    }

    public enum EditMode
    {
        Create,
        Edit,
    }
    #endregion

    #region List
    public class LibraryModelList
    {
        public static Dictionary<byte, string> BookFormatDictionary => new ()
        {
            { (byte)BookFormat.Relie, "Relié"},
            { (byte)BookFormat.Broche, "Broché"},
            { (byte)BookFormat.Cartonne, "Cartonné"},
            { (byte)BookFormat.Poche, "Poche"},
            { (byte)BookFormat.Audio, "Audio"},
            { (byte)BookFormat.Ebook, "Ebook"},
        };
    }
   
    #endregion
}
