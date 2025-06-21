namespace Allinone.Domain.Exceptions
{
    public class MemberExistException(string message = "Member already exist") : Exception(message) { }

    public class MemberNotFoundException(string message = "Member not found") : Exception(message) { }

    public class TodolistNotFoundException(string message = "Todolist not found") : Exception(message) { }

    public class TodolistAlreadyDoneException(string message = "Todolist already done") : Exception(message) { }

    //TodolistDone
    public class TodolistDoneNotFoundException(string message = "TodolistDone not found") : Exception(message) { }

    //Diary
    public class DiaryActivityNotFoundException(string message = "Diary Activity not found") : Exception(message) { }
    public class DiaryEmotionNotFoundException(string message = "Diary Emotion not found") : Exception(message) { }
    public class DiaryFoodNotFoundException(string message = "Diary Food not found") : Exception(message) { }
    public class DiaryLocationNotFoundException(string message = "Diary Location not found") : Exception(message) { }
    public class DiaryBookNotFoundException(string message = "Diary Book not found") : Exception(message) { }
    public class DiaryWeatherNotFoundException(string message = "Diary Weather not found") : Exception(message) { }
    public class DiaryNotFoundException(string message = "Diary not found") : Exception(message) { }

    //DSAccount
    public class DSAccountNotFoundException(string message = "DS Account not found") : Exception(message) { }

    //DSItem
    public class DSItemNotFoundException(string message = "DS Item not found") : Exception(message) { }


    //DSItemSub
    public class DSItemSubNotFoundException(string message = "DS Sub Item not found") : Exception(message) { }


    //DSTransaction
    public class DSTransactionNotFoundException(string message = "DS Transaction not found") : Exception(message) { }
    public class DSTransactionBadRequestException(string message = "DS Transaction bad request") : Exception(message) { }
    public class DSTransactionTransferOutBadRequestException(string message = "DS Transaction transfer out bad request") : Exception(message) { }


    //Kanban
    public class KanbanNotFoundException(string message = "Kanban not found") : Exception(message) { }


    //ShopType
    public class ShopTypeNotFoundException(string message = "Shop type not found") : Exception(message) { }


    //Shop
    public class ShopNotFoundException(string message = "Shop not found") : Exception(message) { }
    public class ShopBadRequestException(string message = "Shop not valid") : Exception(message) { }


    //Shop Diary
    public class ShopDiaryNotFoundException(string message = "Shop diary not found") : Exception(message) { }


    //Trip
    public class TripNotFoundException(string message = "Trip not found") : Exception(message) { }
    public class TripBadRequestException(string message = "Trip not valid") : Exception(message) { }


    //TripDetailType
    public class TripDetailTypeNotFoundException(string message = "Trip Detail Type not found") : Exception(message) { }
    public class TripDetailTypeBadRequestException(string message = "Trip Detail Type not valid") : Exception(message) { }


    //TripDetail
    public class TripDetailNotFoundException(string message = "Trip Detail not found") : Exception(message) { }


    //General
    public class NotFoundException(string message) : Exception(message) { }

    public class ValidationException(string message) : Exception(message) { }
}
