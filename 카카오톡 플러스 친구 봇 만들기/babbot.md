# 카카오톡 플러스 친구 봇 만들기

> 플러스친구(구 옐로아이디) 기능을 사용해본 후 관심이 생겼다.
회사 식당 메뉴를 늘 인프라에 접속해서 확인해야 한다는 불편함을 해소하기 위해  
플러스 친구를 통해 식단을 보내주는 봇을 만들어 보기로 했다.


### 플러스 친구 가입
- https://center-pf.kakao.com  
- 하나의 관리자 아이디로 여러개의 플러스 친구 생성 가능
- 플러스 친구 이름은 카톡에 대표 노출되고 1회 수정이 가능하므로 알아서 정하기

### API형
~~~javascript
var http = require('http');
var server = http.createServer(function(req, res){
  res.writeHead(200, {'Content-Type' : 'text/plain'});
  res.end('Hello World');
});
server.listen(9000);
~~~
