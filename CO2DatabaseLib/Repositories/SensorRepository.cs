using CO2DatabaseLib.Models;
using CO2DatabaseLib;

namespace CO2StatisticRestApi
{
    public class SensorRepository : DBConnection
    {
        public Sensor? GetById(int id)
        {
            return _dbContext.Sensors.FirstOrDefault(s => s.Id == id);
        }

    }
}
