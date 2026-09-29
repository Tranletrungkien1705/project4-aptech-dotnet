using Project4.Data;
using Project4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Amazon.S3.Util.S3EventNotification;
using System.Security.Claims;

namespace Project4.Repository
{
    public interface IBaseRepository<T> where T : Base
    {

        Task<List<T>> GetAllAsync();
        Task<List<T>> FilterAsync( Expression<Func<T, bool>> filter = null);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(string id);
        Task<T> GetByIdAsync(String id);

    }
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly UserManager<CustomUser> _userManager;
        protected readonly string currentUserId = "";
        protected readonly IHttpContextAccessor _contextAccessor;


        public BaseRepository(ApplicationDbContext context, UserManager<CustomUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            var currentUser = _userManager.GetUserAsync(_contextAccessor.HttpContext.User).GetAwaiter().GetResult();
            if (currentUser != null)
            {
                this.currentUserId = currentUser.Id;
            }
        }

        public async Task<T> CreateAsync(T entity)
        {
            // Kiểm tra xem entity không null
            if (entity != null)
            {
                // Kiểm tra xem Id đã được thiết lập chưa
                if (string.IsNullOrEmpty(entity.Id))
                {
                    // Nếu Id chưa được thiết lập, bạn có thể sinh ra một giá trị mới
                    entity.Id = Guid.NewGuid().ToString();
                }

                var userId = string.Empty;
                if (_contextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = identity.FindFirst(ClaimTypes.Sid).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    entity.CreatedUser = user.Id;
                }
                //entity.CreatedUser = currentUserId;
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;

                // Thêm mới entity vào DbSet và lưu thay đổi vào cơ sở dữ liệu
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Trả về entity đã được tạo mới
                return entity;
            }
            return null;
        }


        public async Task<T> DeleteAsync(string id)
        {
            if (id != null)
            {
                var userId = string.Empty;
                if (_contextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = identity.FindFirst(ClaimTypes.Sid).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    userId = user.Id;
                }
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity != null)
                {
                    existingEntity.DeletedUser = userId;
                    existingEntity.IsDeleted = true;
                    existingEntity.DeletedTime = DateTime.Now;

                    _dbSet.Update(existingEntity);
                    await _context.SaveChangesAsync();

                    return existingEntity;
                }
            }

            return null;
        }


        public async Task<List<T>> FilterAsync(Expression<Func<T, bool>> filter = null)
        {
            var dataRows = _dbSet.AsQueryable();

            if (filter != null)
            {
                dataRows = dataRows.Where(filter);
            }
            var result = await dataRows.ToListAsync();
            return result;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var result = await _dbSet.Where(x=> x.IsDeleted == false).ToListAsync();
            return result;
        }




        public async Task<T> UpdateAsync(T entity)
        {
            if (entity != null)
            {
                var userId = string.Empty;
                if (_contextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = identity.FindFirst(ClaimTypes.Sid).Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    entity.UpdatedUser = user.Id;
                }
                _dbSet.Update(entity);
                //entity.UpdatedUser = currentUserId;
                entity.UpdatedTime = DateTime.Now;
                entity.IsDeleted = false;
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public async Task<T> GetByIdAsync(String id)
        {
            if (id != null)
            {
                var result = await _dbSet.FindAsync(id);
                if (result.IsDeleted == false)
                {
                    return result;
                }
                return null;
            }
            return null;
        }
    }
}
