# 카카오톡 플러스 친구 봇 만들기

> 플러스친구(구 옐로아이디) 기능을 사용해본 후 관심이 생겼다.
회사 식당 메뉴를 늘 인프라에 접속해서 확인해야 한다는 불편함을 해소하기 위해 플러스 친구를 통해 식단을 보내주는 봇을 만들어 보기로 했다.


## 플러스 친구 가입
- https://center-pf.kakao.com  
- 하나의 관리자 아이디로 여러개의 플러스 친구 생성 가능
- 플러스 친구 이름은 카톡에 대표 노출되고 1회 수정이 가능하므로 알아서 정하기

## 스마트 채팅
- API형 선택
- 서버URL에 API테스트를 통해 SUCCESS 되어야 앱을 등록할 수 있게 됨

## 봇에 응답할 서버 구성하기
> API테스트를 할 간단한 서버 구성


### NodeJS를 이용
~~~javascript
var http = require('http');
http.createServer(function(req, res) {
  if(req.url.substring(1)=="keyboard"){
    var resObj = {
      "type": "text"
    };
    res.setHeader('content-Type':'application/json');
    res.end(JSON.stringify(resObj));
  } else {
    res.setHeader('content-Type':'application/json');
    res.end("");
  })
}).listen(8000);
~~~

### WCF Service를 이용
- System.ServiceModel.AddressAccessDeniedException 예외 발생 > 관리자계정으로 실행

##### 1. WCF Service Contract 프로젝트 생성
> 이하 ServiceInterface

-  "C# 클래스 라이브러리" 타입으로 프로젝트 추가
- 서버 메소드들의 Interface를 정의함

###### Iservice.cs
~~~ cs
[ServiceContract]
public Interface IService{
  [OperationContract]
  [WebInvoke(Method="GET", ResponseFormat = WebMessageFormat.Json)]
  KeyboardRes keyboard();
}
[DataContract]
public class KeyboardRes{
  [DataMember]
  public string type{ get;set;}
}
~~~

##### 2. 콘솔서버어플리케이션용 WCF Service 생성
> 이하 ConsoleServer

- "C# 콘솔 어플리케이션" 타입으로 프로젝트 추가
- Iservice에 정의된 메소드들을 구현

###### Service.cs
~~~ cs
public class Service : IService{
  public KeyboardRes keyboard(){
    return _keyboard();
  }

  KeyboardRes _keyboard(){
    KeyboardRes res = new KeyboardRes();
    res.type = "text";
    return res;
  }
}
~~~

###### Program.cs
~~~ cs
class Program {
  static void Main(string[] args){
    WebServiceHost host = new WebServiceHost(typeof(Service), new Uri("http://localhost"));
    host.AddServiceEndpoint(typeof(IService), new WebHttpBinding(),"");

    try{
      host.open();
      Console.WriteLine("Hi! Press <ENTER> to terminate");
      Console.ReadLine();
      host.close();
    }
    catch(CommunicationException e){
      Console.WriteLine("Excetpion: {0}", e.Message);
      host.Abort();
    }
  }
}
~~~

##### 3. IIS용 WCF Service 생성
> 이하 IisServer

###### Service.svc.cs
- service.cs 와 동일하게 구현

###### Web.Config
~~~ Config
<?xml version ="1.0"?>
<configuration>
  <system.serviceModel>
    <services>
      <service name="IisServer.Service">
        <endpoint address="Http"
                  binding="webHttpBinding"
                  contract="ServiceInterface.IService"
                  behaviorConfiguration="webHttp"/>
      </service>
    </services>
    <behaviors>
      <endpointBehaviors>
        <behavior name="webHttp">
          <webHttp/>
        </behavior>
      </endpointBehaviors>
    </behaviors>
  </system.serviceModel>
</configuration>
~~~
