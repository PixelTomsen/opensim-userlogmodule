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
using System.Text;
using System.Xml;

using log4net;
using Nwc.XmlRpc;

namespace OpenSim.Region.UserLogModule.Data
{
    public class XmlRpcData : IUserStatsData
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string m_connectionString = String.Empty;

        public XmlRpcData(string connectionString)
        {
            Initialise(connectionString);
        }

        public void Initialise(string connectionString)
        {
            m_connectionString = connectionString;
        }

        public void StoreAgentLoginData(UserLogAgentData AgentData)
        {
            Hashtable ReqHash = new Hashtable();

            ReqHash["region_id"] = AgentData.RegionID.ToString();
            ReqHash["region_name"] = AgentData.RegionName;
            ReqHash["agent_position"] = AgentData.Position;
            ReqHash["agent_id"] = AgentData.ID.ToString();
            ReqHash["agent_name"] = AgentData.Name;
            ReqHash["agent_ip"] = AgentData.IP;
            ReqHash["agent_country_code"] = AgentData.CountryCode;
            ReqHash["agent_country_name"] = AgentData.CountryName;
            ReqHash["agent_viewer"] = AgentData.Viewer;
            ReqHash["agent_grid"] = AgentData.Grid;

            XMLRPCRequest(ReqHash, "userlog_update");
        }

        public void StoreAgentViewerData(UserLogAgentViewerData ViewerData)
        {
        }

        private Hashtable XMLRPCRequest(Hashtable ReqParams, string method)
        {
            ArrayList SendParams = new ArrayList();
            SendParams.Add(ReqParams);

            XmlRpcResponse Resp;
            try
            {
                XmlRpcRequest Req = new XmlRpcRequest(method, SendParams);
                Resp = Req.Send(m_connectionString, 30000);
            }
            catch (WebException ex)
            {
                m_log.ErrorFormat(

                    "[UserLogModule]: XmlRpcDataConnector::WebException > Url:{0}, Method: {1}, Params: {2}, " +
                    "Exception: {3}", m_connectionString, method, SendParams.ToString(), ex);

                Hashtable ErrorHash = new Hashtable();
                ErrorHash["success"] = false;
                ErrorHash["errorMessage"] = "Unable to log User data at this time. ";
                ErrorHash["errorURI"] = "";

                return ErrorHash;
            }
            catch (SocketException ex)
            {
                m_log.ErrorFormat(
                        "[UserLogModule]: XmlRpcDataConnector::SocketException > Url:{0}, Method: {1}, Params: {2}, " +
                        "Exception: {3}", m_connectionString, method, SendParams.ToString(), ex);

                Hashtable ErrorHash = new Hashtable();
                ErrorHash["success"] = false;
                ErrorHash["errorMessage"] = "Unable to log user data at this time. ";
                ErrorHash["errorURI"] = "";

                return ErrorHash;
            }
            catch (XmlException ex)
            {
                m_log.ErrorFormat(
                        "[UserLogModule]: XmlRpcDataConnector::XmlException > Url:{0}, Method: {1}, Params: {2}, " +
                        "Exception: {3}", m_connectionString, method, SendParams.ToString(), ex);

                Hashtable ErrorHash = new Hashtable();
                ErrorHash["success"] = false;
                ErrorHash["errorMessage"] = "Unable to log user data at this time. ";
                ErrorHash["errorURI"] = "";

                return ErrorHash;
            }
            if (Resp.IsFault)
            {
                Hashtable ErrorHash = new Hashtable();
                ErrorHash["success"] = false;
                ErrorHash["errorMessage"] = "Unable to log user data at this time. ";
                ErrorHash["errorURI"] = "";
                return ErrorHash;
            }

            Hashtable RespData = (Hashtable)Resp.Value;
            return RespData;
        }
    }
}
