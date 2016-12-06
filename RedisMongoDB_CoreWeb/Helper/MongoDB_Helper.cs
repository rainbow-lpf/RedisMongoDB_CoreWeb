using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RedisMongoDB_CoreWeb.Helper
{
    public class MongoDB_Helper
    {
        /// <summary>
        /// 数据库的实例
        /// </summary>
        public MongoDatabase _db;

        /// <summary>
        /// 得到数据库服务器
        /// </summary>
        private MongoServer _dbServer;

        /// <summary>
        /// ObjectId的键
        /// </summary>
        private readonly string OBJECTID_KEY = "_id";

        //初始化构造函数
        public MongoDB_Helper(string MONGO_CONN_HOST, string DB_Name)
        {
            this._db = new MongoDB().GetDataBase(MONGO_CONN_HOST, DB_Name);
            this._dbServer = new MongoDB().GetDataBaseServer(MONGO_CONN_HOST);
        }
        /// <summary>
        /// 将数据插入进数据库
        /// </summary>
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>
        /// <param name="t">需要插入数据库的具体实体</param>
        /// <param name="collectionName">指定插入的集合</param>
        public void Insert<T>(T t, string collectionName)
        {
            var mc = this._db.GetCollection<BsonDocument>(collectionName);

            //将实体转换为bson文档
            BsonDocument bd = t.ToBsonDocument();

            //进行插入操作
            WriteConcernResult result = mc.Insert(bd);
            if (!string.IsNullOrEmpty(result.LastErrorMessage))
            {
                throw new Exception(result.LastErrorMessage);
            }

        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>
        /// <param name="list">需要插入数据的列表</param>
        /// <param name="collectionName">指定要插入的集合</param>
        public void Insert<T>(List<T> list, string collectionName)
        {
            var mc = this._db.GetCollection<BsonDocument>(collectionName);

            //创建一个空间bson集合
            List<BsonDocument> bsonList = new List<BsonDocument>();

            //批量将数据转为bson格式 并且放进bson文档,lambda表达式
            list.ForEach(t => bsonList.Add(t.ToBsonDocument()));

            //批量插入数据
            mc.InsertBatch(bsonList);
        }
        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <typeparam name="T">移除的数据类型</typeparam>
        /// <param name="query">移除的数据条件</param>
        /// <param name="collectionName">指定的集合名词</param>
        public void Remove<T>(IMongoQuery query, string collectionName)
        {
            var mc = this._db.GetCollection<T>(collectionName);

            query = this.InitQuery(query);
            //根据指定查询移除数据
            mc.Remove(query);
        }

        /// <summary>
        /// 移除实体里面所有的数据
        /// </summary>
        /// <typeparam name="T">移除的数据类型</typeparam>
        /// <param name="collectionName">指定的集合名称</param>
        public void RemoveAll<T>(string collectionName)
        {
            this.Remove<T>(null, collectionName);
        }
        /// <summary>
        /// 查询一个集合中的所有数据
        /// </summary>
        /// <typeparam name="T">该集合数据的所属类型</typeparam>
        /// <param name="collectionName">指定集合的名称</param>
        /// <returns>返回一个List列表</returns>
        public List<T> FindAll<T>(string collectionName)
        {
            var mc = this._db.GetCollection<T>(collectionName);
            //以实体方式取出其数据集合
            var mongoCursor = mc.FindAll();
            //直接转化为List返回
            return mongoCursor.ToList<T>();
        }



        /// <summary>
        /// 查询指定字段的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collectionName">数据表名称</param>
        /// <param name="fd">字段区间定义</param>
        /// <returns></returns>
        public List<T> FindAll<T>(string collectionName, FieldsDocument fd)
        {
            var mc = this._db.GetCollection<T>(collectionName);
            //以实体方式取出其数据集合
            var mongoCursor = mc.FindAll().SetFields(fd);
            //直接转化为List返回
            return mongoCursor.ToList<T>();
        }
        /// <summary>
        /// 查询一条记录
        /// </summary>
        /// <typeparam name="T">该数据所属的类型</typeparam>
        /// <param name="query">查询的条件 可以为空</param>
        /// <param name="collectionName">去指定查询的集合</param>
        /// <returns>返回一个实体类型</returns>
        public T FindOne<T>(IMongoQuery query, string collectionName)
        {
            MongoCollection<T> mc = this._db.GetCollection<T>(collectionName);
            query = this.InitQuery(query);
            T t = mc.FindOne(query);
            return t;
        }

        /// <summary>
        /// 根据指定条件查询集合中的多条数据记录
        /// </summary>
        /// <typeparam name="T">该集合数据的所属类型</typeparam>
        /// <param name="query">指定的查询条件 比如Query.And(Query.EQ("username","admin"),Query.EQ("password":"admin"))</param>
        /// <param name="collectionName">指定的集合的名称</param>
        /// <returns>返回一个List列表</returns>
        public List<T> Find<T>(IMongoQuery query, string collectionName)
        {
            MongoCollection<T> mc = this._db.GetCollection<T>(collectionName);
            query = this.InitQuery(query);

            MongoCursor<T> mongoCursor = mc.Find(query);

            return mongoCursor.ToList<T>();
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">更新的数据 所属的类型</typeparam>
        /// <param name="query">更新数据的查询</param>
        /// <param name="update">需要更新的文档</param>
        /// <param name="collectionName">指定更新集合的名称</param>
        public void Update<T>(IMongoQuery query, BsonDocument bd, string collectionName)
        {

            var mc = this._db.GetCollection<T>(collectionName);
            query = this.Update(query);

            mc.Update(query, new UpdateDocument(bd));
        }

    }
}