using Human.Chrs.Domain.CommonModels;
using Human.Chrs.Domain.DTO;

namespace Human.Chrs.Domain.Services
{
    public class UserService
    {
        private CurrentUser _user = null;

        private CurrentUser User
        {
            get
            {
                return _user;
            }
        }

        public CurrentUser GetCurrentUser()
        {
            return User;
        }

        public void SetApToken(string apToken)
        {
            _user = _user ?? new CurrentUser();

            _user.ApToken = apToken;
        }

        public void SetCompanyId(int companyId)
        {
            _user = _user ?? new CurrentUser();

            _user.CompanyId = companyId;
        }

        public void SetCurrentUser(CurrentUser user)
        {
            _user = _user ?? new CurrentUser();

            _user.Id = user.Id;
            _user.CompanyId = user.CompanyId;
            _user.StaffName = user.StaffName;
            _user.WorkPosition = user.WorkPosition;
            _user.Email = user.Email;
            _user.Auth = user.Auth;
        }

        public void SetCurrentAdmin(AdminDTO dto)
        {
            _user = _user ?? new CurrentUser();

            _user.Id = dto.Id;
            _user.CompanyId = dto.CompanyId;
            _user.StaffName = dto.UserName;
            _user.WorkPosition = dto.WorkPosition;
            _user.Auth = dto.Permissions;
        }
    }
}