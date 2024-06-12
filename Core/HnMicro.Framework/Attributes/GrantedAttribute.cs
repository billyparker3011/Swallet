//using HnMicro.Core.Options;
//using HnMicro.Framework.Exceptions;
//using HnMicro.Framework.Helpers;
//using HnMicro.Framework.Services;
//using Microsoft.AspNetCore.Mvc.Authorization;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace HnMicro.Framework.Attributes
//{
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
//    public class GrantedAttribute : ActionFilterAttribute
//    {
//        private readonly List<string> _permissions = new List<string>();

//        public GrantedAttribute(string permission)
//        {
//            _permissions.Add(permission);
//        }

//        public GrantedAttribute(params string[] permission)
//        {
//            foreach (var perm in permission)
//            {
//                _permissions.Add(perm);
//            }
//        }

//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            if (context.Filters.Any(f => f.GetType() == typeof(AllowAnonymousFilter)))
//            {
//                return;
//            }

//            var serviceContext = context.HttpContext.RequestServices.GetService(typeof(IServiceContext)) as IServiceContext;
//            var userId = serviceContext.UserId;
//            var permissions = serviceContext.Permissions;
//            var actionName = context.ActionDescriptor.DisplayName;

//            if (userId <= 0L)
//            {
//                throw new UnauthorizedException("Unauthorized '{0}'", actionName);
//            }

//            if (permissions.Count <= 0)
//            {
//                throw new ForbiddenException("Forbidden '{0}'", actionName);
//            }

//            if (permissions.IsFullAccess())
//            {
//                return;
//            }

//            var serviceOption = context.HttpContext.RequestServices.GetService(typeof(ServiceOption)) as ServiceOption;
//            foreach (var perm in _permissions)
//            {
//                var normalizedPerm = perm.NormalizedPermission(serviceOption.ServiceCode);

//                if (permissions.Contains(normalizedPerm)) continue;

//                throw new ForbiddenException("Forbidden '{0}'", actionName);
//            }
//        }
//    }
//}
