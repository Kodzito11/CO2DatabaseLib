﻿using CO2DatabaseLib.Models;
using CO2DatabaseLib;

namespace CO2DatabaseLib
{
    public class MeasurementRepository : DBConnection
    {
        public Measurement? GetBySensorId(int id)
        {
            return _dbContext.Measurements.FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<Measurement> GetInTimeFrame(int id, DateTime? start, DateTime? end)
        {
            if (start == null)
                start = DateTime.MinValue;
            if (end == null) 
                end = DateTime.MaxValue;
            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }

            return _dbContext.Measurements.Where(m => m.MeasurementTime >= start && m.MeasurementTime <= end);
        }

    }
}
