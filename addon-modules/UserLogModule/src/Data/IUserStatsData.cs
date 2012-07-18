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
using System.Text;

using OpenMetaverse;

namespace OpenSim.Region.UserLogModule
{
    public interface IUserStatsData
    {
        void Initialise(string connectionString);
        void StoreAgentLoginData(UserLogAgentData AgentData);
        void StoreAgentViewerData(UserLogAgentViewerData ViewerData);
    }

    /// <summary>
    /// Region-Login Data
    /// </summary>
    ///
    public class UserLogAgentData
    {
        public UUID ID { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string Viewer { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Position { get; set; }
        public UUID RegionID { get; set; }
        public string RegionName { get; set; }
    }

    /// <summary>
    /// ViewerStatsCaps Log
    /// </summary>
    ///
    public class UserLogAgentViewerData
    {
        public UUID AgentID { get; set; }
        public string ViewerLanguage { get; set; }
        public string Cpu { get; set; }
        public string Gpu { get; set; }
        public string Os { get; set; }
        public string Ram { get; set; }
        public int RegionVisited { get; set; }
        public uint StartTime { get; set; }
        public uint RunTime { get; set; }
    }

}

