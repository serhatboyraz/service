using System.Collections.Generic;
using Anketbaz.Database.Database;

namespace Anketbaz.Data.Logical
{
    public static class DatabaseService
    {

        public static staff CheckStaffAuth(long staffId, string authKey, long compId)
        {
            return
                EntityConnectionService.Staff.GetSingle(
                    x => x.compid.Equals(compId) && x.staffid.Equals(staffId) && x.authkey.Equals(authKey));
        }
        public static user CheckUserAuth(long userid, string authKey)
        {
            return
                EntityConnectionService.User.GetSingle(
                    x => x.userid.Equals(userid) && x.authkey.Equals(authKey));
        }




    }
}
