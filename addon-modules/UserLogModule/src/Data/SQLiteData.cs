/*
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
using System.Text;
using System.Reflection;
using log4net;
using Mono.Data.SqliteClient;
using OpenSim.Framework;

using Migration = OpenSim.Data.Migration;

namespace OpenSim.Region.UserLogModule.Data
{
    public class SQLiteData : IUserStatsData
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string m_connectionString = String.Empty;

        protected virtual Assembly Assembly
        {
            get { return GetType().Assembly; }
        }

        public SQLiteData(string connectionString)
        {
            Initialise(connectionString);
        }

        public void Initialise(string connectionString)
        {
            m_connectionString = connectionString;

            m_log.DebugFormat("[UserLogModule]: Sqlite - connecting: {0}", m_connectionString);

            try
            {
                if (Util.IsWindows())
                    Util.LoadArchSpecificWindowsDll("sqlite3.dll");

                using (SqliteConnection connection = new SqliteConnection(m_connectionString))
                {
                    connection.Open();

                    Migration m = new Migration(connection, Assembly, "sqlite");
                    m.Update();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                m_log.ErrorFormat("[UserLogModule]: Initial sqlite exception for URI '{0}', Exception: {1}", m_connectionString, ex.Message);
                Environment.Exit(-1);
            }
        }

        public void StoreAgentLoginData(UserLogAgentData AgentData)
        {
            try
            {
                UpdateAgentTable(AgentData);
            }
            catch (Exception ex)
            {
                m_log.ErrorFormat("[UserLogModule]: SQLite Exception: {0}", ex);
            }
        }

        public void StoreAgentViewerData(UserLogAgentViewerData ViewerData)
        {
            m_log.WarnFormat("[UserLogModule]: Sqlite Data-Storage not supported.");
        }


        private void UpdateAgentTable(UserLogAgentData agentData)
        {
            lock (this)
            {
                using (SqliteConnection connection = new SqliteConnection(m_connectionString))
                {
                    connection.Open();

                    using (SqliteCommand cmd = new SqliteCommand(
                        "REPLACE INTO userlog_agent (region_id, agent_id, agent_name, " +
                        "agent_pos, agent_ip, agent_country, agent_viewer,agent_grid, agent_time) " +
                        "VALUES (" +
                        ":region_id, :agent_id, :agent_name, :agent_pos, :agent_ip, :agent_country, " +
                        ":agent_viewer, :agent_grid, :agent_time)", connection))
                    {
                        cmd.Parameters.Add(":region_id", agentData.RegionID.ToString());
                        cmd.Parameters.Add(":region_name", agentData.RegionName);
                        cmd.Parameters.Add(":agent_id", agentData.ID.ToString());
                        cmd.Parameters.Add(":agent_name", agentData.Name);
                        cmd.Parameters.Add(":agent_pos", agentData.Position);
                        cmd.Parameters.Add(":agent_ip", agentData.IP);
                        cmd.Parameters.Add(":agent_country", agentData.CountryCode);
                        cmd.Parameters.Add(":country_name", agentData.CountryName);
                        cmd.Parameters.Add(":agent_viewer", agentData.Viewer);
                        cmd.Parameters.Add(":agent_grid", agentData.Grid);
                        cmd.Parameters.Add(":agent_time", Util.UnixTimeSinceEpoch().ToString());

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "REPLACE INTO userlog_country (country_code, country_name) " +
                            "VALUES (" +
                            ":agent_country, :country_name)";

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "REPLACE INTO userlog_region (region_id, region_name) " +
                            "VALUES (" +
                            ":region_id, :region_name)";

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "REPLACE INTO userlog_viewer (viewer) " +
                             "VALUES (" +
                             ":agent_viewer)";

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
            }
        }
    }
}
