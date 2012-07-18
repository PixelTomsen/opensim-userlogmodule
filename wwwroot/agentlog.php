<?PHP
//Developer: Pixel Tomsen (chk) (pixel.tomsen [at] gridnet.info)
//Function : PHP Port for OpenSim-userlogmodule
//Source Tree : https://github.com/PixelTomsen/opensim-userlogmodule

include("include/userlogdb.php");

mysql_connect ($DB_HOST, $DB_USER, $DB_PASSWORD);
mysql_select_db ($DB_NAME);

$xmlrpc_server = xmlrpc_server_create();

//rpc-server register methods
xmlrpc_server_register_method($xmlrpc_server, "userlog_update",
        "userlog_update");

function userlog_update($method_name, $params, $app_data)
{
	processAgentUpdate($params[0]);
}

function processAgentUpdate($hash)
{

	$region_id = mysql_escape_string($hash['region_id']);
	$region_name = mysql_escape_string($hash['region_name']);
	$agent_id = mysql_escape_string($hash['agent_id']);
	$agent_name = mysql_escape_string($hash['agent_name']);
	$agent_pos = mysql_escape_string($hash['agent_position']);
	$agent_ip = mysql_escape_string($hash['agent_ip']);
	$agent_country = mysql_escape_string($hash['agent_country_code']);
	$agent_countryname = mysql_escape_string($hash['agent_country_name']);
	$agent_viewer = mysql_escape_string($hash['agent_viewer']);

	_processAgentTableUpdate($region_id, $agent_id, $agent_name, $agent_pos, $agent_ip, $agent_country, $agent_viewer);

	_processRegionTableUpdate($region_id, $region_name);

	_processCountryTableUpdate($agent_country, $agent_countryname);

	_processViewerTableUpdate($agent_viewer);

	 $response_xml = xmlrpc_encode(array(
         'success' => 'TRUE'
	 ));

	 print $response_xml;
}

function _processAgentTableUpdate($regionid, $agentid, $agentname, $agentpos, $agentip, $agentcountry, $agentviewer)
{
	$stamp = time();

	$sql = "REPLACE INTO " .TB_USERLOG_AGENTS. "(region_id, agent_id, agent_name, agent_pos, agent_ip, agent_country, agent_viewer, agent_time) VALUES ('$regionid','$agentid','$agentname','$agentpos','$agentip','$agentcountry','$agentviewer','$stamp')";

	$result = mysql_query($sql);

	if(!$result)
	{
	 $response_xml = xmlrpc_encode(array(
         'success' => $result,
         'errorMessage' => mysql_error()
	 ));

	 print $response_xml;
         die();
	}
}

function _processRegionTableUpdate($id, $name)
{
	$sql = "REPLACE INTO " .TB_USERLOG_REGIONS. "(region_id, region_name) VALUES ('$id','$name')";

	$result = mysql_query($sql);

	if(!$result)
	{
	 $response_xml = xmlrpc_encode(array(
         'success' => $result,
         'errorMessage' => mysql_error()
	 ));

	 print $response_xml;
         die();
	}
}

function _processCountryTableUpdate($id, $name)
{
	$sql = "REPLACE INTO " .TB_USERLOG_COUNTRYS. "(country_code, country_name) VALUES ('$id','$name')";

	$result = mysql_query($sql);

	if(!$result)
	{
	 $response_xml = xmlrpc_encode(array(
         'success' => $result,
         'errorMessage' => mysql_error()
	 ));

	 print $response_xml;
         die();
	}
}

function _processViewerTableUpdate($viewer)
{
	$sql = "REPLACE INTO " .TB_USERLOG_VIEWERS. "(viewer) VALUES ('$viewer')";

	$result = mysql_query($sql);

	if(!$result)
	{
	 $response_xml = xmlrpc_encode(array(
         'success' => $result,
         'errorMessage' => mysql_error()
	 ));

	 print $response_xml;
         die();
	}
}

$request_xml = $HTTP_RAW_POST_DATA;
xmlrpc_server_call_method($xmlrpc_server, $request_xml, '');
xmlrpc_server_destroy($xmlrpc_server);
?>
