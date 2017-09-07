#### putty 로 접속
> 22포트 오픈

#### tmux 실행
- 설정파일 수정
> 우분투에는 sudo apt-get install tmux로 설치는 가능하지만 오래된 버전임.

##### 참고자료 https://code4rain.wordpress.com/2015/03/01/tmux-%EC%82%AC%EC%9A%A9%EB%B0%A9%EB%B2%95-%EC%B0%BD%EB%B6%84%ED%95%A0%ED%95%98%EA%B8%B0-%EB%B9%8C%EB%93%9C%EA%B1%B8%EA%B3%A0-%ED%87%B4%EA%B7%BC%ED%95%98%EA%B8%B0/

bindkey(C-j) + : 를 실행시키면 노란색 라인이 뜨는데 그곳에서
- source /.tmux.conf 를 실행시켜서 설정파일이 실행되도록 만들어주기

#### 리눅스를 원격으로 접속하고 싶다!
Putty + x11 + Xming

##### 참고자료
http://ra2kstar.tistory.com/93
