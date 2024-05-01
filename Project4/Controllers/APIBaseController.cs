using Project4.Models;
using Project4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static Amazon.S3.Util.S3EventNotification;

namespace Project4.Controllers
{
    [Route("api")]
    [ApiController]

    public class APIBaseController<T> : Controller where T : Base
    {
        private readonly IBaseRepository<T> _repository;
        protected readonly UserManager<CustomUser> _userManager;
        public APIBaseController(IBaseRepository<T> repository, UserManager<CustomUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Error!");
            }
        }
        [HttpPost]
        [Route("[controller]/[action]")]
        public virtual async Task<IActionResult> Create(T entity)
        {
            var result = await _repository.CreateAsync(entity);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Error!");
            }
        }

        [HttpPut]
        [Route("[controller]/[action]")]
        public virtual async Task<IActionResult> Update([FromBody]T entity)
        {
            var result = await _repository.UpdateAsync(entity);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Error!");
            }
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> GetById(string Id)
        {
            var result = await _repository.GetByIdAsync(Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Error!");
            }   
        }

        [HttpDelete]
        [Route("[controller]/[action]/{id}")]
        public virtual async Task<IActionResult> Delete(string id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Error!");
            }
        }
    }
}
