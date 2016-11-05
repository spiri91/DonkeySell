using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnUsers
    {
        Task<ViewUser> CreateOrUpdateUser(ViewUser viewUser);

        Task<string> DeleteUser(string username);

        Task<ViewUser> GetUser(string username);
        Task<bool> CheckIfUsernameIsTaken(string username);
        Task<bool> CheckIfEmailIsInUse(string emailAddress);
    }

    public class CrudOnUsers : ICrudOnUsers, IDisposable
    {
        private DonkeySellContext context;
        private UserManager<DonkeySellUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private ICrudOnProducts crudOnProducts;

        public CrudOnUsers(ICrudOnProducts crudOnProducts)
        {
            context = new DonkeySellContext();
            userManager = new UserManager<DonkeySellUser>(new UserStore<DonkeySellUser>(context));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            this.crudOnProducts = crudOnProducts;
        }

        public async Task<ViewUser> CreateOrUpdateUser(ViewUser viewUser)
        {
            ViewUser newUser = null;
            if (UserIsAllreadyPresentInDb(viewUser.UserName))
                newUser = await UpdateUser(viewUser);
            else
                newUser = await CreateUser(viewUser);

            return newUser;
        }

        private async Task<ViewUser> UpdateUser(ViewUser viewUser)
        {
            var user = await userManager.FindAsync(viewUser.UserName, viewUser.Password);
            if (user != null)
            {
                user.Email = viewUser.Email;
                user.Address = viewUser.Address;
                user.Avatar = viewUser.Avatar;
                user.Facebook = viewUser.Facebook;
                user.Phone = viewUser.Phone;
                user.Twitter = viewUser.Twitter;
                await context.SaveChangesAsync();
                var newViewUser = Mapper.Map<ViewUser>(user);

                return newViewUser;
            }

            return null;
        }

        private async Task<ViewUser> CreateUser(ViewUser viewUser)
        {
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
                Twitter = viewUser.Twitter
            };

            await userManager.CreateAsync(user, viewUser.Password);
            var newUser = context.Users.Single(x => x.UserName == viewUser.UserName);
            var newViewUser = Mapper.Map<ViewUser>(newUser);

            return newViewUser;
        }

        private bool UserIsAllreadyPresentInDb(string userName)
        {
            return context.Users.Any(x => x.UserName == userName);
        }

        public async Task<string> DeleteUser(string username)
        {
            var user = context.Users.Single(x => x.UserName == username);
            await crudOnProducts.DeleteAllProductsOfUser(username);
            await userManager.DeleteAsync(user);

            return user.Id;
        }

        public async Task<ViewUser> GetUser(string username)
        {
            var user = context.Users.Include(x => x.Products).Single(x => x.UserName == username);
            var viewUser = Mapper.Map<ViewUser>(user);
            return viewUser;
        }

        public async  Task<bool> CheckIfUsernameIsTaken(string username)
        {
            var usernameIsTaken = context.Users.Count(x => x.UserName == username);

            return usernameIsTaken == 0;
        }

        public async Task<bool> CheckIfEmailIsInUse(string email)
        {
            var emailAddressInUse = context.Users.Count(x => x.Email == email);

            return emailAddressInUse == 0;
        }

        public void Dispose()
        {
            userManager.Dispose();
            roleManager.Dispose();
            context.Dispose();
        }
    }
}