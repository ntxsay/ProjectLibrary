using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using LibShared.ViewModels.Contacts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibApi.Services.Contacts
{
    public class ContactType : ContactTypeVM
    {
        /// <summary>
        /// Initialise une nouvelle instance de l'objet <see cref="ContactType"/> afin créer un nouveau type de contact puis d'interagir avec lui.
        /// </summary>
        /// <param name="name">Nom du nouveau type</param>
        /// <param name="description">Description du nouveau type</param>
        /// <remarks>Remarque : Pour ajouter le type dans la base de données, appelez la méthode <see cref="CreateAsync"/></remarks>

        public ContactType(string name, string? description = null)
        {
            Name = name.Trim();
            Description = description?.Trim();
        }

        #region CRUD
        /// <summary>
        /// Ajoute un type de contact dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAsync()
        {
            try
            {
                if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Name), "Le nom du type ne peut pas être nul, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                bool isExist = await context.TcontactTypes.AnyAsync(c => c.Name.ToLower() == Name.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(ContactType), nameof(CreateAsync), "Ce type existe déjà");
                    return true;
                }

                TcontactType TcontactType = new()
                {
                    Name = Name,
                    Description = Description,
                };

                await context.TcontactTypes.AddAsync(TcontactType);
                await context.SaveChangesAsync();

                Id = TcontactType.Id;

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactType), nameof(CreateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactType), nameof(CreateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactType), nameof(CreateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour le type dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateNameAsync(string value)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(value), "Le nom du type ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"La type n'existe pas avec l'id \"{Id}\".");
                }

                bool isExist = await context.TcontactTypes.AnyAsync(c => c.Id != Id && c.Name.ToLower() == value.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(ContactType), nameof(UpdateNameAsync), "Ce type existe déjà");
                    return false;
                }

                TcontactType.Name = value.Trim();

                context.TcontactTypes.Update(TcontactType);
                _ = await context.SaveChangesAsync();

                Name = TcontactType.Name;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateNameAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateDescriptionAsync(string? value = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"Le type n'existe pas avec l'id \"{Id}\".");
                }

                TcontactType.Description = value?.Trim();

                context.TcontactTypes.Update(TcontactType);
                _ = await context.SaveChangesAsync();

                Description = TcontactType.Description;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactType), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
        }


        /// <summary>
        /// Supprime la bibliothèque de la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAsync()
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TcontactType? TcontactType = await context.TcontactTypes.SingleOrDefaultAsync(s => s.Id == Id);
                if (TcontactType == null)
                {
                    throw new ArgumentNullException(nameof(TcontactType), $"Le type n'existe pas avec l'id \"{Id}\".");
                }

                context.TcontactTypes.Remove(TcontactType);
                _ = await context.SaveChangesAsync();

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactType), nameof(DeleteAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactType), nameof(DeleteAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactType), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion
    }
}
