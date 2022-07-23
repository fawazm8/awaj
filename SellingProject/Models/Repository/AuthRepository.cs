using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SellingProject.Data;
using SellingProject.Models.RepositoryInterface;

namespace SellingProject.Models.Repository
{
    public class AuthRepository : IAuthRepository
    {
        public DataContext _dataConext { get; set; }
        public AuthRepository(DataContext dataConext)
        {
            _dataConext = dataConext;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _dataConext.Users.FirstOrDefaultAsync(x =>x.UserName==username);
            if(user == null ) return null;
            if(!VerifayPasswordHash(password,user.PasswordSalt,user.PasswordHash)) return null ;
            return user;
        }

        private bool VerifayPasswordHash(string password, byte[] passwordSalt,byte[] passwordHash)
        {
           using (var hmac  = new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputeHash.Length; i++)
                {
                     if(ComputeHash[i] !=passwordHash[i])  return false;
                }
           }
           return true  ;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] PasswordHash, PasswordSalt;
            CreatePassword(password, out PasswordHash, out PasswordSalt);
            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;
            await _dataConext.Users.AddAsync(user);
            await _dataConext.SaveChangesAsync();
            return user;
        }

        private void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExist(string username)
        {
           var userExist = await  _dataConext.Users.AnyAsync(x=>x.UserName==username);
           if(userExist) return true;
           return false;
        }
    }
}