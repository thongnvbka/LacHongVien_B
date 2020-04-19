using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;

namespace ViElearn.Services
{
   public class NewsLanguageServices
    {
        private static NewsLanguageServices instance = null;
        private static readonly object padlock = new object();
        private NewsLanguageDAL _dal = null;

        public NewsLanguageServices()
        {
            _dal = new NewsLanguageDAL();
        }

        public static NewsLanguageServices Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new NewsLanguageServices();
                    }
                    return instance;
                }
            }
        }
        public NewsLanguage GetDataNewsLanguage(int _newsId = 0, string _lang = "")
        {
            return _dal.GetDataNewsLanguage(_newsId, _lang);
        }
    }
}
