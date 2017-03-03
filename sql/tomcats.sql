
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `serverslist`
-- ----------------------------
DROP TABLE IF EXISTS `serverslist`;
CREATE TABLE `serverslist` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号',
  `ipaddress` varchar(50) NOT NULL,
  `port` varchar(6) NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `operatingsystem` varchar(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of serverslist
-- ----------------------------
INSERT INTO `serverslist` VALUES ('1', '192.168.0.8', '20000', '测试机', 'win7');

-- ----------------------------
-- Table structure for `tomcatlist`
-- ----------------------------
DROP TABLE IF EXISTS `tomcatlist`;
CREATE TABLE `tomcatlist` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `lujing` varchar(100) NOT NULL,
  `tomcatname` varchar(100) NOT NULL,
  `lujingstop` varchar(100) NOT NULL,
  `serverid` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tomcatlist
-- ----------------------------
INSERT INTO `tomcatlist` VALUES ('1', 'D:\\Apache-Solr-Tomcat-7.62\\bin\\startup.bat', 'Solr', 'D:\\Apache-Solr-Tomcat-7.62\\bin\\shutdown.bat', '1');

