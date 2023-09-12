using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Human.Chrs.Domain.DTO;

namespace Human.Repository.AutoMapper
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            //CreateMap<Admin, NewAdminDTO>()
            //    .ForMember(x => x.AuthType, x => x.MapFrom(o => AuthType.Get(o.AuthType)));
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