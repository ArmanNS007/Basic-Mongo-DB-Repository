using CommonTools.Models.Settings;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoTools.Models;
using Pluralize.NET;
using System.Linq.Expressions;

namespace MongoTools.Contracts
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public GenericRepository(MongoDbSettings mongoDbSettings)
        {
            var database = new MongoClient(mongoDbSettings.ConnectionString)
                .GetDatabase(mongoDbSettings.DatabaseName);

            Pluralizer pluralizer = new();
            string pluralizedName = pluralizer.Pluralize(typeof(TEntity).Name);
            _collection = database.GetCollection<TEntity>(pluralizedName);
        }

        #region Async Methods

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await _collection.Find(filterExpression).ToListAsync();
        }

        public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await _collection.Find(filterExpression).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllUserAsync()
        {
            return await _collection.AsQueryable().ToListAsync();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entity)
        {
            await _collection.InsertManyAsync(entity);
        }

        public virtual async Task ReplaceOneAsync(TEntity entity)
        {
            entity.UpdateMoment = DateTime.UtcNow;
            var filter = Builders<TEntity>.Filter.Eq(a => a.Id, entity.Id);
            await _collection.FindOneAndReplaceAsync(filter, entity);
        }

        public async Task DeleteByIdAsync(string Id)
        {
            var filter = Builders<TEntity>.Filter.Eq(a => a.Id, Id);
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task DeleteOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            await _collection.DeleteOneAsync(filterExpression);
        }

        #endregion

        #region Sync Methods

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToList();
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAllUser()
        {
            return _collection.AsQueryable().ToList();
        }

        public void Insert(TEntity user)
        {
            _collection.InsertOne(user);
        }

        public void InsertMany(IEnumerable<TEntity> users)
        {
            _collection.InsertMany(users);
        }

        public virtual void ReplaceOne(TEntity entity)
        {
            entity.UpdateMoment = DateTime.UtcNow;
            var filter = Builders<TEntity>.Filter.Eq(a => a.Id, entity.Id);
            _collection.FindOneAndReplace(filter, entity);
        }

        public void DeleteById(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(a => a.Id, id);
            _collection.FindOneAndDelete(filter);
        }
        public void DeleteOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            _collection.DeleteOne(filterExpression);
        }

        #endregion
    }
}