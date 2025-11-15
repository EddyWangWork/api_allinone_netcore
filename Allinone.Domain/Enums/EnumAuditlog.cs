namespace Allinone.Domain.Enums
{
    public enum EnumAuditlogType
    {
        Login = 1,
        Todolist = 2,
        TodolistDone = 9,
        Diary = 3,
        DailySpent = 4,
        Kanban = 5,
        Member = 6,
        Shop = 7,
        ShopDiary = 10,
        Trip = 8
    }

    public enum EnumAuditlogActionType
    {
        New = 1,
        Update = 2,
        Delete = 3,
        Done = 4
    }
}
