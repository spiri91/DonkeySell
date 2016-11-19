using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Extra;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnUsers
    {
        Task<DonkeySellUser> CreateOrUpdateUser(ViewUser viewUser);
        Task<string> DeleteUser(string username);
        Task<DonkeySellUser> GetUser(string username);
        Task<bool> CheckIfUsernameIsTaken(string username);
        Task<bool> CheckIfEmailIsInUse(string emailAddress);
        Task<bool> CheckIfGuidIsTheSame(string guid, string username);
        Task ConfirmEmail(string username);
        Task ResetPassword(string username, string password);
        Task<string> GetEmailOfUser(string username);
        Task ChangePassword(string username, string oldPassword, string newPassword);
        Task<List<string>> GetUsersLike(string username);
    }

    public class CrudOnUsers : ICrudOnUsers, IDisposable
    {
        private DonkeySellContext context;
        private UserManager<DonkeySellUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        private ICrudOnFriends crudOnFriends;
        private ICrudOnProducts crudOnProducts;
        private IMailSender mailSender;

        public CrudOnUsers(ICrudOnProducts crudOnProducts, IMailSender mailSender, ICrudOnFriends crudOnFriends)
        {
            context = new DonkeySellContext();
            userManager = new UserManager<DonkeySellUser>(new UserStore<DonkeySellUser>(context));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            this.crudOnProducts = crudOnProducts;
            this.mailSender = mailSender;
            this.crudOnFriends = crudOnFriends;
        }

        public async Task<DonkeySellUser> CreateOrUpdateUser(ViewUser viewUser)
        {
            if (!viewUser.IsValid())
                throw new FormatException();

            DonkeySellUser newUser = null;
            if (context.Users.Any(x => x.UserName == viewUser.UserName))
                newUser = await UpdateUser(viewUser);
            else
            {
                newUser = await CreateUser(viewUser);

                // mail confirmation
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("password")))
                    await mailSender.SendEmailConfirmationMessage(newUser.Email, newUser.ConfirmationGuid, viewUser.UserName);
            }

            return newUser;
        }

        private async Task<DonkeySellUser> UpdateUser(ViewUser viewUser)
        {
            var user = await userManager.FindAsync(viewUser.UserName, viewUser.Password);
            if (user == null)
                throw new ObjectNotFoundException();

            user.Email = viewUser.Email;
            user.Address = viewUser.Address;
            user.Avatar = viewUser.Avatar;
            user.Facebook = viewUser.Facebook;
            user.Phone = viewUser.Phone;
            user.Twitter = viewUser.Twitter;
            await context.SaveChangesAsync();

            return user;
        }

        private async Task<DonkeySellUser> CreateUser(ViewUser viewUser)
        {
            if (!Checks.PasswordIsValid(viewUser.Password))
                throw new FormatException();

            Guid guid = Guid.NewGuid();
            DonkeySellUser user = new DonkeySellUser()
            {
                UserName = viewUser.UserName,
                Email = viewUser.Email,
                UserId = guid.ToString(),
                Address = viewUser.Address,
                Avatar = viewUser.Avatar,
                Facebook = viewUser.Facebook,
                Phone = viewUser.Phone,
                Twitter = viewUser.Twitter,
                ConfirmationGuid = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(user, viewUser.Password);
            var newUser = context.Users.Single(x => x.UserName == viewUser.UserName);

            return newUser;
        }

        public async Task<string> DeleteUser(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var user = context.Users.Single(x => x.UserName == username);
            await crudOnProducts.DeleteAllProductsOfUser(username);
            await crudOnFriends.DeleteUserFromListOfFriends(username);
            await userManager.DeleteAsync(user);

            return user.Id;
        }

        public async Task<DonkeySellUser> GetUser(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var user = context.Users.Include(x => x.Products).Single(x => x.UserName == username);

            return user;
        }

        public async Task<bool> CheckIfUsernameIsTaken(string username)
        {
            var usernameIsTaken = context.Users.Any(x => x.UserName == username);

            return usernameIsTaken;
        }

        public async Task<bool> CheckIfEmailIsInUse(string email)
        {
            var emailAddressInUse = context.Users.Any(x => x.Email == email);

            return emailAddressInUse;
        }

        public async Task<bool> CheckIfGuidIsTheSame(string guid, string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var usersConfirmationGuid = context.Users.Single(x => x.UserName == username).ConfirmationGuid;

            return guid == usersConfirmationGuid;
        }

        public async Task ConfirmEmail(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var user = context.Users.Single(x => x.UserName == username);
            user.EmailConfirmed = true;
            await context.SaveChangesAsync();
        }

        public async Task ResetPassword(string username, string password)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var hashPassword = userManager.PasswordHasher.HashPassword(password);
            var user = await userManager.FindByNameAsync(username);
            user.PasswordHash = hashPassword;
            await userManager.UpdateAsync(user);
        }

        public async Task<string> GetEmailOfUser(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var email = context.Users.Single(x => x.UserName == username).Email;

            return email;
        }

        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            if (!Checks.PasswordIsValid(newPassword))
                throw new FormatException();

            var user = await userManager.FindByNameAsync(username);
            IdentityResult result = await userManager.ChangePasswordAsync(user.UserId, oldPassword, newPassword);

            if(result != IdentityResult.Success)
                throw new FormatException();
        }

        public async Task<List<string>> GetUsersLike(string username)
        {
            var users =
                context.Users.Where(x => x.UserName.ToLower().Contains(username.ToLower()))
                    .Select(x => x.UserName)
                    .ToList();

            return users;
        }

        public void Dispose()
        {
            userManager.Dispose();
            roleManager.Dispose();
            context.Dispose();
        }
    }
}