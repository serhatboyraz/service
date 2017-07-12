using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Data.Logical;
using Anketbaz.Data.Models;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;
using Newtonsoft.Json;

namespace Anketbaz.Data.Services
{
    /// <summary>
    /// Anketler ile ilgili islemlerin yapildigi servis
    /// </summary>
    public class PollService
    {
        #region Personnel Methods

        /// <summary>
        /// Bir kisisel anket  ekler.
        /// </summary>
        /// <param name="userid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <param name="ispassword">Sifre ile mi korunuyor ? ('X'/'')</param>
        /// <param name="password">Sifre ile korunuyorsa sifre  ('X'/'')</param>
        /// <param name="iscookiecheck">Cookie den dogrulama yapilsin mi ? ('X'/'')</param>
        /// <param name="isipcheck">IP den dogrulama yapilsin mi ? ('X'/'')</param>
        /// <param name="polldata">{"PollTitle": "AnketBasligi","Questions": [{"Answers": [{"AnswerType": "CevapTipi (P/I plain/image)","Content": "cevabin icerigi (p ise metin  i ise img urlsi"}],"Content": "soru basligi","QuestionType": "SoruTipi (M/S multi/single)"}]}</param>
        /// <param name="fielddata">[{"name":"Field Basligi","type":"FieldType","code":"fieldcodeunique"},{"name":"Field Basligi","type":"FieldType","code":"fieldcodeunique"}]</param>
        /// <returns>Anket eklenirse True,Eklenmezse False</returns>
        public string AddPersonnelPoll(dynamic jsonData)
        {
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;
            string isPassword = jsonData.ispassword;
            string password = jsonData.password;
            string isCookieCheck = jsonData.iscookiecheck;
            string isIpCheck = jsonData.isipcheck;
            string isShowResult = jsonData.isshowresult;
            string fieldData = jsonData.fielddata;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            PollModel poll = jsonData.polldata.ToObject<PollModel>();
            if (poll == null)
                return Helper.GetResult(false, "0x0011");

            return PollBusiness.AddPoll(userId, "P", userId, string.Empty, isIpCheck, isCookieCheck, isPassword,
                password, fieldData, isShowResult, poll);
        }

        /// <summary>
        /// Giris yapan kullanicinin anket listenini dondurur
        /// </summary>
        /// <param name="userid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetPersonnelPollList(dynamic jsonData)
        {
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.GetPollList(userId, "P");
        }

        /// <summary>
        /// Cevaplari analiz edilmis sekilde getirir.
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="userid">Giris yapmis olan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetPersonnelAnalizedPollData(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.GetAnalizedPollData(userId, "P", pollId);
        }

        /// <summary>
        /// Cevaplari analiz edilmis sekilde getirir.
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="userid">Giris yapmis olan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetPersonnelAnalizedPollDataExport(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.ExportAnalizedPollData(userId, "P", pollId);
        }

        /// <summary>
        /// Verilen pollid yi active/deactive yapar
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="userid">Giris yapmis olan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string SetPersonnelToggleActivePoll(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long userId = jsonData.userid;
            string authKey = jsonData.authkey;

            user loggedUser = DatabaseService.CheckUserAuth(userId, authKey);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            poll editPoll = EntityConnectionService.Poll.GetSingle(
                x => x.pollid.Equals(pollId) && x.ownerid.Equals(userId) && x.ownertype.Equals("P"));
            if (editPoll == null)
                return Helper.GetResult(false, "0x0012");

            editPoll.active = editPoll.active != "X" ? "X" : "";
            if (EntityConnectionService.Poll.Update(editPoll))
                return Helper.GetResult(true, true);

            return Helper.GetResult(false, "0x0019");
        }

        #endregion

        #region Company Methods

        /// <summary>
        /// Bir sirket anketi ekler.
        /// </summary>
        /// <param name="staffid">istegi atan personel</param>
        /// <param name="ownerid">Firmanin id si</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <param name="ispassword">Sifre ile mi korunuyor ? ('X'/'')</param>
        /// <param name="password">Sifre ile korunuyorsa sifre  ('X'/'')</param>
        /// <param name="iscookiecheck">Cookie den dogrulama yapilsin mi ? ('X'/'')</param>
        /// <param name="isipcheck">IP den dogrulama yapilsin mi ? ('X'/'')</param>
        /// <param name="isprivate">Bu anketi sadece sirket personelleri yapabilir.  ('X'/'')</param>
        /// <param name="polldata">{"PollTitle": "AnketBasligi","Questions": [{"Answers": [{"AnswerType": "CevapTipi (P/I plain/image)","Content": "cevabin icerigi (p ise metin  i ise img urlsi"}],"Content": "soru basligi","QuestionType": "SoruTipi (M/S multi/single)"}]}</param>
        /// <param name="fielddata">[{"name":"Field Basligi","type":"FieldType","code":"fieldcodeunique"},{"name":"Field Basligi","type":"FieldType","code":"fieldcodeunique"}]</param>
        /// <returns>Anket eklenirse True,Eklenmezse False</returns>
        public string AddCompanyPoll(dynamic jsonData)
        {
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;
            long ownerid = jsonData.ownerid;

            string isPrivate = jsonData.isprivate;
            string isPassword = jsonData.ispassword;
            string password = jsonData.password;
            string isCookieCheck = jsonData.iscookiecheck;
            string isIpCheck = jsonData.isipcheck;
            string fieldData = jsonData.fielddata;
            string isShowResult = jsonData.isshowresult;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            PollModel poll = jsonData.polldata.ToObject<PollModel>();
            return PollBusiness.AddPoll(ownerid, "C", staffid, isPrivate, isIpCheck, isCookieCheck, isPassword,
                password, fieldData, isShowResult, poll);
        }

