using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViELearn.DAL.ModulesDAL;
using ViELearn.Models.ProjectModels;

namespace ViElearn.Services
{
   public  class CustomerService
    {
        private static CustomerService instance = null;
        private static readonly object padlock = new object();
        private CustomersDAL _dal = null;

        public CustomerService()
        {
            _dal = new CustomersDAL();
        }

        public static CustomerService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CustomerService();
                    }
                    return instance;
                }
            }
        }
        public List<Customers> GetAllLisstCustomer()
        {
            return _dal.GetAllLisstCustomer();
        }
        public List<Customers> SearhDataCustomer(string name , string phone,int  dateFrom,int dateTo)
        {
            return _dal.SearhDataCustomer(name,phone, dateFrom, dateTo);
        }
        public void DeleteCus(int id)
        {
             _dal.DeleteCus(id);
        }
    }
}
