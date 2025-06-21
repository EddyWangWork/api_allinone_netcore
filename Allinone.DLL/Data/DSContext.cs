using Allinone.Domain.Diarys;
using Allinone.Domain.Diarys.DiaryActivitys;
using Allinone.Domain.Diarys.DiaryBooks;
using Allinone.Domain.Diarys.DiaryEmotions;
using Allinone.Domain.Diarys.DiaryFoods;
using Allinone.Domain.Diarys.DiaryLocations;
using Allinone.Domain.Diarys.DiaryWeathers;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.DS.Transactions;
using Allinone.Domain.Kanbans;
using Allinone.Domain.Members;
using Allinone.Domain.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Allinone.Domain.Shops.ShopTypes;
using Allinone.Domain.Todolists;
using Allinone.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Data
{
    public class DSContext(DbContextOptions<DSContext> options) : DbContext(options)
    {

        public DbSet<DSItem> DSItem { get; set; }
        public DbSet<DSItemSub> DSItemSub { get; set; }
        public DbSet<DSAccount> DSAccount { get; set; }
        public DbSet<DSType> DSType { get; set; }
        public DbSet<DSTransaction> DSTransaction { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<Todolist> Todolist { get; set; }
        public DbSet<TodolistDone> TodolistDone { get; set; }

        public DbSet<TripDetailType> TripDetailType { get; set; }
        public DbSet<TripDetail> TripDetail { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<Kanban> Kanban { get; set; }
        public DbSet<Shop> Shop { get; set; }
        public DbSet<ShopType> ShopType { get; set; }
        public DbSet<ShopDiary> ShopDiary { get; set; }
        public DbSet<DiaryActivity> DiaryActivity { get; set; }
        public DbSet<DiaryEmotion> DiaryEmotion { get; set; }
        public DbSet<DiaryFood> DiaryFood { get; set; }
        public DbSet<DiaryLocation> DiaryLocation { get; set; }
        public DbSet<DiaryBook> DiaryBook { get; set; }
        public DbSet<DiaryWeather> DiaryWeather { get; set; }
        public DbSet<Diary> Diary { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<Member>().ToTable("Member");
        //}
    }
}
