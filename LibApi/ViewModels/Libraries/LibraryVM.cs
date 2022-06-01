using System;
using System.Diagnostics;
using AppHelpers;
using AppHelpers.Strings;
using LibApi.Models.Local.SQLite;
using Microsoft.EntityFrameworkCore;

namespace LibApi.ViewModels.Libraries
{
	public class LibraryVM : GenericItemVM
	{
		public LibraryVM()
		{
		}

		public override async Task<bool> CreateAsync()
		{
            try
            {
				if (Name.IsStringNullOrEmptyOrWhiteSpace())
                {
					throw new ArgumentNullException(nameof(Name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
				}

				using LibrarySqLiteDbContext context = new ();

				bool isExist = await context.Tlibraries.AnyAsync(c => c.Name.ToLower() == Name.ToLower());
				if (isExist)
                {
					Logs.Log(nameof(LibraryVM), nameof(CreateAsync), "Cette bibliothèque existe déjà");
					return true;
				}

				Tlibrary tlibrary = new()
				{
					DateAjout = DateAjout.ToString(),
					Guid = Guid.ToString(),
					Name = Name,
					Description = Description,
				};

				await context.Tlibraries.AddAsync(tlibrary);
                await context.SaveChangesAsync();

				Id = tlibrary.Id;

				return true;
			}
			catch (ArgumentNullException ex)
            {
				Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
				return false;
			}
			catch (OperationCanceledException ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
				return false;
			}
			catch (Exception ex)
            {
				Logs.Log(nameof(LibraryVM), nameof(CreateAsync), ex);
				return false;
            }
		}

		public override async Task<bool> UpdateAsync()
		{
			try
			{
				if (Name.IsStringNullOrEmptyOrWhiteSpace())
				{
					throw new ArgumentNullException(nameof(Name), "Le nom de la bibliothèque ne peut pas être nulle, vide ou ne contenir que des espaces blancs.");
				}

				using LibrarySqLiteDbContext context = new();

				Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
				if (tlibrary == null)
                {
					throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
				}

				bool isExist = await context.Tlibraries.AnyAsync(c => c.Id != Id &&  c.Name.ToLower() == Name.ToLower());
				if (isExist)
				{
					Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), "Cette bibliothèque existe déjà");
					return false;
				}

				DateTime dateEdition = DateTime.Now;

				tlibrary.Name = this.Name;
				tlibrary.Description = this.Description;
				tlibrary.DateEdition = dateEdition.ToString();

				context.Tlibraries.Update(tlibrary);
				_ = await context.SaveChangesAsync();

				DateEdition = dateEdition;

				return true;
			}
			catch (ArgumentNullException ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
			catch (OperationCanceledException ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
			catch (Exception ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
		}

		public override async Task<bool> DeleteAsync()
		{
			try
			{
				using LibrarySqLiteDbContext context = new();

				Tlibrary? tlibrary = await context.Tlibraries.SingleOrDefaultAsync(s => s.Id == Id);
				if (tlibrary == null)
				{
					throw new ArgumentNullException(nameof(Tlibrary), $"La bibliothèque n'existe pas avec l'id \"{Id}\".");
				}
				
				context.Tlibraries.Remove(tlibrary);
				_ = await context.SaveChangesAsync();

				return true;
			}
			catch (ArgumentNullException ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
			catch (OperationCanceledException ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
			catch (Exception ex)
			{
				Logs.Log(nameof(LibraryVM), nameof(UpdateAsync), ex);
				return false;
			}
		}


	}
}

