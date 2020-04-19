using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;

namespace ViElearn.Services
{
  public  class DanhMucChungServices
    {
        private static DanhMucChungServices instance = null;
        private static readonly object padlock = new object();
        private DanhMucChungDAL _dal = null;

        public DanhMucChungServices()
        {
            _dal = new DanhMucChungDAL();
        }

        public static DanhMucChungServices Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DanhMucChungServices();
                    }
                    return instance;
                }
            }
        }

        public List<DanhMucChung> GetDanhMucChung(int loai)
        {
            return _dal.GetDanhMucChung(loai);
        }
        public List<DanhMucChung> GetListDanhMucChung()
        {
            return _dal.GetlistDanhMucChung();
        }
        public List<DanhMucChung> GetChildDanhMucChung(int loai, int parentId)
        {
            return _dal.GetChildDanhMucChung(loai,parentId);
        }


    }
}
