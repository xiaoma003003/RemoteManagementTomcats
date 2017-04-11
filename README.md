
RemoteManagementTomcats

远程管理win机器上的tomcat，是通过c# Socket实现，方便开启、关闭、重启、远程win服务器上的tomcat服务。没有做任何安全措施，仅用于本地测试环境使用。
=======

使用方法
* mysql创建tomcats数据库，导入sql目录中的.sql文件
* socketServer以管理员身份在远程服务端开机启动，
* 数据库serverslist保存服务器信息，主要是IP和端口，tomcatslist保存tomcat服务信息，包括开启、关闭服务路径，以及所在服务器的外键
* deno为客户端文件

## 使用建议
本地测试环境使用，本地测试环境使用，本地测试环境使用
=======

