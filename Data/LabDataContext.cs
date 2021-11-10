using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebMathModelLabs.Entity;

namespace WebMathModelLabs.Data
{
        public class LabDataContext : DbContext {
     
    
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             {
                 var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyLabDb.db" };
                 var connectionString = connectionStringBuilder.ToString();
                 var connection = new SqliteConnection(connectionString);

                 optionsBuilder.UseSqlite(connection);
              }
    
            public DbSet<MathLabTable1> MathLabTable1 { get; set; }
        }
}
