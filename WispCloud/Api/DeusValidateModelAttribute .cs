using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using DeusCloud.Exceptions;
using DeusCloud.Logic.CommonBase;

namespace DeusCloud.Api
{
    public class DeusValidateModelAttribute : ActionFilterAttribute
    {
        public static Type BaseModelType { get; }

        static DeusValidateModelAttribute()
        {
            BaseModelType = typeof(BaseModel);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            CheckNulls(actionContext);
            CheckParse(actionContext);
            ValidateModels(actionContext);
        }

        void CheckNulls(HttpActionContext actionContext)
        {
            var emptyArguments = actionContext.ActionArguments.Where(x => x.Value == null);
            if (emptyArguments.Any())
                throw new DeusException("Input parse error, probably invalid JSON;");
        }

        void CheckParse(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errorStates = actionContext.ModelState.Where(x => x.Value.Errors.Any());
                var innerException = new DeusException(errorStates.First().Value.Errors.First().Exception.Message);
                foreach (var state in errorStates)
                    innerException.Data.Add(state.Key, state.Value.Errors);

                throw new DeusException("Input parse error;", innerException);
            }
        }

        void ValidateModels(HttpActionContext actionContext)
        {
            foreach (var argument in actionContext.ActionArguments.Values)
            {
                var argumentType = argument.GetType();

                try
                {
                    if (BaseModelType.IsAssignableFrom(argumentType))
                        (argument as BaseModel).Validate();
                    else if (argumentType.IsArray && BaseModelType.IsAssignableFrom(argumentType.GetElementType()))
                        (argument as BaseModel[]).Validate();
                }
                catch (Exception exception)
                {
                    var expected = (exception is DeusException ? (exception as DeusException).Expected : false);
                    string message = exception.ContainsPath()
                        ? $"Ошибка валидации: Path: '{exception.GetOrCreatePath()}';" : "Ошибка валидации:";

                    throw new DeusException(message, exception, expected);
                }
            }
        }

    }

}