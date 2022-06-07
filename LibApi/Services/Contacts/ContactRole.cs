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
    public class ContactRole : ContactRoleVM
    {
        /// <summary>
        /// Obtient une valeur booléenne indiquant si l'objet a déjà été effacé de la base de données
        /// </summary>
        public bool IsDeleted { get; private set; }

        public new long Id
        {
            get => _Id;
            protected set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string Name
        {
            get => _Name;
            protected set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public new string? Description
        {
            get => _Description;
            protected set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de l'objet <see cref="ContactRole"/> afin créer un nouveau rôle de contact puis d'interagir avec lui.
        /// </summary>
        /// <param name="name">Nom du nouveau rôle</param>
        /// <param name="description">Description du nouveau rôle</param>
        /// <remarks>Remarque : Pour ajouter le rôle dans la base de données, appelez la méthode <see cref="CreateAsync"/></remarks>

        private ContactRole()
        {
        }

        #region CRUD
        /// <summary>
        /// Ajoute un rôle dans la base de données
        /// </summary>
        /// <returns></returns>
        public static async Task<ContactRole?> CreateAsync(string name, string? description = null, bool openIfExist = false)
        {
            try
            {
                if (name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(name), "Le nom du rôle ne peut pas être nul, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                TcontactRole? existingItem = await context.TcontactRoles.SingleOrDefaultAsync(c => c.Name.ToLower() == name.Trim().ToLower());
                if (existingItem != null)
                {
                    Logs.Log(nameof(ContactRole), nameof(CreateAsync), $"Le rôle \"{name}\" existe déjà");
                    if (openIfExist)
                    {
                        return ConvertToViewModel(existingItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Le rôle {name} existe déjà");
                    }
                }

                TcontactRole tContactRole = new()
                {
                    Name = name.Trim(),
                    Description = description?.Trim(),
                };

                await context.TcontactRoles.AddAsync(tContactRole);
                await context.SaveChangesAsync();

                return ConvertToViewModel(tContactRole);
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(CreateAsync), ex);
                return null;
            }
        }

        /// <summary>
        /// Met à jour le rôle dans la base de données
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
                    throw new InvalidOperationException($"La bibliothèque {Name} a déjà été supprimée.");
                }

                //S'il n'y a pas de nouveau nom et que la modification de la description est ignoré, alors génère une erreur.
                if (newName.IsStringNullOrEmptyOrWhiteSpace() && newDescription == null)
                {
                    throw new InvalidOperationException("Le nouveau nom de ce rôle ou sa nouvelle description doit être renseignée.");
                }

                using LibrarySqLiteDbContext context = new();

                TcontactRole? record = await context.TcontactRoles.SingleOrDefaultAsync(s => s.Id == Id);
                if (record == null)
                {
                    throw new NullReferenceException($"Le rôle n'existe pas avec l'id \"{Id}\".");
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    bool isExist = await context.TcontactRoles.AnyAsync(c => c.Id != Id && c.Name.ToLower() == newName.Trim().ToLower())!;
                    if (isExist)
                    {
                        throw new ArgumentException($"Ce rôle existe déjà.");
                    }
                }

                if (!newName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    record.Name = newName.Trim();
                }

                //La mise à jour de la description n'est pas ignorée si elle est différent de null.
                if (newDescription != null)
                {
                    record.Description = newDescription?.Trim();
                }

                context.TcontactRoles.Update(record);
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
                Logs.Log(nameof(ContactRole), nameof(UpdateAsync), ex);
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

                TcontactRole? tContactRole = await context.TcontactRoles.SingleOrDefaultAsync(s => s.Id == Id);
                if (tContactRole == null)
                {
                    throw new ArgumentNullException(nameof(TcontactRole), $"Le rôle n'existe pas avec l'id \"{Id}\".");
                }

                context.TcontactRoles.Remove(tContactRole);
                _ = await context.SaveChangesAsync();

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(DeleteAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(DeleteAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(DeleteAsync), ex);
                return false;
            }
        }

        #endregion


        /// <summary>
        /// Convertit un model en Model de vue
        /// </summary>
        /// <param name="model">Modèle de données</param>
        /// <returns></returns>
        public static ContactRole? ConvertToViewModel(TcontactRole model)
        {
            try
            {
                if (model == null) return null;

                ContactRole viewModel = new()
                {
                    Id = model.Id,
                    Description = model.Description,
                    Name = model.Name,
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(ConvertToViewModel), ex);
                return null;
            }
        }
    }
}
