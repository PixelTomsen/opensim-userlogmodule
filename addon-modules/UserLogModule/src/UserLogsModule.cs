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
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Framework.Servers;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using log4net;
using Nini.Config;
using Nwc.XmlRpc;
using Mono.Addins;

using OpenSim.Region.UserLogModule.Data;
//using Caps = OpenSim.Framework.Capabilities.Caps;

[assembly: Addin("UserLogModule", "0.1")]
[assembly: AddinDependency("OpenSim", "0.5")]

namespace OpenSim.Region.UserLogModule
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "UserLogModule")]
    public class UserLogsModule : IUserStatsLogModule, ISharedRegionModule
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IConfig m_config = null;
        private IUserStatsData m_dataStore = null;
        private AgentCountry m_agentCountry = null;

        private List<Scene> m_Scenes = new List<Scene>();
        private bool m_Enabled = false;

        //        private bool m_ViewerStatsLog = false;

        //        private string capsBase = "/CAPS/VS/";

        #region /// ISharedRegion
        public void Initialise(IConfigSource config)
        {
            m_config = config.Configs["UserLogModule"];

            if (m_Scenes.Count == 0)
            {
                if (!CheckConfig())
                {
                    m_Enabled = false;
                    m_log.InfoFormat("[{0}]: Module is disabled.", Name);
                    return;
                }

                m_log.InfoFormat("[{0}]: Module is enabled.", Name);
                m_Enabled = true;
            }
        }

        public void RegionLoaded(Scene scene)
        {
            if (!m_Enabled)
                return;

            scene.RegisterModuleInterface<IUserStatsLogModule>(this);
            scene.EventManager.OnMakeRootAgent += OnMakeRootAgent;

            //if (m_ViewerStatsLog)
            //{
            //    scene.EventManager.OnRegisterCaps += OnRegisterCaps;
            //scene.EventManager.OnNewClient += OnNewClient;
            //scene.EventManager.OnClientClosed += OnClientClosed;
            // }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void PostInitialise()
        {
        }

        public void AddRegion(Scene scene)
        {
            if (!m_Enabled)
                return;

            lock (m_Scenes)
            {
                m_Scenes.Add(scene);
            }
        }

        public void RemoveRegion(Scene scene)
        {
            if (!m_Enabled)
                return;

            lock (m_Scenes)
            {
                m_Scenes.Remove(scene);
            }

            scene.UnregisterModuleInterface<IUserStatsLogModule>(this);
        }

        public string Name
        {
            get { return "UserLogModule"; }
        }

        public void Close()
        {
        }
        #endregion

        private bool CheckConfig()
        {
            if (null == m_config || !m_config.GetBoolean("enabled", false))
                return false;

            string db = m_config.GetString("DataBase", String.Empty);
            string conn = m_config.GetString("ConnectionString", String.Empty);

            if (String.IsNullOrEmpty(db) || String.IsNullOrEmpty(conn))
                return false;

            //            m_ViewerStatsLog = m_config.GetBoolean("ViewerStatsLog", false);

            switch (db)
            {
                case "web":
                    m_dataStore = new XmlRpcData(conn);
                    break;
                case "mysql":
                    m_dataStore = new MySQLData(conn);
                    break;
                case "sqlite":
                    m_dataStore = new SQLiteData(conn);
                    break;
                default:
                    return false;
            }

            m_agentCountry = new AgentCountry();

            return true;
        }

        #region /// Events
        private void OnMakeRootAgent(ScenePresence sp)
        {
            AgentRegionLoginLog(sp);
        }

        /*
                private void OnRegisterCaps(UUID agentID, Caps caps)
                {
        //            m_log.DebugFormat("[{0}]: OnRegisterCaps: agentID {1} caps {2}",Name, agentID, caps);

                    caps.RegisterHandler(
                        "ViewerStats",
                        new RestStreamHandler(
                            "POST",
                            capsBase + UUID.Random(),
                            (request, path, param, httpRequest, httpResponse)
                                => ViewerStatsReport(request, path, param, agentID, caps),
                            "ViewerStats",
                            agentID.ToString()));
                }
         */
        #endregion

        private void AgentRegionLoginLog(ScenePresence sp)
        {
            UserLogAgentData agentData = new UserLogAgentData();

            try
            {
                ClientInfo ci = sp.ControllingClient.GetClientInfo();
                IPEndPoint ip = sp.ControllingClient.RemoteEndPoint;

                agentData.RegionID = sp.Scene.RegionInfo.RegionID;
                agentData.RegionName = sp.Scene.RegionInfo.RegionName;
                agentData.Position = string.Format("/{0:F2}/{1:F2}/{2:F2}", sp.AbsolutePosition.X, sp.AbsolutePosition.Y, sp.AbsolutePosition.Z);
                agentData.ID = ci.agentcircuit.AgentID;
                agentData.Name = string.Format("{0} {1}",ci.agentcircuit.firstname, ci.agentcircuit.lastname);
                agentData.IP = ip.Address.ToString();
                agentData.Viewer = sp.Viewer;
                agentData.CountryCode = m_agentCountry.LookupCountryCode(ip.Address);
                agentData.CountryName = m_agentCountry.LookupCountryName(ip.Address);
            }
            catch (Exception ex)
            {
                m_log.ErrorFormat("[{0}]: Exception on received agentdata - {1} : {2}",Name, ex.Message, ex.StackTrace);
                Util.PrintCallStack();
                return;
            }

            if (m_dataStore == null)
            {
                m_log.WarnFormat("[{0}]: No data-storage is aviable.", Name);
                return;
            }

            m_dataStore.StoreAgentLoginData(agentData);
        }

    }
}
