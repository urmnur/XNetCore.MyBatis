﻿
#region Apache Notice
/*****************************************************************************
 * $Revision: 408164 $
 * $LastChangedDate: 2008-06-28 09:26:16 -0600 (Sat, 28 Jun 2008) $
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

using System;
using System.Xml.Serialization;
using XNetCore.MyBatis.DataMapper.Model.Sql.Dynamic.Handlers;
using XNetCore.MyBatis.Common.Utilities.Objects.Members;

namespace XNetCore.MyBatis.DataMapper.Model.Sql.Dynamic.Elements
{
	/// <summary>
	/// Represent an isLessEqual sql tag element.
	/// </summary>
	[Serializable]
	[XmlRoot("isLessEqual", Namespace="http://ibatis.apache.org/mapping")]
	public sealed class IsLessEqual : Conditional
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="IsLessEqual"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsLessEqual(AccessorFactory accessorFactory)
		{
            this.Handler = new IsLessEqualTagHandler(accessorFactory);
		}
	}
}
