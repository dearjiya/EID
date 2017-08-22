# 카카오톡 플러스 친구 봇 만들기

> 플러스친구(구 옐로아이디) 기능을 사용해본 후 관심이 생겼다.
회사 식당 메뉴를 늘 인프라에 접속해서 확인해야 한다는 불편함을 해소하기 위해  
플러스 친구를 통해 식단을 보내주는 봇을 만들어 보기로 했다.


### 플러스 친구 가입
- https://center-pf.kakao.com  
- 하나의 관리자 아이디로 여러개의 플러스 친구 생성 가능
- 플러스 친구 이름은 카톡에 대표 노출되고 1회 수정이 가능하므로 알아서 정하기

### 스마트 채팅
- API형 선택
- 서버URL에 API테스트를 통해 SUCCESS 되어야 앱을 등록할 수 있게 됨

### 봇에 응답할 서버 구성하기
> API테스트를 할 간단한 서버 구성

#### NodeJS를 이용
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

#### wcf rest API
- System.ServiceModel.AddressAccessDeniedException 예외 발생 > 관리자계정으로실행

##### 1. WCF Service Contract 프로젝트 생성
-  "C# 클래스 라이브러리" 타입으로 프로젝트 추가
- 서버 메소드들의 Interface를 정의함

###### Iservice.cs
~~~ cs
[ServiceContract]
public Interface IService{
  [OperationContract]
  [WebInvoke(Method="GET",ResponseFormat = WebMessageFormat.Json)]
  KeyboardRes keyboard();
}
[DataContract]
public class KeyboardRes{
  [DataMember]
  public string type{ get;set;}
}
~~~

2.
#####
