먼저 nodejs를 이용해 간단히 요청에 응답하는 아~주 단순한 서버를 구성해볼 예정이다.
nodejs에 대해 간단히 설명하자면 구글 크롬의 자바스크립트 엔진에 기반해 만들어진 서버 사이드 플랫폼이다.

먼저 HTTP서버를 구성하는 방법은 간단하다.
모듈 import & 서버 생성 이다.

1. 필요한 모듈을 불러오기 위해 require를 사용한다.
var http = require("http");

2. 서버 생성 메소드를 실행한다.
~~~javascript
http.createServer(function(request, response){
  //HTTP헤더 전송
  //HTTP Status 200: OK
  //Content-type: text/plain
  response.writeHead(200, {'Content-type':'text/plain'});
  //ResponseBody를 Hi로 설정함
  response.end("Hi\n");
}).listen(8010);
~~~
3. 서버 실행
 nodejs Bob.js
