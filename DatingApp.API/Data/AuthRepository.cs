using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            //para poder usar el firstordefautl , debemos declarar que usaremos la libreria de Microsoft.EntityFrameworkCore;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
                return null;
            
            if(!VerifyPasswordHash(password, user.PasswordHash , user.PasswordSalt))
                return null;

            return user;
        }

        //Funcion que devolvera el hash de la contraseña usando el passworldSalt y passwordHash que ya estaban guardadas en ese usuario.
        //Basicamente vamos a encriptar la contraseña para comparar el hash resultante , con le hash que este en la base de datos. 
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using ( var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i=0; i<computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte [] passwordHash , passwordSalt;
            //out sirve para hacer pase por referencia
            //cuando cambiemos el valor dentro del parametro en  la funcion createpassworhash
            //tambien se cambiaran aqui
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //Despues de hacerle el hash a la contraseña, lo guardamos en el modelo usuario actual. 

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        //Funcion para hashear las contraseñas y darles mas seguiridad.
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using ( var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
           
        }

        public async Task<bool> UserExists(string username)
        {
           if(await _context.Users.AnyAsync(x=> x.Username == username))
                return true;

            return false;
        }
    }
}