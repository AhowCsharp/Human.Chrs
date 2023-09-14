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

        public void SetCurrentUser(LoginDTO dto)
        {
            _user = _user ?? new CurrentUser();

            _user.Id = dto.Id;
            _user.CompanyId = dto.CompanyId;
            _user.StaffName = dto.StaffName;
            _user.DepartmentId = dto.DepartmentId;
            _user.AdminToken = dto.AdminToken;
            _user.Auth = dto.Auth;
        }
    }
}