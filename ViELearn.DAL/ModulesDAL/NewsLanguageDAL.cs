using FluentData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViELearn.Models.ProjectModels;

namespace ViELearn.DAL.ModulesDAL
{
    public class NewsLanguageDAL: DALBaseClass<NewsLanguage>
    {
        public IDbContext MainDB()
        {
            return new DbContext().ConnectionStringName("DefaultConnection",
                    new SqlServerProvider());
        }
        public NewsLanguage GetDataNewsLanguage(int _newsId = 0, string _lang = "")
        {
            using (var context = MainDB())
            {
                return context.StoredProcedure("be_loaddata_News_changelanguage")
                    .Parameter("newsId", _newsId)
                    .Parameter("lang", _lang)

                     .QuerySingle<NewsLanguage>();
            }
        }
    }
}