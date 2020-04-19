using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class CustomersDAL
    {
        public IDbContext MainDB()
        {
            return new DbContext().ConnectionStringName("DefaultConnection",
                    new SqlServerProvider());
        }
        public List<Customers> GetAllLisstCustomer()
        {
            using (var context = MainDB())
            {
                return context.Sql("SELECT TOP 1000 [Id],[CusName] ,[CusMail],[Service] ,[Status],[Type]FROM[Saohathanh].[dbo].[Customers]")
                    .QueryMany<Customers>();
            }
        }
        public List<Customers> SearhDataCustomer(string name, string phone, int dateFrom, int dateTo)
        {
            using (var context = MainDB())
            {
                return context.StoredProcedure("Search_Dada_tbl_Custommer")
                    .Parameter("name", name)
                    .Parameter("phone", phone)
                    .Parameter("from_date", dateFrom)
                    .Parameter("to_date", dateTo)
                    .QueryMany<Customers>();
            }
        }
        public void DeleteCus(int id)
        {
            using (var context = MainDB())
            {
                context.Sql($"DELETE dbo.Customers WHERE Id = {id}")
                   .Execute();
            }

        }
    }
}