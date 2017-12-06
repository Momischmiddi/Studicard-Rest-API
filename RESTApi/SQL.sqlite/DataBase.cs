using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFCDataRESTApi.Models;
using RESTApi.Models.NfcDataModels;

namespace NFCDataRESTApi.SQLiteDataBase
{
    public class DataBase : DbContext
    {
        public DbSet<CardInfoForStudent> CardInformationForStudent { get; set; }

        public DataBase()
        { }

        public DataBase(DbContextOptions<DataBase> options)
            : base(options)
        { }
    }
}
