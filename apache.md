아마존을 이용하여 웹서비스를 구동하려고 하면 아파치나 톰캣과 같은 웹서버 구동 프로그램이 필요하다.

### 참고자료
- WAS와 웹서버의 차이
>http://sungbine.github.io/tech/post/2015/02/15/tomcat%EA%B3%BC%20apache%EC%9D%98%20%EC%97%B0%EB%8F%99.html

- 설치과정
>http://m.cafe.daum.net/ssaumjil/LnOm/1831840?svc=kakaotalkTab&bucket=toros_cafe_channel_alpha

### 설치
1. 다운로드
https://www.apachelounge.com/download/
> 아파치는 C++ Redistributable Visual Studio 2015의 선행 설치를 요구한다.
이게 없으면 중간에 VCRUNTIME140.dll 오류를 만날 수 있다.

2. zip을 풀고 그 중에서 "Apache24"라는 폴더를 원하는 곳에 풀어준다. 그곳이 아파치가 설치되는 곳.

3. 설정파일 수정(httpd.conf)
> [설치경로]/conf/httpd.conf

- ServerRoot 경로 변경 (\ 를 /로 변경해주기)
- 포트 변경 (굳이 안해도 상관은 없음)
- 웹문서 저장위치 변경
> DocumentRoot "D:/Apache24/htdocs"
> <Directory "D:/Apache24/htdocs">

- ServerName 변경
> ServerName www.example.com:80 찾아서 # 제거하고 ServerName localhost:80 (포트 변경되었다면 변경해줌)

4. 아파치 서비스를 윈도우에 등록
- path 등록
(win+R > control > 시스템 > 고급시스템설정 > 고급 > 환경변수 > 시스템변수 > path >"D:\Apache24\bin" 추가)
- 아파치 서비스 설치
(관리자권한으로 cmd 실행 > httpd -k install)
