;Developer: Pixel Tomsen (Christian Kurzhals / www.ckinfo.de) (pixel.tomsen [at] gridnet.info)
;
;Function : Agent Log-Module for Different Database-Storages [OpenSim-RegionServer-AddOn]
;
;Source Tree : https://github.com/PixelTomsen/opensim-userlogmodule
;
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
*
;

changes from old Repo : https://github.com/PixelTomsen/opensim-dev-external/tree/master/opensim/addon-modules/OpenSimUserLog

- add AgentIP2Country logging
- preparing for different Storages
- Viewer-Logging
- different tables for stats-calculations


ToDo :
- copy this Folder to source-folder-of-opensim/addon-modules
- run runprebuild.bat (msvc) or runprebuild.sh (mono-linux)
- run compile (msvc) or xbuild (linux-mono)

- for logging to web-storage prepare your webserver (database and files) for action (files in wwwroot)


:Add following Lines to OpenSim.ini or see file OpenSim-addition.ini.example:



[UserLogModule]

 enabled = true

 ;;
 ;; current only web, mysql and sqlite storage implemented
 ;;
 
 DataBase = web
 ConnectionString = "http://localhost/log/agentlog.php"

 ;DataBase = mysql
 ;ConnectionString = "Data Source=localhost;Database=userlog;User ID=opensim;Password=***;"

 ;DataBase = mssql
 ;ConnectionString = "Server=localhost\SQLEXPRESS;Database=userlog;User Id=opensim; password=***;"

 ;DataBase = sqlite
 ;ConnectionString = "URI=file:UserLog.db,version=3,UseUTF16Encoding=True"