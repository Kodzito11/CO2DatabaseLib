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

        public Sensor Create(string name, int warningValue)
        {
            var sensor = new Sensor() { SensorName = name, WarningValue = warningValue };
            _dbContext.Sensors.Add(sensor);
            _dbContext.SaveChanges();
            return sensor;
        }
        public void ChangeWarningValue(int sensorId, int newWarningValue)
        {
            var sensor = GetById(sensorId);
            if (sensor == null)
                return;
            sensor.WarningValue = newWarningValue;
            _dbContext.SaveChanges();
        }
    }
}
