﻿using AppHelpers;
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
        /// Initialise une nouvelle instance de l'objet <see cref="ContactRole"/> afin créer un nouveau rôle de contact puis d'interagir avec lui.
        /// </summary>
        /// <param name="name">Nom du nouveau rôle</param>
        /// <param name="description">Description du nouveau rôle</param>
        /// <remarks>Remarque : Pour ajouter le rôle dans la base de données, appelez la méthode <see cref="CreateAsync"/></remarks>

        public ContactRole(string name, string? description = null)
        {
            Name = name.Trim();
            Description = description?.Trim();
        }

        #region CRUD
        /// <summary>
        /// Ajoute un rôle dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateAsync()
        {
            try
            {
                if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Name), "Le nom du rôle ne peut pas être nul, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                bool isExist = await context.TcontactRoles.AnyAsync(c => c.Name.ToLower() == Name.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(ContactRole), nameof(CreateAsync), "Ce rôle existe déjà");
                    return true;
                }

                TcontactRole tContactRole = new()
                {
                    Name = Name,
                    Description = Description,
                };

                await context.TcontactRoles.AddAsync(tContactRole);
                await context.SaveChangesAsync();

                Id = tContactRole.Id;

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(CreateAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(CreateAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(CreateAsync), ex);
                return false;
            }
        }

        /// <summary>
        /// Met à jour le rôle dans la base de données
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateNameAsync(string value)
        {
            try
            {
                if (value.IsStringNullOrEmptyOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(value), "Le nom du rôle ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
                }

                using LibrarySqLiteDbContext context = new();

                TcontactRole? tContactRole = await context.TcontactRoles.SingleOrDefaultAsync(s => s.Id == Id);
                if (tContactRole == null)
                {
                    throw new ArgumentNullException(nameof(tContactRole), $"Le rôle n'existe pas avec l'id \"{Id}\".");
                }

                bool isExist = await context.TcontactRoles.AnyAsync(c => c.Id != Id && c.Name.ToLower() == value.ToLower());
                if (isExist)
                {
                    Logs.Log(nameof(ContactRole), nameof(UpdateNameAsync), "Ce rôle existe déjà");
                    return false;
                }

                tContactRole.Name = value.Trim();

                context.TcontactRoles.Update(tContactRole);
                _ = await context.SaveChangesAsync();

                Name = tContactRole.Name;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateNameAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateNameAsync), ex);
                return false;
            }
        }

        public async Task<bool> UpdateDescriptionAsync(string? value = null)
        {
            try
            {
                using LibrarySqLiteDbContext context = new();

                TcontactRole? tContactRole = await context.TcontactRoles.SingleOrDefaultAsync(s => s.Id == Id);
                if (tContactRole == null)
                {
                    throw new ArgumentNullException(nameof(tContactRole), $"La rôle n'existe pas avec l'id \"{Id}\".");
                }

                tContactRole.Description = value?.Trim();

                context.TcontactRoles.Update(tContactRole);
                _ = await context.SaveChangesAsync();

                Description = tContactRole.Description;
                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (OperationCanceledException ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateDescriptionAsync), ex);
                return false;
            }
            catch (Exception ex)
            {
                Logs.Log(nameof(ContactRole), nameof(UpdateDescriptionAsync), ex);
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
    }
}
