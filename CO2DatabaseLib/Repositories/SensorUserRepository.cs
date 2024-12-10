using CO2DatabaseLib.Models;
using CO2DatabaseLib;
using Microsoft.EntityFrameworkCore;

namespace CO2StatisticRestApi
{
    public class SensorUserRepository : DBConnection
    {
        private SensorRepository _sensorRepository = new SensorRepository();
        public IEnumerable<Sensor> GetByUserId(int id)
        {
            IEnumerable<int> SensorUsers = _dbContext.SensorUser.Where(su => su.UserId == id).Select(su => su.SensorId);
            return _dbContext.Sensors.Where(s => SensorUsers.Contains(s.Id));
        }
        public IEnumerable<User> GetBySensorId(int id)
        {
            IEnumerable<int> SensorUsers = _dbContext.SensorUser.Where(su => su.SensorId == id).Select(su => su.UserId);
            return _dbContext.Users.Where(u => SensorUsers.Contains(u.Id));
        }

        public SensorUser? SubscribeToSensor(int userId, int sensorId)
        {
            if (_sensorRepository.GetById(sensorId) == null || 
                _dbContext.SensorUser.FirstOrDefault(su => su.UserId == userId && su.SensorId == sensorId) != null)
                return null;
            SensorUser su = new SensorUser() { SensorId=sensorId, UserId=userId };
            _dbContext.SensorUser.Add(su);
            _dbContext.SaveChanges();
            return su;
        }

    }
}
