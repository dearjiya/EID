# ELK (Elastic Search + Logstash + Kibana) 로그 분석 환경 구성

#### 설치환경 : window7

### 1.다운로드 zip 풀어서 만들어놓기
### 2. ElasticSearch
    > 자바는 필수로 깔려있어야함 **
    JAVA_HOME 과 path에 환경변수 설정
    (간혹, 자바경로를 못 찾는 경우가 있는데)
    (1. JAVA_HOME의 환경변수에 /bin이 들어가있는지 확인 > 없어야 함.
       bat파일 자체에 자바경로를 JAVA_HOME\bin으로 잡아두었기 때문에 bin이 중복되서는 안됨)
    (2. 그래도 안되면 자바가 기본으로 깔리는 경로 중 Program" "Files의 저 사이 때문에 문제가 생겼을 가능성도 있음
     빈공간이 없는 경로에 자바를 설치. 나는 어차피 서버라 C:\java\jre~\bin에 설치해버림


    정상적 설치라면 http://localhost:9200 에 들어가면 JSON정보가 뜸.
    \elasticsearch\config\elasticsearch.yml
    cluster.name
    node.name
    network.host (localhost를 사용하지 않기 위해 직접 IP를 넣음)
    http.port
    수정
    서비스로 구동
    bin폴더에서
     elasticsearch-service.bat install

### 3. Kibana
   http://localhost:5601

   \kibana\config\kibana.yml

   server.host

   server.name

   elasticsearch.url

   수정

### 4. Logstash  
    간단히 config 수정
