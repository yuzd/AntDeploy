# AppendFileVersion
VSIX 统一修改 html or cshtml 中的 css标签和script标签添加统一版本号 来防止浏览器的缓存

# download
https://marketplace.visualstudio.com/items?itemName=nainaigu.AppendSrcVersion

# why
我经常会遇到下面的问题：
在页面引用了js 或者 css
比如：

<1ink href=“~/css/login.css” rel=”stylesheet”/>
<script src="~/js/login.js"></script>

然后样式需要修改 或者 js发生了改变
然后我发布到了生产，但是浏览器有缓存。又不希望麻烦使用者清缓存。所以得重新会进行如下修改：

<link href="~/css/login.css?2018112011" rel="stylesheet"/>
<script src="~/js/login.js?2018112011"></script>

这样在重新发布到生产就不会有浏览器缓存问题了。。



一般专业前端开发可以用一些打包工具可以实现同样的功能
或者最新的asp.net core 的razor 可以设置 asp-append-version="true" 来解决。。
但是加了这个属性后 浏览器会每次请求拉取最新的。
我觉得没有必要,只有在我修改了才需要拉取最新的。
所以我写了这个vs插件来帮助简单操作就能批量的给script标签或者css标签的文件后 append version！

# 给某个html里面进行appen version操作
![image](https://github.com/yuzd/AppendFileVersion/blob/master/b.png)

# 给某个目录里面的所有cshtml进行appen version操作
![image](https://github.com/yuzd/AppendFileVersion/blob/master/c.png)

# append version前后对比
![image](https://github.com/yuzd/AppendFileVersion/blob/master/a.png)
