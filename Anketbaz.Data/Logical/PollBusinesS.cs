using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anketbaz.Data.Models;
using Anketbaz.Database.Database;
using Anketbaz.Database.Helper;
using Anketbaz.Database.Log;

namespace Anketbaz.Data.Logical
{
    public static class PollBusiness
    {
        public static string AddPoll(long ownerid, string ownerType, long crusr, string isPrivate, string isIpCheck, string isCookieCheck, string isPassword, string password, string fieldData, string isShowResult, PollModel poll)
        {
            poll newPoll = new poll
            {
                ownertype = ownerType,
                ownerid = ownerid,
                polltitle = poll.PollTitle,
                crusr = crusr,
                isprivate = isPrivate,
                isipcheck = isIpCheck,
                iscookiecheck = isCookieCheck,
                ispassword = isPassword,
                password = password,
                fielddata = fieldData,
                isshowresult = isShowResult,
                active = "X",
                viewcount = 0
            };

            List<answer> pollAnswers = new List<answer>();

            if (EntityConnectionService.Poll.Add(newPoll))
            {
                for (int i = 0; i < poll.Questions.Count; i++)
                {
                    question addQuestion = new question
                    {
                        ownerid = ownerid,
                        ownertype = ownerType,
                        pollid = newPoll.pollid,
                        questionid = poll.Questions[i].QuestionId,
                        content = poll.Questions[i].Content,
                        questiontype = poll.Questions[i].QuestionType,
                    };
                    EntityConnectionService.Question.Add(addQuestion);

                    for (int j = 0; j < poll.Questions[i].Answers.Count; j++)
                    {
                        pollAnswers.Add(new answer
                        {
                            ownerid = ownerid,
                            ownertype = ownerType,
                            pollid = newPoll.pollid,
                            questionid = addQuestion.questionid,
                            content = poll.Questions[i].Answers[j].Content,
                            answertype = poll.Questions[i].Answers[j].AnswerType
                        });
                    }
                    EntityConnectionService.Answer.Add(pollAnswers.ToArray());
                    pollAnswers = new List<answer>();
                }
                return Helper.GetResult(true, newPoll.pollid);
            }

            return Helper.GetResult(false, "0x0010");
        }

        public static string GetAnalizedPollData(long ownerid, string ownerType, long pollId)
        {
            AnalysePollData pollData = GetAnalizedPollDataModel(ownerid, ownerType, pollId);
            if (pollData == null)
                return Helper.GetResult(false, "0x0012");
            return Helper.GetResult(true, pollData);
        }


        public static string ExportAnalizedPollData(long ownerid, string ownerType, long pollId)
        {
            return Exporter.PollExportExcel(ownerid, ownerType, pollId);
        }


        public static AnalysePollData GetAnalizedPollDataModel(long ownerid, string ownerType, long pollId)
        {
            poll pollMaster =
              EntityConnectionService.Poll.GetSingle(x => x.pollid.Equals(pollId) && x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType));
            if (pollMaster == null)
                return null;

            Stopwatch analyseStopwatch = new Stopwatch();
            analyseStopwatch.Start();

            List<question> questionList =
                EntityConnectionService.Question.GetList(
                    x =>
                        x.pollid.Equals(pollId) && x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType)).ToList();

            PollModel poll = new PollModel();
            poll.PollId = pollMaster.pollid;
            poll.PollTitle = pollMaster.polltitle;
            poll.Questions = new List<Question>();
            poll.Fields = pollMaster.fielddata;
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

