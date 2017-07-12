using Anketbaz.Database.Repositories;

namespace Anketbaz.Data
{
    public static class EntityConnectionService
    {

        public static readonly ICompanyRepository Company = new CompanyRepository();
        public static readonly IAnswerRepository Answer = new AnswerRepository();
        public static readonly IGuestRepository Guest = new GuestRepository();
        public static readonly IGuestAnswerRepository GuestAnswer = new GuestAnswerRepository(); 
        public static readonly IPollRepository Poll = new PollRepository();
        public static readonly IQuestionRepository Question = new QuestionRepository();
        public static readonly IStaffRepository Staff = new StaffRepository();
        public static readonly IUserRepository User = new UserRepository(); 


    }
}
