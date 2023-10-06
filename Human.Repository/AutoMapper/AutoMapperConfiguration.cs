using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Human.Chrs.Domain.DTO;
using Human.Repository.EF;

namespace Human.Repository.AutoMapper
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<Staff, StaffDTO>().ReverseMap();
            CreateMap<AmendCheckRecord, AmendCheckRecordDTO>().ReverseMap();
            CreateMap<Admin, AdminDTO>().ReverseMap();
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<IncomeLogs, IncomeLogsDTO>().ReverseMap();
            CreateMap<Application, ApplicationDTO>().ReverseMap();
            CreateMap<PersonalDetail, PersonalDetailDTO>().ReverseMap();
            CreateMap<CompanyRule, CompanyRuleDTO>().ReverseMap();
            CreateMap<CheckRecords, CheckRecordsDTO>().ReverseMap();
            CreateMap<OverTimeLog, OverTimeLogDTO>().ReverseMap();
            CreateMap<VacationLog, VacationLogDTO>().ReverseMap();
            CreateMap<EventLogs, EventLogsDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<SalarySetting, SalarySettingDTO>().ReverseMap();
        }

        //private IEnumerable<NewMessageDTO> GetMessageList(string json)
        //{
        //    var jToken = JToken.Parse(json);
        //    JsonHelper.SetEmptyToNull(jToken);

        //    JsonSerializer jsonSerializer = new();

        //    jsonSerializer.Converters.Add(new LineMessageTypeConverter());
        //    jsonSerializer.Converters.Add(new EnumerationConverter<ActionType>());

        //    return jToken["List"].ToObject<IEnumerable<NewMessageDTO>>(jsonSerializer);
        //}
    }
}