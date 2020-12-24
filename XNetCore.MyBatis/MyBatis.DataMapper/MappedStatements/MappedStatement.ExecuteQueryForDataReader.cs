#region Apache Notice
/*****************************************************************************
 * $Revision: 374175 $
 * $LastChangedDate: 2008-10-11 12:07:44 -0400 (Sat, 11 Oct 2008) $
 * $LastChangedBy: gbayon $
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

using System.Data;
using MyBatis.DataMapper.Scope;
using MyBatis.DataMapper.Session;

namespace MyBatis.DataMapper.MappedStatements
{
    public partial class MappedStatement
    {
        #region ExecuteQueryForDataReader
        /// <summary>
        /// ExecuteQueryForDataReader
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The IDataReader</returns>
        public virtual IDataReader ExecuteQueryForDataReader(ISession session, object parameterObject)
        {
            return Execute(PreSelectEventKey, PostSelectEventKey, session, parameterObject,
                (r, p) => RunQueryForDataReader(r, session, parameterObject));

            // return RaisePostEvent<DataTable, PostSelectEventArgs>(PostSelectEventKey, param, dataTable);
        }

        /// <summary>
        /// Runs the query for for data table.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="session">The session.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns></returns>
        internal IDataReader RunQueryForDataReader(RequestScope request, ISession session, object parameterObject)
        {
            IDbCommand command = request.IDbCommand;
            IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }
        #endregion
    }
}
