using Expense_Tracker.Models;
using MessagePack.Formatters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Expense_Tracker.Customs
{
    public class CustomUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        public CustomUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public byte[] getUserProfilePicture(ApplicationUser User)
        {
            return User.profilePicture;
        }
    }
}
