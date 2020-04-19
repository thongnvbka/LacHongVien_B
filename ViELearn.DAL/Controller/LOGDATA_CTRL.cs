using ViELearn.Models.ViELearnModels;

namespace ViELearn.DAL.Controller
{
    public class LOGDATA_CTRL : DALBaseClass<ACTION_LOG>
    {
        public LOGDATA_CTRL()
        {
            ConnectionStringName = "DBLogConnection";
            _db = new PetaPoco.Database(ConnectionStringName);
        }

        public LOGDATA_CTRL(string dbName)
        {
            ConnectionStringName = dbName;
            _db = new PetaPoco.Database(ConnectionStringName);
        }
        public void DeleteItemById(string userID)
        {
            _db.Delete("UserDeviceOS", "UserId", null, userID);
        }
    }
}