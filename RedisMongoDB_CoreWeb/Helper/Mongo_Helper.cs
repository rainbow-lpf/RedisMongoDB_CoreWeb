using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RedisMongoDB_CoreWeb.Helper
{
    public class Mongo_Helper
    {
        protected static string conmongoSetting = ConfigurationSettings.AppSettings["MongoStrign"].ToString();
        protected static IMongoDatabase _Database;
        protected static IMongoClient _Client;
        private static Mongo_Helper Mongo;
        public Mongo_Helper()
        {
            _Client = new MongoClient(conmongoSetting);
            _Database = _Client.GetDatabase("");
        }
        public static Mongo_Helper Mongodb()
        {
            if (Mongo == null)
            {
                Mongo = new Mongo_Helper();
            }
            return Mongo;
        }
        /// <summary>
        /// 将数据插入进数据库
        /// </summary>
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>
        /// <param name="t">需要插入数据库的具体实体</param>
        /// <param name="collectionName">指定插入的集合</param>
        public static void Insert<T>(T t, string collectionName)
        {
            try
            {
                var mc = _Database.GetCollection<BsonDocument>(collectionName);
                //将实体转换为bson文档
                BsonDocument bd = t.ToBsonDocument();
                mc.InsertOne(bd);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>
        /// <param name="list">需要插入数据的列表</param>
        /// <param name="collectionName">指定要插入的集合</param>
        public static void Insert<T>(List<T> list, string collectionName)
        {
            try
            {
                var mc = _Database.GetCollection<BsonDocument>(collectionName);
                //创建一个空间bson集合
                List<BsonDocument> bsonList = new List<BsonDocument>();
                //批量将数据转为bson格式 并且放进bson文档,lambda表达式
                list.ForEach(t => bsonList.Add(t.ToBsonDocument()));
                //批量插入数据
                mc.InsertMany(bsonList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <typeparam name="T">移除的数据类型</typeparam>
        /// <param name="query">移除的数据条件</param>
        /// <param name="collectionName">指定的集合名词</param>
        public static void Remove<T>(string filedName, string value, string collectionName)
        {
            var mc = _Database.GetCollection<T>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(filedName, value) as FilterDefinition<T>;
            var result = mc.DeleteManyAsync(filter);
            result.Result.DeletedCount.ToString();
            //根据指定查询移除数据
        }
        /// <summary>
        /// 移除实体里面所有的数据
        /// </summary>
        /// <typeparam name="T">移除的数据类型</typeparam>
        /// <param name="collectionName">指定的集合名称</param>
        public static void RemoveAll<T>(string collectionName)
        {
            try
            {
                var collection = _Database.GetCollection<BsonDocument>(collectionName);
                var filter = new BsonDocument();
                var result = collection.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 查询一个集合中的所有数据
        /// </summary>
        /// <typeparam name="T">该集合数据的所属类型</typeparam>
        /// <param name="collectionName">指定集合的名称</param>
        /// <returns>返回一个List列表</returns>
        public static List<T> FindAll<T>(string collectionName)
        {
            var mc = _Database.GetCollection<T>(collectionName);
            var filter = new BsonDocument();
            //以实体方式取出其数据集合
            var mongoCursor = mc.Find(filter);
            //直接转化为List返回
            return mongoCursor.ToList<T>();
        }

        public static List<T> Find<T>(string filedName, string value, string collectionName)
        {
            var mc = _Database.GetCollection<T>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(filedName, value) as FilterDefinition<T>;
            return mc.Find(filter).ToList<T>();
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">更新的数据 所属的类型</typeparam>
        /// <param name="query">更新数据的查询</param>
        /// <param name="update">需要更新的文档</param>
        /// <param name="collectionName">指定更新集合的名称</param>
        public static void Update<T>(T bd,string filedName,string value, string collectionName)
        {
            try
            {
                var mc = _Database.GetCollection<T>(collectionName);
                BsonDocument bson = bd.ToBsonDocument();
                var filter = Builders<BsonDocument>.Filter.Eq(filedName, value) as FilterDefinition<T>;
                var result = mc.UpdateManyAsync(filter, bson);
              
              
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }
    }
}