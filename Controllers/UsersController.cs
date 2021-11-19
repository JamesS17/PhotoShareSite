using ImageGallery.Data;
using Microsoft.AspNetCore.Mvc;
using PhotoShareSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PhotoShareSite.Controllers
{
    public class UsersController : Controller
    {
        private readonly IImage _imageService;

        public UsersController(IImage imageService)
        {
            _imageService = imageService;
        }


      
        public IActionResult Index(int id)
        {

            var users = _imageService.GetAllUsers();

            var usersAll = User.FindAll(ClaimTypes.NameIdentifier).ToList();
            List<UsersModel> userList = new List<UsersModel>();

            foreach (var item in users)
            {
                userList.Add(new UsersModel
                {
                    ImgId=id,
                    UserId = item.Id,
                    UserName = item.UserName,
                    Email = item.Email
                });
            }
            
            return View(userList);
        }
    }
}
