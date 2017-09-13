아마존을 이용하여 웹서비스를 구동하려고 하면 아파치나 톰캣과 같은 웹서버 구동 프로그램이 필요하다.

### 참고자료
- WAS와 웹서버의 차이
>http://sungbine.github.io/tech/post/2015/02/15/tomcat%EA%B3%BC%20apache%EC%9D%98%20%EC%97%B0%EB%8F%99.html

- 설치과정
>http://m.cafe.daum.net/ssaumjil/LnOm/1831840?svc=kakaotalkTab&bucket=toros_cafe_channel_alpha

-
> http://aidev.co.kr/chatbot/1229

### Apache 설치
1. 다운로드
https://www.apachelounge.com/download/
> 아파치는 C++ Redistributable Visual Studio 2015의 선행 설치를 요구한다.
> 이게 없으면 중간에 VCRUNTIME140.dll 오를 만날 수 있다.

2. 설정파일 수정(httpd.conf)
> [설치경로]/conf/httpd.conf

- ServerRoot 경로 변경 (\를 /로 변경해주기)
- 포트 변경 (굳이 안해도 상관은 없음)
- 웹문서 저장위치 변경
> DocumentRoot "D:/Apache24/htdocs"
> <Directory "D:/Apache24/htdocs">

- ServerName 변경
> ServerName www.example.com:80 찾아서 # 제거하고 ServerName localhost:80 (포트 변경되었다면 변경해줌)

3. 아파치 서비스를 윈도우에 등록
- path 등록
(win+R > control > 시스템 > 고급시스템설정 > 고급 > 환경변수 > 시스템변수 > path >"D:\Apache24\bin" 추가)
- 아파치 서비스 설치
(관리자권한으로 cmd 실행 > 경로를 httpd.exe가 있는 곳으로 이동 > httpd -k install)

### php 설치 및 연동
1. 다운로드
http://windows.php.net/download/
>버전이 너무 많은데 which version do i choose를 확인해본다.
IIS는 Non-Thread Safe(NTS), Apache는 Thread Safe(TS) / 32비트냐 64비트냐 는 아파치와 맞춰줌.

- 역시나 원하는 곳에 풀어주기.

2. php 설정파일 수정(php.ini-production)
- php.ini 로 이름 변경
- ;extension_dir="./" (; 제거) > extension_dir="[설치경로]/ext" (\가 아닌 /로)
> extension_dir="D:/php/ext"

3. 연동을 위해 apach 설정파일 수정(httpd.conf)
- <IfModule dir_module> 을 찾아서 DirectoryIndex에 index.php를 index.html 앞에 추가한다.
- 구문 추가
~~~ ini
PHPIniDir "[php.ini파일 경로]"
LoadModule php7_module "[php설치경로]/php7apache2_4.dll"
AddType application/x-httpd-php .html .php
AddHandler application/x-httpd-php .php
~~~

4. 아파치 재시작 (httpd -k restart)
5. 아파치 웹문서 저장공간에 phpinfo.php 파일 저장

~~~ php
<?php
phpinfo();
 ?>
~~~

6. 웹브라우저에 http://localhost/phpinfo.php 입력해보기

### keyboard API
~~~ php
<?php
echo <<< EOD
{
	"type":"text"
}
EOD;
 ?>
 ~~~

### php확장자 숨기기
> httpd.conf 수정

~~~ conf
<Directory "[소스가 있는 폴더 경로]">
  Options FollowSymLinks MultiViews
  AllowOverride none
  Order allow, deny
  Allow from all
</Directory>
~~~
