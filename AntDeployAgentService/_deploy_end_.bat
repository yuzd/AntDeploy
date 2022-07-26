@ECHO OFF
sc stop AntDeployAgentWindowsService ::这句的意思是停止Agent服务
xcopy /e $DeployFolder$ $AppFolder$ /y ::这句里面有占位符，意思是复制文件
sc start AntDeployAgentWindowsService ::这句的意思是启动Agent服务