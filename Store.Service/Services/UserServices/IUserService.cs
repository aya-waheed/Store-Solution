using Store.Service.Services.UserServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.UserServices
{
    public interface IUserService
    {

        Task<UserDto> Login(LoginDto input);

        Task<UserDto> Register(RegisterDto input);



    }
}
