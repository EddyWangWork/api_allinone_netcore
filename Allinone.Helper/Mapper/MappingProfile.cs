using Allinone.Domain.Auditlogs;
using Allinone.Domain.Diarys;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Diarys.DiaryTypes;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Enums;
using Allinone.Domain.Kanbans;
using Allinone.Domain.Members;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Domain.Todolists;
using Allinone.Domain.Trips;
using Allinone.Helper.Enums;
using AutoMapper;

namespace Allinone.Helper.Mapper
{
    public class Source<T>
    {
        public T Item { get; set; }
    }
    public class Sources<T>
    {
        public List<T> Items { get; set; }
    }

    public class Destination<T>
    {
        public T Item { get; set; }
    }

    public class Destinations<T>
    {
        public List<T> Items { get; set; }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap(typeof(Source<>), typeof(Destination<>));
            //CreateMap(typeof(Destination<>), typeof(Source<>));
            //CreateMap(typeof(Sources<>), typeof(Destinations<>));
            //CreateMap(typeof(Destinations<>), typeof(Sources<>));
            CreateMap<MemberDto, Member>();
            CreateMap<Member, MemberDto>();

            CreateMap<Todolist, TodolistAddReq>();
            CreateMap<TodolistAddReq, Todolist>();

            CreateMap<TodolistDoneAddReq, TodolistDone>();
            CreateMap<TodolistDone, TodolistDoneAddReq>();
            CreateMap<TodolistDoneUpdateReq, TodolistDone>();

            CreateMap<DSAccountAddReq, DSAccount>();

            CreateMap<DSItemAddReq, DSItem>();

            CreateMap<DSTransactionReq, DSTransaction>();

            CreateMap<DSItemSubAddReq, DSItemSub>();

            CreateMap<KanbanAddReq, Kanban>();

            CreateMap<ShopTypeAddReq, ShopType>();

            CreateMap<ShopAddReq, Shop>();
            CreateMap<Shop, ShopDto>();

            CreateMap<ShopDiaryAddReq, ShopDiary>();

            CreateMap<TripAddReq, Trip>();
            CreateMap<TripDetailTypeAddReq, TripDetailType>();
            CreateMap<TripDetailAddReq, TripDetail>();

            CreateMap<DiaryActivityAddReq, DiaryActivity>();
            CreateMap<DiaryEmotionAddReq, DiaryEmotion>();
            CreateMap<DiaryFoodAddReq, DiaryFood>();
            CreateMap<DiaryLocationAddReq, DiaryLocation>();
            CreateMap<DiaryBookAddReq, DiaryBook>();
            CreateMap<DiaryWeatherAddReq, DiaryWeather>();
            CreateMap<Diary, DiaryDto>();

            CreateMap<DiaryAddReq, Diary>();
            CreateMap<DiaryTypeAddReq, DiaryType>();
            CreateMap<DiaryDetailAddReq, DiaryDetail>();
            CreateMap<DiaryDetail, DiaryDetailDto>();

            CreateMap<Auditlog, AuditlogDto>()
                .ForMember(dest => dest.TypeName, opt =>
                    opt.MapFrom(src => EnumHelper.GetEnumStringValue<EnumAuditlogType>(src.TypeID)))
                .ForMember(dest => dest.ActionTypeName, opt =>
                    opt.MapFrom(src => EnumHelper.GetEnumStringValue<EnumAuditlogActionType>(src.ActionTypeID)));
        }
    }
}
