using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibApi.Models.Local.SQLite;
namespace LibApi
{
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
        Croissant,
        DCroissant
    }
}