        /// <summary>
        /// Giris yapan personelin ait oldugu sirket anketlerini dondurur.
        /// </summary>
        /// <param name="ownerid">istegi atan kullanici</param>
        /// <param name="staffid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetCompanyPollList(dynamic jsonData)
        {
            long ownerid = jsonData.ownerid;
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.GetPollList(ownerid, "C");
        }

        /// <summary>
        /// Cevaplari analiz edilmis sekilde getirir.
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="ownerid">Anketi acan kullanicinin Id si</param>
        /// <param name="staffid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetCompanyAnalizedPollData(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long ownerid = jsonData.ownerid;
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.GetAnalizedPollData(ownerid, "C", pollId);
        }

        /// <summary>
        /// Cevaplari excell olarak export eder ve veriyi base64 olarak dondurur.
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="ownerid">Anketi acan kullanicinin Id si</param>
        /// <param name="staffid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string GetCompanyAnalizedPollDataExport(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long ownerid = jsonData.ownerid;
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            return PollBusiness.GetAnalizedPollData(ownerid, "C", pollId);
        }

        /// <summary>
        /// Verilen pollid yi active/deactive yapar
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="ownerid">Anketi acan kullanicinin Id si</param>
        /// <param name="staffid">istegi atan kullanici</param>
        /// <param name="authkey">Dogrulama keyi</param>
        /// <returns></returns>
        public string SetCompanyToggleActivePoll(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long ownerid = jsonData.ownerid;
            long staffid = jsonData.staffid;
            string authKey = jsonData.authkey;

            staff loggedUser = DatabaseService.CheckStaffAuth(staffid, authKey, ownerid);
            if (loggedUser == null)
                return Helper.GetResult(false, "0x0009");

            poll editPoll = EntityConnectionService.Poll.GetSingle(
                x => x.pollid.Equals(pollId) && x.ownerid.Equals(ownerid) && x.ownertype.Equals("C"));
            if (editPoll == null)
                return Helper.GetResult(false, "0x0012");

            editPoll.active = editPoll.active != "X" ? "X" : "";
            if (EntityConnectionService.Poll.Update(editPoll))
                return Helper.GetResult(true, true);

            return Helper.GetResult(false, "0x0019");
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Verilen bilgilere ait anketin verisini dondurur.
        /// </summary>
        /// <param name="pollid">Anket idsi</param>
        /// <param name="ownerid">Anketi olusturan kullanici idsi</param>
        /// <param name="ownertype">Anketi olusturan kullanici type i</param>
        /// <returns></returns>
        public string GetPollData(dynamic jsonData)
        {
            long pollId = jsonData.pollid;
            long ownerId = jsonData.ownerid;
            string ownerType = jsonData.ownertype;
            poll pollMaster =
                EntityConnectionService.Poll.GetSingle(x => x.pollid.Equals(pollId) && x.ownerid.Equals(ownerId) && x.ownertype.Equals(ownerType));
            if (pollMaster == null)
                return Helper.GetResult(false, "0x0012");

            if (pollMaster.isipcheck == ("X"))
            {
                string ip = jsonData.ip;
                guest pollGuest = EntityConnectionService.Guest.GetSingle(x => x.ip == ip && x.ownerid == ownerId && x.ownertype == ownerType && x.pollid == pollId);
                if (pollGuest != null)
                    return Helper.GetResult(false, "0x0018");

            }

            if (pollMaster.isprivate == "X")
            {
                long staffId = jsonData.staffid;
                string authKey = jsonData.authkey;

                staff loggedUser = DatabaseService.CheckStaffAuth(staffId, authKey, ownerId);
                if (loggedUser == null)
                    return Helper.GetResult(false, "0x0017");
            }
            pollMaster.viewcount++;
            EntityConnectionService.Poll.Update(pollMaster);

            List<question> questionList =
                EntityConnectionService.Question.GetList(
                    x =>
                        x.pollid.Equals(pollId) && x.ownerid.Equals(ownerId) && x.ownertype.Equals(ownerType)).ToList();

            PollModel poll = new PollModel();
            poll.PollId = pollMaster.pollid;
            poll.IsPassword = pollMaster.ispassword;
            poll.Password = pollMaster.password;
            poll.PollTitle = pollMaster.polltitle;
            poll.Questions = new List<Question>();
            poll.Fields = pollMaster.fielddata;
            poll.IsCookieCheck = pollMaster.iscookiecheck;
            poll.Active = pollMaster.active;
            for (int i = 0; i < questionList.Count; i++)
            {
                Question localQuestion = new Question
                {
                    Content = questionList[i].content,
                    QuestionId = questionList[i].questionid,
                    QuestionType = questionList[i].questiontype,
                    Answers = new List<Answer>()
                };

                List<answer> answerList =
                    EntityConnectionService.Answer.GetList(
                        x =>
                            x.pollid.Equals(pollId) && x.questionid.Equals(localQuestion.QuestionId) &&
                            x.ownerid.Equals(ownerId) && x.ownertype.Equals(ownerType)).ToList();

                for (int j = 0; j < answerList.Count; j++)
                {
                    Answer localAnswer = new Answer
                    {
                        AnswerId = answerList[j].answerid,
                        Content = answerList[j].content,
                        AnswerType = answerList[j].answertype
                    };
                    localQuestion.Answers.Add(localAnswer);
                }
                poll.Questions.Add(localQuestion);
            }

            return Helper.GetResult(true, poll);
        }

        /// <summary>
        /// Guestin cevaplarini tablolara insert eder.
        /// </summary>
        /// <param name="pollid">Anket Id</param>
        /// <param name="ownerid">Anketi acan kullanicinin Id si</param>
        /// <param name="ownertype">Anketi acan kullanici type i</param>
        /// <param name="guestanswerdata">Kullanicinin cevaplari : [{QuestionId:'SoruId',AnswerId:'CevapId'}]</param>
        /// <param name="staffid">eger private anket ise doldurulur o an anketi yapan staffid sidir</param>
        /// <param name="authkey">eger private anket ise doldurulur o an anketi yapan authkey idir</param>
        /// <param name="fielddata">PollMaster da belirtilen field larin doldurulmus verisini alir database e basar.[{"FieldCode":"ad-soyad","FieldValue":"test"}]</param>
        /// <returns></returns>
        public string SetGuestAnswers(dynamic jsonData)
        {
            string fieldData = jsonData.fielddata;
            long pollId = jsonData.pollid;
            long ownerId = jsonData.ownerid;
            string ownerType = jsonData.ownertype;
            int complateSecond = jsonData.complatesecond;
            string ip = jsonData.ip;
            List<GuestAnswer> answerList = jsonData.guestanswerdata.ToObject<List<GuestAnswer>>();

            poll pollMaster =
               EntityConnectionService.Poll.GetSingle(x => x.pollid.Equals(pollId) && x.ownerid.Equals(ownerId) && x.ownertype.Equals(ownerType));
            if (pollMaster == null)
                return Helper.GetResult(false, "0x0012");

            if (pollMaster.active != "X")
                return Helper.GetResult(false, "0x0020");

            if (pollMaster.isipcheck == ("X"))
            {
                guest pollGuest = EntityConnectionService.Guest.GetSingle(x => x.ip == ip && x.ownerid == ownerId && x.ownertype == ownerType && x.pollid == pollId);
                if (pollGuest != null)
                    return Helper.GetResult(false, "0x0018");
            }

            if (pollMaster.isprivate == ("X"))
            {
                long staffId = jsonData.staffid;
                string authKey = jsonData.authkey;

                staff loggedUser = DatabaseService.CheckStaffAuth(staffId, authKey, ownerId);
                if (loggedUser == null)
                    return Helper.GetResult(false, "0x0017");
            }

            guest guest = new guest
            {
                fielddata = fieldData,
                pollid = pollId,
                ownerid = ownerId,
                ownertype = ownerType,
                ip = ip,
                complatesecond = complateSecond
            };
            if (!EntityConnectionService.Guest.Add(guest))
                return Helper.GetResult(false, "0x0013");

            List<guestanswer> guestanswers = new List<guestanswer>();
            for (int i = 0; i < answerList.Count; i++)
            {
                guestanswers.Add(new guestanswer()
                {
                    pollid = pollId,
                    ownerid = ownerId,
                    ownertype = ownerType,
                    questionid = answerList[i].QuestionId,
                    answerid = answerList[i].AnswerId,
                    guestid = guest.guestid
                });
            }

            if (!EntityConnectionService.GuestAnswer.Add(guestanswers.ToArray()))
                return Helper.GetResult(false, "0x0014");

            return Helper.GetResult(true, true);

        }

        #endregion
    }
}
