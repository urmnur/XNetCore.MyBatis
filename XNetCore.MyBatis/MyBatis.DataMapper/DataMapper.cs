﻿
#region Apache Notice
/*****************************************************************************
 * $Header: $
 * $Revision: 513043 $
 * $Date: 2009-06-28 01:03:34 -0600 (Sun, 28 Jun 2009) $
 * 
 * iBATIS.NET Data Mapper
 * Copyright (C) 2008/2005 - The Apache Software Foundation
 *  
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Data;
using MyBatis.DataMapper.Session;
using MyBatis.DataMapper.Model;
using MyBatis.DataMapper.MappedStatements;
using MyBatis.DataMapper.Exceptions;
using MyBatis.Common.Contracts;
using MyBatis.Common.Contracts.Constraints;

namespace MyBatis.DataMapper
{
    /// <summary>
    /// The default implementation of the <see cref="IDataMapper"/>  
    /// </summary>
    public class DataMapper : IDataMapper, IModelStoreAccessor
    {
        private readonly IModelStore modelStore = null;
        private readonly ISessionStore sessionStore = null;
        private readonly ISessionFactory sessionFactory = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        /// <param name="modelStore">The model store.</param>
        public DataMapper(IModelStore modelStore)
        {
            Contract.Require.That(modelStore, Is.Not.Null).When("retrieving argument modelStore in DataMapper constructor");

            this.modelStore = modelStore;
            this.modelStore.DataMapper = this;
            sessionStore = modelStore.SessionStore;
            sessionFactory = modelStore.SessionFactory;
        }

        #region IDataMapper Members

        /// <summary>
        /// Executes a Sql INSERT statement.
        /// Insert is a bit different from other update methods, as it
        /// provides facilities for returning the primary key of the
        /// newly inserted row (rather than the effected rows).  This
        /// functionality is of course optional.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the INSERT values.
        /// </summary>
        /// <param name="statementId">The name of the statement to execute.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>
        /// The primary key of the newly inserted row.
        /// This might be automatically generated by the RDBMS,
        /// or selected from a sequence table or other source.
        /// </returns>
        public object Insert(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteInsert(sessionScope.Session, parameterObject);
            }
        }

        /// <summary>
        /// Alias to QueryForMap, .NET spirit.
        /// Feature idea by Ted Husted.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        /// <exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty)
        {
            return QueryForMap(statementName, parameterObject, keyProperty, valueProperty);
        }

        /// <summary>
        /// Alias to QueryForMap, .NET spirit.
        /// Feature idea by Ted Husted.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty)
        {
            return QueryForMap(statementName, parameterObject, keyProperty);
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An Ilist object used to hold the objects.</param>
        public void QueryForList(string statementId, object parameterObject, IList resultObject)
        {
            if (resultObject == null)
            {
                throw new DataMapperException("resultObject parameter must be instantiated before being passed to QueryForList");
            }

            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                statement.ExecuteQueryForList(sessionScope.Session, parameterObject, resultObject);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        public IList QueryForList(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForList(sessionScope.Session, parameterObject);
            }
        }


        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the entire result object.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty)
        {
            return QueryForMap(statementName, parameterObject, keyProperty, null);
        }

        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the value of the property specified
        /// in the valueProperty parameter.  If valueProperty is null, the entire result object will be entered.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        /// <exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary QueryForMap(string statementId, object parameterObject, string keyProperty, string valueProperty)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForMap(sessionScope.Session, parameterObject, keyProperty, valueProperty);
            }
        }

        /// <summary>
        /// Runs a query with a custom object that gets a chance to deal
        /// with each row as it is processed.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <param name="rowDelegate">The row delegate.</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        /// <exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary QueryForMapWithRowDelegate(string statementId, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForMapWithRowDelegate(sessionScope.Session, parameterObject, keyProperty, valueProperty, rowDelegate);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns a single object of the type of the
        /// resultObject parameter.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An object of the type to be returned.</param>
        /// <returns>
        /// The single result object populated with the result set data.
        /// </returns>
        public object QueryForObject(string statementId, object parameterObject, object resultObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForObject(sessionScope.Session, parameterObject, resultObject);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns that returns data
        /// to populate a single object instance.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>
        /// The single result object populated with the result set data.
        /// </returns>
        public object QueryForObject(string statementId, object parameterObject)
        {
            return QueryForObject(statementId, parameterObject, null);
        }

        /// <summary>
        /// Runs a query for list with a custom object that gets a chance to deal
        /// with each row as it is processed.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate">The row delegate.</param>
        /// <returns>A List of result objects.</returns>
        public IList QueryWithRowDelegate(string statementId, object parameterObject, RowDelegate rowDelegate)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForRowDelegate(sessionScope.Session, parameterObject, rowDelegate);
            }
        }

        /// <summary>
        /// Executes a Sql UPDATE statement.
        /// Update can also be used for any other update statement type,
        /// such as inserts and deletes.  Update returns the number of
        /// rows effected.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the UPDATE values as well as the WHERE clause parameter(s).
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>The number of rows effected.</returns>
        public int Update(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteUpdate(sessionScope.Session, parameterObject);
            }
        }

        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the value of the property specified
        /// in the valueProperty parameter.  If valueProperty is null, the entire result object will be entered.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>
        /// A IDictionary of object containing the rows keyed by keyProperty.
        /// </returns>
        /// <exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary<K, V> QueryForDictionary<K, V>(string statementId, object parameterObject, string keyProperty, string valueProperty)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForDictionary<K, V>(sessionScope.Session, parameterObject, keyProperty, valueProperty);
            }
        }

        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the entire result object.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>
        /// A IDictionary of object containing the rows keyed by keyProperty.
        /// </returns>
        public IDictionary<K, V> QueryForDictionary<K, V>(string statementId, object parameterObject, string keyProperty)
        {
            return QueryForDictionary<K, V>(statementId, parameterObject, keyProperty, null);
        }

        /// <summary>
        /// Runs a query with a custom object that gets a chance to deal
        /// with each row as it is processed.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <param name="rowDelegate">A delegate called once per row in the QueryForDictionary method&gt;</param>
        /// <returns>
        /// A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.
        /// </returns>
        /// <exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary<K, V> QueryForDictionary<K, V>(string statementId, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate<K, V> rowDelegate)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForDictionary(sessionScope.Session, parameterObject, keyProperty, valueProperty, rowDelegate);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns a single object of the type of the
        /// resultObject parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="instanceObject">An object of the type to be returned.</param>
        /// <returns>
        /// The single result object populated with the result set data.
        /// </returns>
        public T QueryForObject<T>(string statementId, object parameterObject, T instanceObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForObject(sessionScope.Session, parameterObject, instanceObject);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns that returns data
        /// to populate a single object instance.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>
        /// The single result object populated with the result set data.
        /// </returns>
        public T QueryForObject<T>(string statementId, object parameterObject)
        {
            return QueryForObject(statementId, parameterObject, default(T));
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        public IList<T> QueryForList<T>(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForList<T>(sessionScope.Session, parameterObject);
            }
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An Ilist object used to hold the objects.</param>
        public void QueryForList<T>(string statementId, object parameterObject, IList<T> resultObject)
        {
            if (resultObject == null)
            {
                throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
            }

            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                statement.ExecuteQueryForList(sessionScope.Session, parameterObject, resultObject);
            }
        }


        /// <summary>
        /// Runs a query for list with a custom object that gets a chance to deal
        /// with each row as it is processed.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate">The row delegate.</param>
        /// <returns>A List of result objects.</returns>
        public IList<T> QueryWithRowDelegate<T>(string statementId, object parameterObject, RowDelegate<T> rowDelegate)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForRowDelegate(sessionScope.Session, parameterObject, rowDelegate);
            }
        }

        /// <summary>
        /// Executes a Sql DELETE statement.
        /// Delete returns the number of rows effected.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>The number of rows effected.</returns>
        public int Delete(string statementId, object parameterObject)
        {
            return Update(statementId, parameterObject);
        }

        /// <summary>
        /// Executes a SQL SELECT statement that returns  data
        /// to populate a DataTable.
        /// If a resultMap is specified, the column name will be the result property name.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>A DataTable</returns>
        public DataTable QueryForDataTable(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForDataTable(sessionScope.Session, parameterObject);
            }
        }

        /// <summary>
        /// 获取 DataSet
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>A DataSet</returns>
        public DataSet QueryForDataSet(string statementId, object parameterObject)
        {
            using (var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory))
            {
                IMappedStatement statement = GetMappedStatement(statementId);
                return statement.ExecuteQueryForDataSet(sessionScope.Session, parameterObject);
            }
        }

        /// <summary>
        /// 获取 DataReader
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>A DataReader</returns>
        public IDataReader QueryForDataReader(string statementId, object parameterObject)
        {
            var sessionScope = new DataMapperLocalSessionScope(sessionStore, sessionFactory);
            IMappedStatement statement = GetMappedStatement(statementId);
            return statement.ExecuteQueryForDataReader(sessionScope.Session, parameterObject);
        }
        #endregion

        #region IModelStoreAccessor Members

        /// <summary>
        /// Gets the <see cref="IModelStore"/>
        /// </summary>
        /// <value>The model store.</value>
        IModelStore IModelStoreAccessor.ModelStore
        {
            get { return modelStore; }
        }

        #endregion

        private IMappedStatement GetMappedStatement(string statementId)
        {
            return modelStore.GetMappedStatement(statementId);
        }

    }
}
