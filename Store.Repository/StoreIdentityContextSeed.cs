using Microsoft.AspNetCore.Identity;
using Store.Data.Entities.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class StoreIdentityContextSeed
    {

        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {

            if(!userManager.Users.Any())
            {

                var user = new AppUser
                {

                    DisplayName = "Aya Waheed",
                    Email = "ayawaheed1506@gmail.com",
                    UserName = "ayawaheed",
                    Address = new Address
                    {
                        FirstName = "Aya",
                        LastName = "Waheed",
                        City = "Helwan",
                        State = "Cairo",
                        Street = "6",
                        PostalCode = "12345"

                    }
                };

                await userManager.CreateAsync(user, "Password123!");

            }



        }



    }
}
