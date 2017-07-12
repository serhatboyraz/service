using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Data.Logical;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;

namespace Anketbaz.Data.Services
{
    /// <summary>
    /// Kullanicilar ile ilgili islemlerin yapildigi servis
    /// </summary>
    public class UserService
    {

        /// <summary>
        /// Kullanici kayitli mi ?
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Login(dynamic jsonData)
        {
            string mail = jsonData.mail;
            string password = jsonData.password;

            password = password.GetMd5Hash();
            user checkUser =
                EntityConnectionService.User.GetSingle(x => x.mail.Equals(mail) && x.password.Equals(password));

            if (checkUser == null)
                return Helper.GetResult(false, "0x0007");

            checkUser.authkey = Helper.RandomString(18);

            if (!EntityConnectionService.User.Update(checkUser))
                return Helper.GetResult(false, "0x0008");

            return Helper.GetResult(true, checkUser);
        }

        /// <summary>
        /// Kullaniciyi kaydet
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Register(dynamic jsonData)
        {
            string mail = jsonData.mail;
            string password = jsonData.password;
            string name = jsonData.name;

            user checkUser = EntityConnectionService.User.GetSingle(x => x.mail.Equals(mail));
            if (checkUser == null)
            {
                checkUser = new user();
                checkUser.mail = mail;
                checkUser.password = password.GetMd5Hash();
                checkUser.name = name;
                checkUser.authkey = Helper.RandomString(18); 

                if (!EntityConnectionService.User.Add(checkUser))
                    return Helper.GetResult(false, "0x0005");

                return Helper.GetResult(true, checkUser);
            }

            return Helper.GetResult(false, "0x0004");
        }

        /// <summary>
        /// Personel sifre degistirmek icin kullanilir.
        /// </summary>
        /// <param name="userid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <param name="expass">Eski sifre</param>
        /// <param name="newpass">Yeni Sifre</param>
        /// <returns></returns>
        public string ChangePassword(dynamic jsonData)
        {
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;

            string exPass = jsonData.expass;
            string newPass = jsonData.newpass;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            if (loggedUser.password != exPass.GetMd5Hash())
                return Helper.GetResult(false, "0x0021");

            loggedUser.password = newPass.GetMd5Hash();
            if (EntityConnectionService.User.Update(loggedUser))
                return Helper.GetResult(true, true);

            return Helper.GetResult(false, "0x0022");
        }
    }
}
