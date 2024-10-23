using DomainObjects.Infrastracture.Exceptions;
using DomainObjects.Subscription;
using MongoDB.Driver;
using SubscriptionDBMongoAccessor.Infrastracture;
using SubscriptionDBMongoAccessor.MongoClasses;

namespace SubscriptionDBMongoAccessor
{
    public class SubscriptionDBAccessor
    {
        private const string DB_NAME = "Subscription";
        private const string PLANS_COLLECTION_NAME = "Plans";
        private const string USAGES_COLLECTION_NAME = "Usages";
        private const string SUBSCRIPTIONS_COLLECTION_NAME = "Subscriptions";
        private const string PROMOCODE_COLLECTION_NAME = "Promocodes";

        private readonly IMongoCollection<MongoPlan> _plansCollection;
        private readonly IMongoCollection<MongoUsage> _usageCollection;
        private readonly IMongoCollection<MongoSubscription> _subscriptionsCollection;
        private readonly IMongoCollection<MongoPromocode> _promocodeCollection;

        public SubscriptionDBAccessor(DbSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var db = client.GetDatabase(DB_NAME);
            _plansCollection = db.GetCollection<MongoPlan>(PLANS_COLLECTION_NAME);
            _usageCollection = db.GetCollection<MongoUsage>(USAGES_COLLECTION_NAME);
            _subscriptionsCollection = db.GetCollection<MongoSubscription>(SUBSCRIPTIONS_COLLECTION_NAME);
            _promocodeCollection = db.GetCollection<MongoPromocode>(PROMOCODE_COLLECTION_NAME);
            // CreateFreePlan();
        }

        public void CreateFreePlan()
        {
            var plan = new MongoPlan()
            {
                BillingPeriod = BillingPeriod.m,
                BillingUnit = 1,
                CreationDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddYears(2),
                Currency = Currency.usd,
                Limits = new Limit { RollbackLimit = 10, SurveyLimit = 1 },
                Name = "FREE",
                Price = 0,
            };
            _plansCollection.InsertOne(plan);
        }


        public async Task<string> SetFreeSubscription(string userId, string freePlanId)
        {
            var filter = Builders<MongoPlan>.Filter.Where(p => p.Id == freePlanId);
            var plan = (await _plansCollection.FindAsync(filter)).FirstOrDefault();

            if (plan == null)
            {
                throw new NotFoundException($"free plan not found id = {freePlanId}");
            }

            
            var creationDate = DateTime.UtcNow;


            var subscription = new MongoSubscription
            {
                CreationDate = creationDate,
                ExpirationDate = GetExpirationDateFromNow(plan, creationDate),
                Status = SubscriptionStatus.ACTIVE,
                InvoiceId = 0,
                isRecurrent = true,
                Limits = plan.Limits,
                PlanId = plan.Id,
                Promocode = null,
                Currency = plan.Currency,
                Discount = default,
                Total = 0
            };

            await _subscriptionsCollection.InsertOneAsync(subscription);

            return subscription.Id ?? throw new FormatException($"Can't get subscription id"); 
        }

        public async Task<string> CreateSubscription(string planId, string invoiceId, decimal sale, string? promocode)
        {
            var filter = Builders<MongoPlan>.Filter.Where(p => p.Id == planId);
            var plan = (await _plansCollection.FindAsync(filter)).FirstOrDefault();

            if (plan == null)
            {
                throw new NotFoundException($"plan not found id = {planId}");
            }

            ApplySale(plan, sale);

            var creationDate = DateTime.UtcNow;

            var subscription = new MongoSubscription
            {
                CreationDate = creationDate,
                ExpirationDate = GetExpirationDateFromNow(plan, creationDate),
                Status = SubscriptionStatus.PENDING,
                InvoiceId = int.Parse(invoiceId),
                isRecurrent = true,
                Limits = plan.Limits,
                PlanId = plan.Id,
                Currency = plan.Currency,
                Promocode = promocode ?? null,
                Discount = sale == 0 ? default : sale,
                Total = plan.Price
            };

            await _subscriptionsCollection.InsertOneAsync(subscription);

            return subscription.Id ?? throw new FormatException($"Can't get subscription id");
        }

        public async Task<string> ActivateSubscription(string subscriptionId)
        {
            var filter = Builders<MongoSubscription>.Filter.Where(p => p.Id == subscriptionId);

            var newSubscription = Builders<MongoSubscription>.Update.Set(u => u.Status, SubscriptionStatus.ACTIVE);

            var updateResult = await _subscriptionsCollection.FindOneAndUpdateAsync(filter, newSubscription);

            return updateResult.Id ?? throw new ArgumentException($"Not found subscription with id = {subscriptionId}");
        }

        public async Task<string> DeactivateSubscription(string subscriptionId)
        {
            var filter = Builders<MongoSubscription>.Filter.Where(p => p.Id == subscriptionId);

            var newSubscription = Builders<MongoSubscription>.Update.Set(u => u.Status, SubscriptionStatus.INACTIVE);

            var updateResult = await _subscriptionsCollection.FindOneAndUpdateAsync(filter, newSubscription);

            return updateResult.Id ?? throw new ArgumentException($"Not found subscription with id = {subscriptionId}");
        }

