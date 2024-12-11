using CO2DatabaseLib.Models;
using CO2DatabaseLib;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CO2DatabaseLib
{
    public class UserRepository : DBConnection
    {
        private const int _saltLength = 8;
        private const int _storedPasswordLength = 12;
        private const int _hashIterations = 10000;
        public User? GetById(int id)
        {
            return _dbContext.Users.FirstOrDefault(user => user.Id == id);
        }

        public User? Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(email))
                return null;
            // email too long
            if (email.Length > 32)
                return null;
            User? user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return null;
            if (!VerifyPassword(password, user))
                return null;

            return user;
        }
        public User? Create(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;
            if (_dbContext.Users.FirstOrDefault(u => u.Email == email) != null)
                return null;
            // email too long
            if (email.Length > 32 ||email.Length < 8)
                return null;
            // @ and .
            if (!email.Contains("@") || !email.Contains("."))
                return null;
            var user = new User() { Email = email, UserPassword = HashPassword(password) };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }

        public User? DeleteUser(string email, string password)
        {
            User? user = Login(email, password);
            if (user == null)
                return null;
            // remove all SensorUser assosiated with this user
            var sensorUsers = _dbContext.SensorUser.Where(x => x.UserId == user.Id);
            _dbContext.RemoveRange(sensorUsers);
            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            return user;
        }
        private string HashPassword(string password)
        {
            // random salt to make it harder to brute force
            byte[] salt = RandomNumberGenerator.GetBytes(_saltLength);
            // the hashing algorithm 
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _hashIterations);
            byte[] hash = pbkdf2.GetBytes(_storedPasswordLength);

            // store the salt, then the hashed password
            byte[] hashBytes = new byte[_saltLength + _storedPasswordLength];
            Array.Copy(salt, 0, hashBytes, 0, _saltLength);
            Array.Copy(hash, 0, hashBytes, _saltLength, _storedPasswordLength);
            // convert from byte-array to string
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }
        private bool VerifyPassword(string password, User user)
        {
            // the stored salt + password
            string savedPasswordHash = user.UserPassword;
            // convert to byte-array
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            // get the salt
            byte[] salt = new byte[_saltLength];
            Array.Copy(hashBytes, 0, salt, 0, _saltLength);
            // run the hashing algorithm
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _hashIterations);
            byte[] hash = pbkdf2.GetBytes(_storedPasswordLength);
            // see if we got the same value
            for (int i = 0; i < _storedPasswordLength; i++)
                if (hashBytes[i + _saltLength] != hash[i])
                    return false;
            return true;
        }

        public bool ChangeEmail(string oldEmail, string password, string newEmail)
        {
            var user = Login(oldEmail, password);
            if (user == null)
                return false;
            user.Email = newEmail;
            _dbContext.SaveChanges();
            return true;
        }

        public bool ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = Login(email, oldPassword);
            if (user == null)
                return false;
            user.UserPassword = HashPassword(newPassword);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