                localQuestion.VoteCount =
                    EntityConnectionService.GuestAnswer.GetDataCount(
                        x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId) && x.questionid.Equals(localQuestion.QuestionId));

                List<answer> answerList =
                    EntityConnectionService.Answer.GetList(
                        x =>
                            x.pollid.Equals(pollId) && x.questionid.Equals(localQuestion.QuestionId) &&
                            x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType)).ToList();

                for (int j = 0; j < answerList.Count; j++)
                {
                    Answer localAnswer = new Answer
                    {
                        AnswerId = answerList[j].answerid,
                        Content = answerList[j].content,
                        AnswerType = answerList[j].answertype
                    };
                    localAnswer.VoteCount =
                        EntityConnectionService.GuestAnswer.GetDataCount(
                            x =>
                                x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.answerid.Equals(localAnswer.AnswerId) &&
                                x.questionid.Equals(localQuestion.QuestionId));
                    localQuestion.Answers.Add(localAnswer);
                }
                poll.Questions.Add(localQuestion);
            }

            AnalysePollData analysePoll = new AnalysePollData();
            analysePoll.GuestAnswers = new List<GuestAnswer>();
            var guestAnswerList = EntityConnectionService.GuestAnswer.GetList(
                x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId));

            for (int i = 0; i < guestAnswerList.Count; i++)
            {
                analysePoll.GuestAnswers.Add(new GuestAnswer()
                {
                    GuestId = guestAnswerList[i].guestid,
                    AnswerId = guestAnswerList[i].answerid,
                    QuestionId = guestAnswerList[i].questionid,
                });
            }
            analysePoll.Poll = poll;
            analysePoll.GuestList =
                EntityConnectionService.Guest.GetDataPart(
                    x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType) && x.pollid.Equals(pollId),
                    x => new { x.guestid, x.fielddata, x.crdat, x.crtim, x.complatesecond },
                    x => new { x.crdat, x.crtim }).ToList();

            int totalComplateSecond = 0;
            foreach (var guest in analysePoll.GuestList)
            {
                if (!string.IsNullOrEmpty(guest.complatesecond.ToString()))
                    totalComplateSecond += int.Parse(guest.complatesecond.ToString());
            }
            if (analysePoll.GuestList.Count > 0)
            {
                TimeSpan avarageTime = TimeSpan.FromSeconds(totalComplateSecond / analysePoll.GuestList.Count);
                analysePoll.AvarageComplateTime = new DateTime(avarageTime.Ticks).ToStringHhmmss();
                analysePoll.AvarageComplateSeconds = (totalComplateSecond / analysePoll.GuestList.Count).ToString();
                analysePoll.TotalComplate = analysePoll.GuestList.Count.ToString();
                analysePoll.TotalView = pollMaster.viewcount.ToString();
                analysePoll.AnalyseDateTime = DateTime.Now.ToStringYyyyMMddHhmmss();
            }
            else
            {
                analysePoll.AvarageComplateTime = "000000";
                analysePoll.AvarageComplateSeconds = "0";
                analysePoll.TotalComplate = "0";
                analysePoll.TotalView = "0";
                analysePoll.AnalyseDateTime = DateTime.Now.ToStringYyyyMMddHhmmss();
            }



            analyseStopwatch.Stop();
            analysePoll.AnalyseTime = string.Format("{0}:{1}:{2}.{3}", analyseStopwatch.Elapsed.Hours,
                analyseStopwatch.Elapsed.Minutes, analyseStopwatch.Elapsed.Seconds,
                analyseStopwatch.Elapsed.Milliseconds);

            Log.Info(string.Format("Analyse Time :{0}", analysePoll.AnalyseTime));
            return analysePoll;
        }

        public static string GetPollList(long ownerid, string ownerType)
        {
            var pollMaster =
                EntityConnectionService.Poll.GetList(x => x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType))
                    .OrderByDescending(x => x.crdat + x.crtim);

            long[] pollIds = pollMaster.Select(x => x.pollid).ToArray();
            var guestList =
                EntityConnectionService.Guest.GetDataPart(x => pollIds.Contains(x.pollid) && x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType),
                    x => new { x.pollid, x.crdat, x.crtim, x.fielddata });

            var questionList =
                EntityConnectionService.Question.GetDataPart(
                    x => pollIds.Contains(x.pollid) && x.ownerid.Equals(ownerid) && x.ownertype.Equals(ownerType),
                    x => new { x.pollid, x.questiontype, x.content });

            var pollList = (from polls in pollMaster
                            select new
                            {
                                polls.pollid,
                                polls.polltitle,
                                polls.active,
                                polls.crdat,
                                polls.crtim,
                                count = guestList.Count(x => x.pollid == polls.pollid),
                                guests = guestList.Where(x => x.pollid == polls.pollid).Take(10),
                                questions = questionList.Where(x => x.pollid == polls.pollid)
                            });

            return Helper.GetResult(true, pollList);
        }

    }
}