        /// <summary>
        /// Use only for first setup
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task SetUpUsage(string userId, string planId)
        {
            var filter = Builders<MongoPlan>.Filter.Where(p => p.Id == planId);
            var plan = (await _plansCollection.FindAsync(filter)).FirstOrDefault();
            await _usageCollection.InsertOneAsync(new MongoUsage { UserId = userId, SurveyUsage = plan.Limits!.SurveyLimit, RollbackUsage = plan.Limits!.RollbackLimit });
        }

        public async Task SetUsage(string userId, string planId)
        {
            var planFilter = Builders<MongoPlan>.Filter.Where(p => p.Id == planId);
            var plan = (await _plansCollection.FindAsync(planFilter)).FirstOrDefault();
            var usageFilter = Builders<MongoUsage>.Filter.Where(p => p.UserId == userId);

            var newUsage = Builders<MongoUsage>.Update.Set(u => u.RollbackUsage, plan.Limits?.RollbackLimit ?? 0)
                                                      .Set(u => u.SurveyUsage, plan.Limits?.SurveyLimit ?? 0);
            await _usageCollection.FindOneAndUpdateAsync(usageFilter, newUsage);
        }

        public async Task<Usage> GetUsage(string userId)
        {
            var filter = Builders<MongoUsage>.Filter.Where(u => u.UserId == userId);
            var usage = (await _usageCollection.FindAsync(filter)).FirstOrDefault();
            return usage.Convert() ?? throw new FormatException($"Can't get usage for user {userId}");
        }

        public async Task<Usage> DecrementUsage(string userId, LimitKind limitKind)
        {
            var usage = await GetUsage(userId);

            switch(limitKind)
            {
                case LimitKind.Rollback:
                    usage.RollbackUsage -= 1;
                    break;
                case LimitKind.Survey:
                    usage.SurveyUsage -= 1;
                    break;
                default:
                    throw new FormatException($"Unknown LimitKind {limitKind}");
            }

            if(usage.RollbackUsage < 0 || usage.SurveyUsage < 0)
            {
                throw new ArgumentException($"Can't decrement usage {limitKind} for user {userId} because it empty");
            }
            else
            {
                var filter = Builders<MongoUsage>.Filter.Where(u => u.UserId == userId);
                var newUsage = Builders<MongoUsage>.Update.Set(u => u.RollbackUsage, usage.RollbackUsage)
                                                          .Set(u => u.SurveyUsage, usage.SurveyUsage);
                await _usageCollection.FindOneAndUpdateAsync<MongoUsage>(filter, newUsage);
            }

            return usage;
        }


        public async Task<Plan> GetPlan(string planId)
        {
            var filter = Builders<MongoPlan>.Filter.Where(u => u.Id == planId);
            var plan = (await _plansCollection.FindAsync(filter)).FirstOrDefault();
            if(plan != null)
            {
                return plan.Convert();
            }

            throw new ArgumentException($"Plan with planId {planId} not exists");
        }

        public async Task<Subscription> GetSubscription(string subscriptionId)
        {
            var filter = Builders<MongoSubscription>.Filter.Where(u => u.Id == subscriptionId);
            var subscription = (await _subscriptionsCollection.FindAsync(filter)).FirstOrDefault();
            if (subscription != null)
            {
                return subscription.Convert();
            }

            throw new ArgumentException($"Subscription with subscriptionId {subscriptionId} not exists");
        }

        public async Task<bool> CheckInvoiceExist(int invoice)
        {
            var filter = Builders<MongoSubscription>.Filter.Where(u => u.InvoiceId == invoice);
            var subscription = (await _subscriptionsCollection.FindAsync(filter)).FirstOrDefault();
            
            return subscription != null;
        }

        private DateTime GetExpirationDateFromNow(MongoPlan plan, DateTime creationDate)
        {
            switch (plan.BillingPeriod)
            {
                case DomainObjects.Subscription.BillingPeriod.d:
                    return creationDate.AddDays(plan.BillingUnit);
                case DomainObjects.Subscription.BillingPeriod.m:
                    return creationDate.AddMonths(plan.BillingUnit);
                case DomainObjects.Subscription.BillingPeriod.y:
                    return creationDate.AddYears(plan.BillingUnit);
                default:
                    return creationDate.AddMonths(1);
            }
        }

        private void ApplySale(MongoPlan plan, decimal sale)
        {
            plan.Price -= sale;
        }

        public async Task<BillingPromocode?> GetPromocode(string promocode)
        {
            var filter = Builders<MongoPromocode>.Filter.Where(u => u.Promocode == promocode);
            var promoModel = (await _promocodeCollection.FindAsync(filter)).FirstOrDefault();

            return promoModel?.Convert();
        }
    }
}
