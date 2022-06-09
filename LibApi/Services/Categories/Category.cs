using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Categories
{
    public sealed class Category : CategoryVM, IDisposable
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }
        LibrarySqLiteDbContext context = new ();

        private Category()
        {

        }

        #region CRUD
        /// <summary>
        /// Met à jour la catégorie dans la base de données
        /// </summary>
        /// <remarks>Remarque : si aucun paramètre n'est renseigné alors une <see cref="InvalidOperationException"/> est levée et annule ainsi l'opération de mise à jour.</remarks>
        /// <param name="newName"></param>
        /// <param name="newDescription">Nouvelle description. Si le paramètre est null alors la modification de ce paramètre est ingoré.</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string? newName, string? newDescription = null)
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La catégorie {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de la catégorie ou sa nouvelle description doivent être renseignées.");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentException(nameof(record), $"La catégorie n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.Tcollections.AnyAsync(c => c.Id != Id && c.Name.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        throw new ArgumentException($"Cette catégorie existe déjà.");
                    }
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    record.Name = newName.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    record.Description = newDescription.Trim();
                }

                context.TlibraryCategories.Update(record);
                _ = await context.SaveChangesAsync();

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    Name = record.Name;
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    Description = record.Description;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(UpdateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Supprime la catégorie de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException($"La catégorie {Name} a déjà été supprimée.");
                }

                TlibraryCategorie? record = await context.TlibraryCategories.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new ArgumentNullException(nameof(Tcollection), $"La catégorie n'existe pas avec l'id \"{Id}\".");
                }

                List< TlibraryCategorie> recordChilds = await context.TlibraryCategories.Where(s => s.IdParentCategorie == Id).ToListAsync();
                if (recordChilds.Any())
                {
                    recordChilds.ForEach(f => f.IdParentCategorie = null);
                    context.TlibraryCategories.UpdateRange(recordChilds);
                }

                context.TlibraryCategories.Remove(record);
                _ = await context.SaveChangesAsync();

                IsDeleted = true;

                return true;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion

        public static Category? ConvertToViewModel(TlibraryCategorie model)
        {
            try
            {
                if (model == null) return null;

                Category viewModel = new()
                {
                    Id = model.Id,
                    IdLibrary = model.IdLibrary,
                    IdParentCategory = model.IdParentCategorie,
                    Description = model.Description,
                    Name = model.Name,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(Category), nameof(ConvertToViewModel), ex);
                return null;
            }
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
