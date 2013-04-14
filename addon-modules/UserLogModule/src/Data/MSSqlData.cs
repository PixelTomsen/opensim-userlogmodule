﻿/*
 * Pixel Tomsen 2012 (pixel.tomsen [at] gridnet.info)
 *
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using log4net;
using OpenSim.Framework;
using OpenSim.Data;
using OpenSim.Data.MSSQL;

using Migration = OpenSim.Data.Migration;

namespace OpenSim.Region.UserLogModule.Data
{
    public class MSSqlData : IUserStatsData
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string m_connectionString = String.Empty;

        private MSSQLManager m_database = null;

        protected virtual Assembly Assembly
        {
            get { return GetType().Assembly; }
        }

        public MSSqlData(string connectionString)
        {
            Initialise(connectionString);
        }

        public void Initialise(string connectionString)
        {
            m_connectionString = connectionString;
            m_log.DebugFormat("[UserLogModule]: Mssql - connecting: {0}", m_connectionString);

            try
            {

                m_database = new MSSQLManager(connectionString);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Migration m = new Migration(conn, Assembly, "mssql");
                    m.Update();
                }
            }
            catch (Exception ex)
            {
                m_log.ErrorFormat("[UserLogModule]: Initial mssql exception for URI '{0}', Exception: {1}", m_connectionString, ex.Message);
                Environment.Exit(-1);
            }
        }

        public void StoreAgentLoginData(UserLogAgentData AgentData)
        {
            m_log.WarnFormat("[UserLogModule]: Mssql Data-Storage not supported.");
        }

        public void StoreAgentViewerData(UserLogAgentViewerData ViewerData)
        {
            m_log.WarnFormat("[UserLogModule]: Mssql Data-Storage not supported.");
        }

    }
}
