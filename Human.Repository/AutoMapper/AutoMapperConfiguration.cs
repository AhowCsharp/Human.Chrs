﻿using AutoMapper;
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
            CreateMap<Company, CompanyDTO>().ReverseMap();
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