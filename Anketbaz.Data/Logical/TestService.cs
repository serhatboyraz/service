using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;
using Anketbaz.Database.Log;

namespace Anketbaz.Data.Logical
{
    public class TestService
    {
        public static void Add(int k)
        {
            try
            {

                string n =
                    "Adams,Baker,Clark,Davis,Evans,Frank,Ghosh,Hills,Irwin,Jones,Klein,Lopez,Mason,Nalty,Ochoa,Patel,Quinn,Reily,Smith,Trott,Usman,Valdo,White,Xiang,Yakub,Zafar";
                string[] names = n.Split(',');
                long pollId = 5;
                long ownerId = 1;
                for (int j = 0; j < k; j++)
                {
                    Random r = new Random();
                    string fieldData =
                        "[{\"FieldCode\":\"ad-soyad\",\"FieldValue\":\"" + names[r.Next(0, 26)] +
                        "\"},{\"FieldCode\":\"cep-telefonu\",\"FieldValue\":\"" + r.Next(11111111, 99999999) + "\"}]";
                    guest guest = new guest
                    {
                        fielddata = fieldData,
                        pollid = pollId,
                        ownerid = ownerId,
                        ownertype = "P",
                        ip = GetRandomIp(),
                        complatesecond = r.Next(20, 60)
                    };
                    EntityConnectionService.Guest.Add(guest);

                    var pollList = EntityConnectionService.Question.GetList(x => x.pollid.Equals(pollId));
                    var answerList = EntityConnectionService.Answer.GetList(x => x.pollid.Equals(pollId));

                    List<guestanswer> guestanswers = new List<guestanswer>();
                    for (var i = 0; i < pollList.Count; i++)
                    {
                        var lAnswers = answerList.Where(x => x.questionid.Equals(pollList[i].questionid)).ToList();
                        long answerId = lAnswers[r.Next(0, lAnswers.Count)].answerid;

                        guestanswer ga = new guestanswer()
                        {
                            pollid = pollId,
                            ownerid = ownerId,
                            ownertype = "P",
                            questionid = pollList[i].questionid,
                            answerid = answerId,
                            guestid = guest.guestid
                        };
                        guestanswers.Add(ga);
                    }

                    EntityConnectionService.GuestAnswer.Add(guestanswers.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString() + ex.Message.ToString());
            }

        }

        private static Random _random = new Random();
        public static string GetRandomIp()
        {
            return string.Format("{0}.{1}.{2}.{3}", _random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));
        }
    }
}
