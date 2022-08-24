# 2DMutiPlayerGame
简单的状态同步，目前服务器只是简单的转发包和判断客户端是否死亡，其余逻辑均由各客户端自行判断  
服务器：.net6.0  
客户端：unity  
通信协议：protobuf+UDP+KCP   
数据库：mysql+EFCore  
[效果演示](https://www.bilibili.com/video/BV16Z4y1i7q7)  

----

2022.8.24

把注册账号的逻辑换成GRPC调用（解决回调地狱的问题，但是因为一开始并没有考虑到这些问题所以有些逻辑不好改，于是只改了注册逻辑试用一下效果）（不过GRPC很快就不维护C#了，但是unity目前不支持HTTP2.0所以只能用GRPC For C#，之后unity改.net core的时候就能用上GRPC-Dotnet了）