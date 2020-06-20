using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Eduardo", "Fuentes", "efuenteszeledon@gmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin);
            UserEntity driver = await CheckUserAsync("2020", "Eduardo", "Fuentes", "efuenteszeledon@yahoo.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Driver);
            UserEntity user1 = await CheckUserAsync("3030", "Eduardo", "Fuentes", "efuenteszeledon@outlook.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user2 = await CheckUserAsync("5050", "Camila", "Cifuentes", "camila@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user3 = await CheckUserAsync("6060", "Sandra", "Usuga", "sandra@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user4 = await CheckUserAsync("7070", "Lisa", "Marin", "luisa@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            await CheckTaxisAsync(driver, user1, user2);
            await CheckUserGroups(user1, user2, user3, user4);
        }

        private async Task CheckUserGroups(UserEntity user1, UserEntity user2, UserEntity user3, UserEntity user4)
        {
            if (!_dataContext.UserGroups.Any())
            {
                _dataContext.UserGroups.Add(new UserGroupEntity
                {
                    User = user1,
                    Users = new List<UserGroupDetailEntity>
            {
                new UserGroupDetailEntity { User = user2 },
                new UserGroupDetailEntity { User = user3 },
                new UserGroupDetailEntity { User = user4 }
            }
                });

                _dataContext.UserGroups.Add(new UserGroupEntity
                {
                    User = user2,
                    Users = new List<UserGroupDetailEntity>
            {
                new UserGroupDetailEntity { User = user1 },
                new UserGroupDetailEntity { User = user3 },
                new UserGroupDetailEntity { User = user4 }
            }
                });

                await _dataContext.SaveChangesAsync();
            }
        }


        private async Task<UserEntity> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTaxisAsync(
            UserEntity driver,
            UserEntity user1,
            UserEntity user2)
        {
            if (!_dataContext.Taxis.Any())
            {
                _dataContext.Taxis.Add(new TaxiEntity
                {
                    User = driver,
                    Plaque = "TPQ123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque nec nunc est. Vestibulum volutpat mauris ut consectetur mattis. Suspendisse rhoncus interdum est, non iaculis enim euismod sed. Curabitur sit amet odio in dui sodales maximus. Aliquam quis nisl sapien. Fusce lobortis, quam ut consequat bibendum, lorem enim vehicula urna, eget rhoncus dui diam in ante. Fusce rhoncus arcu mauris, nec efficitur quam euismod ac. Donec iaculis ante vel volutpat ornare. Proin ullamcorper euismod odio, sed elementum orci laoreet eget. Cras finibus metus eu sem pretium, eget rhoncus dui interdum. In lobortis orci id diam sagittis, sed congue eros pharetra.",
                            User = user1
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Nullam ut nisi auctor, porttitor orci at, faucibus mauris. Mauris faucibus iaculis eros a lacinia. Curabitur auctor ac sapien sit amet ultrices. Ut mattis ante ut dui rhoncus, et tincidunt ipsum mattis. Sed mattis lacinia risus a varius. Aenean vulputate sit amet erat sed eleifend. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vel risus nisl. Pellentesque hendrerit bibendum diam ac vestibulum. Duis sodales sem velit, consequat dapibus tortor egestas quis. Sed hendrerit sodales odio, ut rutrum ex molestie nec. Quisque suscipit ullamcorper dolor nec mollis. Praesent quis accumsan magna. Nunc malesuada iaculis aliquam.",
                            User = user1
                        }
                    }
                });

                _dataContext.Taxis.Add(new TaxiEntity
                {
                    Plaque = "THW321",
                    User = driver,
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user2
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user2
                        }
                    }
                });

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
