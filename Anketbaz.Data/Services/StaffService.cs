using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Data.Logical;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;

namespace Anketbaz.Data.Services
{

    /// <summary>
    /// Firma personellerinin kullandigi servis. (UserService in neredeyse kopyasidir.) 
    /// </summary>
    public class StaffService
    {

        /// <summary>
        /// Giris yap
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Login(dynamic jsonData)
        {
            string mail = jsonData.mail;
            string password = jsonData.password;


            password = password.GetMd5Hash();
            staff checkStaff =
                EntityConnectionService.Staff.GetSingle(x => x.mail.Equals(mail) && x.password.Equals(password));

            if (checkStaff == null)
                return Helper.GetResult(false, "0x0007");

            checkStaff.authkey = Helper.RandomString(18);

            if (!EntityConnectionService.Staff.Update(checkStaff))
                return Helper.GetResult(false, "0x0008");
            company comp = EntityConnectionService.Company.GetSingle(x => x.compid.Equals(checkStaff.compid));
            dynamic staffData = new ExpandoObject();
            staffData.authkey = checkStaff.authkey;
            staffData.compid = checkStaff.compid;
            staffData.isadmin = checkStaff.isadmin;
            staffData.name = checkStaff.name;
            staffData.staffid = checkStaff.staffid;
            staffData.compname = comp.title;
            return Helper.GetResult(true, staffData);
        }

        /// <summary>
        /// Firmayi kaydet.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="companytitle"></param>
        /// <param name="companysector"></param>
        /// <returns></returns>
        public string Register(dynamic jsonData)
        {
            string mail = jsonData.mail;
            string password = jsonData.password;
            string name = jsonData.name;
            string companyTitle = jsonData.companytitle;
            string companySector = jsonData.companysector;

            staff checkStaff = EntityConnectionService.Staff.GetSingle(x => x.mail.Equals(mail));
            if (checkStaff != null)
                return Helper.GetResult(false, "0x0004");

            company company = new company(); 
            company.sector = companySector;
            company.title = companyTitle;
            if (!EntityConnectionService.Company.Add(company))
                return Helper.GetResult(false, "0x0015");

            checkStaff = new staff
            {
                compid = company.compid, 
                isadmin = "X",
                mail = mail,
                password = password.GetMd5Hash(),
                name = name,
                authkey = Helper.RandomString(18),
            };

            if (!EntityConnectionService.Staff.Add(checkStaff))
                return Helper.GetResult(false, "0x0016");

            dynamic data = new ExpandoObject();
            data.company = company;
            data.staff = checkStaff;
            return Helper.GetResult(true, data);
        }

        /// <summary>
        /// Personel sifre degistirmek icin kullanilir.
        /// </summary>
        /// <param name="staffid">istegi atan personel</param>
        /// <param name="ownerid">Firmanin id si</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <param name="expass">Eski sifre</param>
        /// <param name="newpass">Yeni Sifre</param>
        /// <returns></returns>
        public string ChangePassword(dynamic jsonData)
        {
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;
            long ownerid = jsonData.ownerid;
            string exPass = jsonData.expass;
            string newPass = jsonData.newpass;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            if (loggedUser.password != exPass.GetMd5Hash())
                return Helper.GetResult(false, "0x0021");

            loggedUser.password = newPass.GetMd5Hash();
            if (EntityConnectionService.Staff.Update(loggedUser))
                return Helper.GetResult(true, true);

            return Helper.GetResult(false, "0x0022");
        }
    }
}
