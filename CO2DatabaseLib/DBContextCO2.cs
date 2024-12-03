using CO2DatabaseLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace CO2DatabaseLib.Services
{
	public class DbContextCO2 : DbContext
	{
		public DbContextCO2(DbContextOptions<DbContextCO2> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Sensor> Sensors { get; set; }
		public DbSet<Measurement> Measurements { get; set; }
		public DbSet<SensorUser> SensorUser { get; set; }

	}
}
