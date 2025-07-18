﻿using Allinone.Domain;
using Allinone.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Allinone.API.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled Exception");

            var response = new ApiResponse(null) { Success = false };
            var statusCode = StatusCodes.Status500InternalServerError;

            var errorMessage = string.Join(", ",
                new[] { context.Exception.Message, context.Exception.InnerException?.ToString() }
                .Where(msg => !string.IsNullOrWhiteSpace(msg)));

            switch (context.Exception)
            {

                case NotFoundException:
                case DSAccountNotFoundException:
                case TodolistDoneNotFoundException:
                case TodolistNotFoundException:
                case MemberNotFoundException:
                case DSItemNotFoundException:
                case DSItemSubNotFoundException:
                case DSTransactionNotFoundException:
                case KanbanNotFoundException:
                case ShopTypeNotFoundException:
                case ShopNotFoundException:
                case TripNotFoundException:
                case TripDetailTypeNotFoundException:
                case TripDetailNotFoundException:
                case DiaryActivityNotFoundException:
                case DiaryEmotionNotFoundException:
                case DiaryFoodNotFoundException:
                case DiaryLocationNotFoundException:
                case DiaryBookNotFoundException:
                case DiaryWeatherNotFoundException:
                case DiaryNotFoundException:
                case DiaryTypeNotFoundException:
                case DiaryDetailNotFoundException:
                    {
                        //response.Message = context.Exception.Message;
                        response.Message = errorMessage;
                        statusCode = StatusCodes.Status404NotFound;
                    }
                    break;
                case ValidationException:
                case MemberExistException:
                case TodolistAlreadyDoneException:
                case DSTransactionBadRequestException:
                case DSTransactionTransferOutBadRequestException:
                case ShopBadRequestException:
                case TripBadRequestException:
                case TripDetailTypeBadRequestException:
                case DiaryBadRequestException:
                case DiaryDateDuplicatedException:
                    {
                        response.Message = errorMessage;
                        statusCode = StatusCodes.Status400BadRequest;
                    }
                    break;
                default:
                    {
                        response.Message = errorMessage;
                        statusCode = StatusCodes.Status500InternalServerError;
                    }
                    break;
            }

            //context.Result = result;

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
}
