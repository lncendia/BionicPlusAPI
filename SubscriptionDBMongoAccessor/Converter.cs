using DomainObjects.Subscription;
using SubscriptionDBMongoAccessor.MongoClasses;

namespace SubscriptionDBMongoAccessor
{
    internal static class Converter
    {
        public static Usage Convert(this MongoUsage mongoUsage)
        {
            return new Usage
            {
                RollbackUsage = mongoUsage.RollbackUsage,
                SurveyUsage = mongoUsage.SurveyUsage,
                UserId = mongoUsage.UserId
            };
        }

        public static MongoUsage Convert(this Usage usage)
        {
            return new MongoUsage
            {
                RollbackUsage = usage.RollbackUsage,
                SurveyUsage = usage.SurveyUsage,
                UserId = usage.UserId
            };
        }

        public static BillingPromocode Convert(this MongoPromocode mongoPromocode)
        {
            return new BillingPromocode
            {
                DiscountType = mongoPromocode.DiscountType,
                DiscountValue = mongoPromocode.DiscountValue,
                Promocode = mongoPromocode.Promocode
            };
        }

        public static MongoPromocode Convert(this BillingPromocode promocode)
        {
            return new MongoPromocode
            {
                DiscountType = promocode.DiscountType,
                DiscountValue = promocode.DiscountValue,
                Promocode = promocode.Promocode
            };
        }

        public static Subscription Convert(this MongoSubscription mongoSub)
        {
            return new Subscription
            {
                CreationDate = mongoSub.CreationDate,
                Status = mongoSub.Status,
                ExpirationDate = mongoSub.ExpirationDate,
                Id = mongoSub.Id,
                InvoiceId = mongoSub.InvoiceId,
                isRecurrent = mongoSub.isRecurrent,
                Limits = mongoSub.Limits,
                PlanId = mongoSub.PlanId,
                Promocode = mongoSub.Promocode,
                Currency = mongoSub.Currency,
                Discount = mongoSub.Discount,
                Total = mongoSub.Total
            };
        }

        public static MongoSubscription Convert(this Subscription sub)
        {
            return new MongoSubscription
            {
                CreationDate = sub.CreationDate,
                Status = sub.Status,
                ExpirationDate = sub.ExpirationDate,
                Id = sub.Id,
                InvoiceId = sub.InvoiceId,
                isRecurrent = sub.isRecurrent,
                Limits = sub.Limits,
                PlanId = sub.PlanId,
                Promocode = sub.Promocode,
                Currency = sub.Currency,
                Discount = sub.Discount,
                Total = sub.Total,
            };
        }

        public static Plan Convert(this MongoPlan mongoPlan)
        {
            return new Plan
            {
                BillingPeriod = mongoPlan.BillingPeriod,
                BillingUnit = mongoPlan.BillingUnit,
                CreationDate = mongoPlan.CreationDate,
                Currency = mongoPlan.Currency,
                ExpirationDate = mongoPlan.ExpirationDate,
                Id = mongoPlan.Id,
                Limits = mongoPlan.Limits,
                Name = mongoPlan.Name,
                Price = mongoPlan.Price,
                ResellerId = mongoPlan.ResellerId
            };
        }

        public static MongoPlan Convert(this Plan plan)
        {
            return new MongoPlan
            {
                BillingPeriod = plan.BillingPeriod,
                BillingUnit = plan.BillingUnit,
                CreationDate = plan.CreationDate,
                Currency = plan.Currency,
                ExpirationDate = plan.ExpirationDate,
                Id = plan.Id,
                Limits = plan.Limits,
                Name = plan.Name,
                Price = plan.Price,
                ResellerId = plan.ResellerId
            };
        }
    }
}
