using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project4.Data;
using Project4.Models;
using Project4.Repository;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Project4.Controllers
{
    public class CategoriesController : APIBaseController<Category>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IBaseRepository<Category> repository,ICategoryRepository categoryRepository, UserManager<CustomUser> userManager) : base(repository,userManager)
        {
            _categoryRepository = categoryRepository;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            // Gọi lại hàm GetAll từ lớp cha
            return await base.GetAll();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER")]
        [HttpPost]
        [Route("[action]")]
        public override async Task<IActionResult> Create(Category entity)
        {
            return await base.Create(entity);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER")]
        [HttpPut]
        [Route("[action]")]
        public override async Task<IActionResult> Update(Category entity)
        {
            return await base.Update(entity);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,MANAGER")]
        [HttpDelete]
        [Route("[action]/{id}")]
        public override async Task<IActionResult> Delete(string id)
        {
            return await base.Delete(id);
        }

        [AllowAnonymous]
        [HttpGet("GetCategoriesByName/{name}")]
        public async Task<ActionResult<Category>> GetCategoriesByName(string name)
        {
            var cateDetail = await _categoryRepository.GetCategoriesByName(name);
            if (cateDetail == null)
            {
                return NotFound();
            }
            return Ok(cateDetail);
        }



    }
}
