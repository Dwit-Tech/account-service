using DwitTech.AccountService.Data.Context;
using DwitTech.AccountService.Data.Entities;

namespace DwitTech.AccountService.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _dbContext;

        public UserRepository(AccountDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<User> GetActivationDetail(string activationCode) 
        {
            return _dbContext.Users.FirstOrDefault(x => x.ActivationDetail == activationCode.ToString());
        }


        public async Task<User> GetUser(int Id)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Status == UserStatus.Inactive);
        }


        public async Task<ActivationDetail> ActivationCodeExpiry(ActivationDetail codeDetail)
        {
            var activationCode = new ActivationDetail()
            {
                Expires = DateTime.Now.AddMinutes(10),
                Created = DateTime.Now
            };

           if (codeDetail.Expires == DateTime.Now)
            {
                throw new InvalidOperationException("Activation Code has expired");
            }
            return codeDetail;
        }
    
        public async Task<User> UpdateUserStatus(int Id)
        {

            var user = await UserRepository.GetUser(id).ToString();
           
            foreach (var Status in user)
            {
                Status = UserStatus.Active;
            }
            user.Status.SaveChanges();
            return user.Status;
        }
    }
}
