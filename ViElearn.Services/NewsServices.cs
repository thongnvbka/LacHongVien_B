using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;

namespace ViElearn.Services
{
  public  class NewsServices
    {

        private static NewsServices instance = null;
        private static readonly object padlock = new object();
        private NewsDAL _dal = null;

        public NewsServices()
        {
            _dal = new NewsDAL();
        }

        public static NewsServices Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new NewsServices();
                    }
                    return instance;
                }
            }
        }

        public List<News> GetListNewsByCateStatus(int cateID, int status)
        {
            return _dal.GetListNewsByCateStatus(cateID,status);
        }

        public List<News> GetListNews()
        {
            return _dal.GetListNews();
        }


    }
}
