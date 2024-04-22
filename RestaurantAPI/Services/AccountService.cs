using RestaurantAPI.DTOs;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        public void RegisterUser(RegisterUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _dbContext;
        public AccountService(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;        
        }
        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                PasswordHash = dto.Password,    //just now, leter we change it to hash obj
                RoleId = dto.RoleId,
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }
    }
}
