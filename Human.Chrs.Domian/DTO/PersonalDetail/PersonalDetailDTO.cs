﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Human.Chrs.Domain.SeedWork;

namespace Human.Chrs.Domain.DTO
{
    public class PersonalDetailDTO : IDTO
    {
        public int id { get; set; }

        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public DateTime BirthDay { get; set; }

        public string Gender { get; set; }

        public int IsMarried { get; set; }

        public int HasLicense { get; set; }

        public double Height { get; set; }

        public int Weight { get; set; }

        public string IdentityNo { get; set; }

        public int? HasCrimeRecord { get; set; }

        public string Memo { get; set; }

        public string WorkLocation { get; set; }

        public string LevelPosition { get; set; }

        public string Department { get; set; }

        public DateTime EntryDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }
    }
}