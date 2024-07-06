using Lottery.Core.Enums;
using System.Linq.Expressions;

namespace Lottery.Core.Helpers
{
    public static class TicketHelper
    {
        public static TicketState GetRefundRejectStateByIsLive(this bool isLive)
        {
            return isLive == true ? TicketState.Reject : TicketState.Refund;
        }

        public static Expression<Func<Data.Entities.Ticket, bool>> ContainsNumbers(this List<int> chooseNumbers, ContainOperator @operator)
        {
            var param = Expression.Parameter(typeof(Data.Entities.Ticket), "p");
            Expression body = null;
            foreach (var number in chooseNumbers)
            {
                var member = Expression.Property(param, nameof(Data.Entities.Ticket.ChoosenNumbers));
                var constant = Expression.Constant(number.NormalizeNumber());
                var expression = Expression.Call(member, "Contains", Type.EmptyTypes, constant);
                body = body == null ? expression : @operator == Enums.ContainOperator.Or ? Expression.OrElse(body, expression) : Expression.And(body, expression);
            }
            return Expression.Lambda<Func<Data.Entities.Ticket, bool>>(body, param);
        }

        public static Expression<Func<Data.Entities.Player, bool>> ContainsUsername(this List<string> usernames, ContainOperator @operator)
        {
            var param = Expression.Parameter(typeof(Data.Entities.Player), "p");
            Expression body = null;
            foreach (var username in usernames)
            {
                var member = Expression.Property(param, nameof(Data.Entities.Player.Username));
                var constant = Expression.Constant(username);
                var expression = Expression.Call(member, "Contains", Type.EmptyTypes, constant);
                body = body == null ? expression : @operator == Enums.ContainOperator.Or ? Expression.OrElse(body, expression) : Expression.And(body, expression);
            }
            return Expression.Lambda<Func<Data.Entities.Player, bool>>(body, param);
        }
    }
}
