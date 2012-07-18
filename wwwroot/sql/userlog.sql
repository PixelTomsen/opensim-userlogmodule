CREATE TABLE IF NOT EXISTS `userlog_agent` (
  `region_id` varchar(255) NOT NULL,
  `agent_id` varchar(64) NOT NULL,
  `agent_name` varchar(128) NOT NULL,
  `agent_pos` varchar(255) NOT NULL,
  `agent_ip` varchar(255) NOT NULL,
  `agent_country` varchar(255) NOT NULL,
  `agent_viewer` varchar(255) NOT NULL,
  `agent_time` bigint(20) NOT NULL,
  PRIMARY KEY (`agent_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `userlog_region` (
  `region_id` varchar(64) NOT NULL,
  `region_name` varchar(128) NOT NULL,
  PRIMARY KEY (`region_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `userlog_country` (
  `country_code` varchar(4) NOT NULL,
  `country_name` varchar(255) NOT NULL,
  PRIMARY KEY (`country_code`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `userlog_viewer` (
  `viewer` varchar(255) NOT NULL,
  PRIMARY KEY (`viewer`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;



