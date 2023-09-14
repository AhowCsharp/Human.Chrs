using Human.Chrs.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Human.Chrs.Domain.DTO
{
    public class ApplicationDTO : IDTO
    {
        /// <summary>
        /// 編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string ApId { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string ApName { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 權杖
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 權杖效期
        /// </summary>
        public DateTime? Expire { get; set; }
    }
}