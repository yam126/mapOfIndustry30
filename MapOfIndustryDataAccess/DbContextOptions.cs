using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapOfIndustryDataAccess.Data
{
    public class DbContextOptions
    {
        #region Properties

        public IServiceCollection Services { get; private set; }

        #endregion

        #region Constructors

        public DbContextOptions(IServiceCollection services)
        {
            Services = services;
        }

        #endregion

        #region Methods

        public void AddMtrlSqlServer(String connectionString)
        {
            this.Services.AddTransient<IMOIRepository, MOIRepository>(x => new MOIRepository(connectionString));
        }

        #endregion
    }
}