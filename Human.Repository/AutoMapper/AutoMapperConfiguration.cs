using AutoMapper;
using LineTag.Core.Domain.DTO;
using LineTag.Core.Domain.DTO.Lottery;
using LineTag.Core.Domain.DTO.Mgm;
using LineTag.Core.Domain.DTO.New;
using LineTag.Core.Domain.DTO.New.Admin;
using LineTag.Core.Domain.DTO.New.Audience;
using LineTag.Core.Domain.DTO.New.AutoResponse;
using LineTag.Core.Domain.DTO.New.Job;
using LineTag.Core.Domain.DTO.New.Line.Message;
using LineTag.Core.Domain.DTO.New.MessageTemplate;
using LineTag.Core.Domain.DTO.New.Push;
using LineTag.Core.Domain.DTO.New.PushKind;
using LineTag.Core.Domain.DTO.New.Welcome;
using LineTag.Core.Domain.DTO.Survey;
using LineTag.Core.Enums;
using LineTag.Core.Utility;
using LineTag.Core.Utility.Json;
using LineTag.Infrastructure.EF;
using LineTag.Infrastructure.MongoDB;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LineTag.Infrastructure
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<Admin, NewAdminDTO>()
                .ForMember(x => x.AuthType, x => x.MapFrom(o => AuthType.Get(o.AuthType)));
            CreateMap<NewAdminDTO, Admin>()
                .ForMember(x => x.AuthType, x => x.MapFrom(o => o.AuthType.Value));

            CreateMap<AdminInviteToken, AdminInviteTokenDTO>()
                .ForMember(x => x.AuthType, x => x.MapFrom(o => AuthType.Get(o.AuthType)));
            CreateMap<AdminInviteTokenDTO, AdminInviteToken>()
                .ForMember(x => x.AuthType, x => x.MapFrom(o => o.AuthType.Value));

            CreateMap<Application, ApplicationDTO>().ReverseMap();
            CreateMap<EF.LineTag, LineTagDTO>().ReverseMap();
            CreateMap<LineAudience, LineAudienceDTO>()
                .ForMember(x => x.AssociationType, x => x.MapFrom(o => AssociationType.Get(o.AssociationType)));
            CreateMap<LineAudienceDTO, LineAudience>()
                .ForMember(x => x.AssociationType, x => x.MapFrom(o => o.AssociationType.Value));
            CreateMap<LineAudienceToUser, LineAudienceToUserDTO>().ReverseMap();
            CreateMap<LineEmoji, LineEmojiDTO>().ReverseMap();
            CreateMap<LineInsightFollower, LineInsightFollowerDTO>().ReverseMap();
            CreateMap<LineInsightInteraction, LineInsightInteractionDTO>().ReverseMap();
            CreateMap<LineInsightTagUsage, LineInsightTagUsageDTO>().ReverseMap();
            CreateMap<LineInteraction, LineInteractionDTO>()
                .ForMember(x => x.Id, x => x.MapFrom(o => o.Id.ToString()))
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => RefFrom.Get(o.RefFrom)))
                .ForMember(x => x.Type, x => x.MapFrom(o => LineInteractionType.Get(o.Type)));
            CreateMap<LineInteractionDTO, LineInteraction>()
                .ForMember(x => x.Id, x => x.MapFrom(o => new ObjectId(o.Id)))
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => o.RefFrom.Name))
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value));
            CreateMap<LineJob, LineJobDTO>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => RefFrom.Get(o.RefFrom)))
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => JobStatus.Get(o.JobStatus)))
                .ForMember(x => x.JobType, x => x.MapFrom(o => JobType.Get(o.JobType)));
            CreateMap<LineJobDTO, LineJob>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => o.RefFrom.Name))
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => o.JobStatus.Value))
                .ForMember(x => x.JobType, x => x.MapFrom(o => o.JobType.Value));
            CreateMap<LineJobDetail, LineJobDetailDTO>()
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => JobStatus.Get(o.JobStatus)));
            CreateMap<LineJobDetailDTO, LineJobDetail>()
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => o.JobStatus.Value));
            CreateMap<LineMenu, LineMenuDTO>().ReverseMap();
            CreateMap<LineMenuAreaLink, LineMenuAreaLinkDTO>()
                .ForMember(x => x.Type, x => x.MapFrom(o => ActionType.Get(o.Type)));
            CreateMap<LineMenuAreaLinkDTO, LineMenuAreaLink>()
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value));
            CreateMap<LineMenuGroup, LineMenuGroupDTO>().ReverseMap();
            CreateMap<LineMenuToUserClick, LineMenuToUserClickDTO>().ReverseMap();
            CreateMap<LineMessage, LineMessageDTO>().ReverseMap();
            CreateMap<LineMessageTemplate, LineMessageTemplateDTO>().ReverseMap();
            CreateMap<LinePush, LinePushDTO>().ReverseMap();
            CreateMap<LinePushKind, LinePushKindDTO>().ReverseMap();
            CreateMap<LinePushToUser, LinePushToUserDTO>().ReverseMap();
            CreateMap<LineResponse, LineResponseDTO>()
                .ForMember(x => x.Environment, x => x.MapFrom(o => ResponseEnvironment.Get(o.Environment)))
                .ForMember(x => x.Type, x => x.MapFrom(o => ResponseType.Get(o.Type)))
                .ForMember(x => x.Period, x => x.MapFrom(o => ResponsePeriod.Get(o.Period)));
            CreateMap<LineResponseDTO, LineResponse>()
                .ForMember(x => x.Environment, x => x.MapFrom(o => o.Environment.Value))
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value))
                .ForMember(x => x.Period, x => x.MapFrom(o => o.Period.Value));
            CreateMap<LineSticker, LineStickerDTO>().ReverseMap();
            CreateMap<LineTagKind, LineTagKindDTO>()
                .ForMember(x => x.Type, x => x.MapFrom(o => TagType.Get(o.Type)));
            CreateMap<LineTagKindDTO, LineTagKind>()
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value));
            CreateMap<LineTagToUser, LineTagToUserDTO>().ReverseMap();
            CreateMap<LineUser, LineUserDTO>()
                .ForMember(x => x.Gender, x => x.MapFrom(o => Gender.Get(o.Gender)))
                .ForMember(x => x.Language, x => x.MapFrom(o => Language.Get(o.Language)));
            CreateMap<LineUserDTO, LineUser>()
                .ForMember(x => x.Gender, x => x.MapFrom(o => o.Gender.Name))
                .ForMember(x => x.Language, x => x.MapFrom(o => o.Language.Name));
            CreateMap<Lottery, LotteryDTO>().ReverseMap();
            CreateMap<LotteryPrize, LotteryPrizeDTO>().ReverseMap();
            CreateMap<LotteryPrizeSerialNumber, LotteryPrizeSerialNumberDTO>().ReverseMap();
            CreateMap<LotteryPrizeTempSerialNumber, LotteryPrizeTempSerialNumberDTO>().ReverseMap();
            CreateMap<MGM, MgmDTO>()
                .ForMember(x => x.Type, x => x.MapFrom(o => MGMType.Get(o.Type)));
            CreateMap<MgmDTO, MGM>()
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value));
            CreateMap<MGMLog, MgmLogDTO>().ReverseMap();
            CreateMap<MGMToUser, MgmToUserDTO>().ReverseMap();

            CreateMap<OfficialAccount, OfficialAccountDTO>()
                .ForMember(x => x.PlanType, x => x.MapFrom(o => PlanType.Get(o.PlanType)));

            CreateMap<OfficialAccount, NewOfficialAccountDTO>()
                .ForMember(x => x.PlanType, x => x.MapFrom(o => PlanType.Get(o.PlanType)));

            CreateMap<OfficialAccountDTO, OfficialAccount>()
                .ForMember(x => x.PlanType, x => x.MapFrom(o => o.PlanType.Value));
            CreateMap<NewOfficialAccountDTO, OfficialAccount>()
                .ForMember(x => x.PlanType, x => x.MapFrom(o => o.PlanType.Value));
            CreateMap<PushPrice, PushPriceDTO>().ReverseMap();

            CreateMap<PushUserInteractionStatistic, PushUserInteractionStatisticDTO>()
                .ForMember(x => x.Id, x => x.MapFrom(o => o.Id.ToString()));
            CreateMap<PushUserInteractionStatisticClick, PushUserInteractionStatisticClickDTO>().ReverseMap();
            CreateMap<PushUserInteractionStatisticDTO, PushUserInteractionStatistic>()
                .ForMember(x => x.Id, x => x.MapFrom(o => new ObjectId(o.Id)));
            CreateMap<PushUserInteractionStatisticMessage, PushUserInteractionStatisticMessageDTO>().ReverseMap();
            CreateMap<PushUserInteractionStatisticOverview, PushUserInteractionStatisticOverviewDTO>().ReverseMap();
            CreateMap<ShortUrl, ShortUrlDTO>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => RefFrom.Get(o.RefFrom)))
                .ForMember(x => x.Target, x => x.MapFrom(o => ShortUrlTarget.Get(o.Target)));
            CreateMap<ShortUrlCounter, ShortUrlCounterDTO>()
                .ForMember(x => x.Id, x => x.MapFrom(o => o.Id.ToString()));
            CreateMap<ShortUrlCounterDTO, ShortUrlCounter>()
                .ForMember(x => x.Id, x => x.MapFrom(o => new ObjectId(o.Id)));
            CreateMap<ShortUrlDTO, ShortUrl>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => o.RefFrom.Name))
                .ForMember(x => x.Target, x => x.MapFrom(o => o.Target.Value));
            CreateMap<Survey, SurveyDTO>().ReverseMap();
            CreateMap<SurveyAnswer, SurveyAnswerDTO>().ReverseMap();
            CreateMap<SurveyUser, SurveyUserDTO>().ReverseMap();
            CreateMap<TagMessage, TagMessageDTO>().ReverseMap();
            CreateMap<TagMessageLog, TagMessageLogDTO>()
                .ForMember(x => x.Id, x => x.MapFrom(o => o.Id.ToString()));
            CreateMap<TagMessageLogDTO, TagMessageLog>()
                .ForMember(x => x.Id, x => x.MapFrom(o => new ObjectId(o.Id)));

            CreateMap<LineMessageTemplate, NewMessageTemplateDTO>()
                .ForMember(x => x.List, x => x.MapFrom(o => GetMessageList(o.JsonData)));
            CreateMap<NewMessageTemplateDTO, LineMessageTemplate>();
            CreateMap<LineJob, NewJobDTO>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => RefFrom.Get(o.RefFrom)))
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => JobStatus.Get(o.JobStatus)))
                .ForMember(x => x.JobType, x => x.MapFrom(o => JobType.Get(o.JobType)));
            CreateMap<NewJobDTO, LineJob>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => o.RefFrom.Name))
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => o.JobStatus.Value))
                .ForMember(x => x.JobType, x => x.MapFrom(o => o.JobType.Value));
            CreateMap<LineJobDetail, NewJobDetailDTO>()
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => JobStatus.Get(o.JobStatus)));
            CreateMap<NewJobDetailDTO, LineJobDetail>()
                .ForMember(x => x.JobStatus, x => x.MapFrom(o => o.JobStatus.Value));
            CreateMap<LineResponse, NewAutoResponseDTO>()
                .ForMember(x => x.Type, x => x.MapFrom(o => ResponseType.Get(o.Type)))
                .ForMember(x => x.Environment, x => x.MapFrom(o => ResponseEnvironment.Get(o.Environment)))
                .ForMember(x => x.Period, x => x.MapFrom(o => ResponsePeriod.Get(o.Period)))
                .ForMember(x => x.Keywords, x => x.MapFrom(o => JsonHelper.DeserializeObject<IEnumerable<string>>(o.Keyword)))
                .ForMember(x => x.List, x => x.MapFrom(o => GetMessageList(o.JsonData)));
            CreateMap<NewAutoResponseDTO, LineResponse>()
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value))
                .ForMember(x => x.Environment, x => x.MapFrom(o => o.Environment.Value))
                .ForMember(x => x.Period, x => x.MapFrom(o => o.Period.Value))
                .ForMember(x => x.Keyword, x => x.MapFrom(o => JsonConvert.SerializeObject(o.Keywords)));
            CreateMap<LineMessage, NewLineMessageDTO>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => RefFrom.Get(o.RefFrom)));
            CreateMap<NewLineMessageDTO, LineMessage>()
                .ForMember(x => x.RefFrom, x => x.MapFrom(o => o.RefFrom.Name));
            CreateMap<LineResponse, NewWelcomeDTO>()
                .ForMember(x => x.Type, x => x.MapFrom(o => ResponseType.Get(o.Type)))
                .ForMember(x => x.Environment, x => x.MapFrom(o => ResponseEnvironment.Get(o.Environment)))
                .ForMember(x => x.List, x => x.MapFrom(o => GetMessageList(o.JsonData)));
            CreateMap<NewWelcomeDTO, LineResponse>()
                .ForMember(x => x.Type, x => x.MapFrom(o => o.Type.Value))
                .ForMember(x => x.Environment, x => x.MapFrom(o => o.Environment.Value));
            CreateMap<LinePushKind, NewPushKindDTO>().ReverseMap();
            CreateMap<LinePush, NewPushDTO>()
                .ForMember(x => x.SendStatus, x => x.MapFrom(o => SendStatus.Get(o.SendStatus)))
                .ForMember(x => x.List, x => x.MapFrom(o => GetMessageList(o.JsonData)));
            CreateMap<NewPushDTO, LinePush>()
                .ForMember(x => x.SendStatus, x => x.MapFrom(o => o.SendStatus.Value));
            CreateMap<LinePushKind, NewPushKindDTO>()
                .ForMember(x => x.PushCount, x => x.MapFrom(o => o.LinePush.Count));
            CreateMap<NewPushKindDTO, LinePushKind>();
            CreateMap<LineAudience, NewAudienceDTO>()
                .ForMember(x => x.AssociationType, x => x.MapFrom(o => AssociationType.Get(o.AssociationType)));
            CreateMap<NewAudienceDTO, LineAudience>()
                .ForMember(x => x.AssociationType, x => x.MapFrom(o => o.AssociationType.Value));
        }

        private IEnumerable<NewMessageDTO> GetMessageList(string json)
        {
            var jToken = JToken.Parse(json);
            JsonHelper.SetEmptyToNull(jToken);

            JsonSerializer jsonSerializer = new();

            jsonSerializer.Converters.Add(new LineMessageTypeConverter());
            jsonSerializer.Converters.Add(new EnumerationConverter<ActionType>());

            return jToken["List"].ToObject<IEnumerable<NewMessageDTO>>(jsonSerializer);
        }
    }
}