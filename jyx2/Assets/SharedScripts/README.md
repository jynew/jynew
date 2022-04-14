金庸群侠传3D重制版SharedScripts说明


这个目录中的内容是跨平台的（Unity/Console），可以被服务端所共享。
所以中间代码不允许依赖于UnityEngine本身，为纯逻辑代码。

调用方式通过客户端与服务端不同的封装，来实现本地的FakeServer或者RPC调用。

所有需要上联机逻辑的服务端代码均应遵守上述原则。