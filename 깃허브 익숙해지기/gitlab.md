# gitlab 설치하기

### 다운로드
> https://about.gitlab.com/installation/
> https://packages.gitlab.com/gitlab/gitlab-ce

#### 참고자료
> http://html5around.com/wordpress/tutorials/ubuntu-gitlab-install-use-1/

- ee는 유료, ce는 무료

### 설치 과정
- $ sudo apt-get install curl openssh-server ca-certificates postfix
> postfix는 일단 그냥 No Configuration으로 선택합니다.
> sudo dpkg-reconfigure postfix 명령으로 언제든지 설정 가능합니다.

- $ curl -sS https://packages.gitlab.com/install/repositories/gitlab/gitlab-ce/script.deb.sh | sudo bash
> Running apt-get update... 메세지 ...약간 시간 걸림
>  메세지 The repository is setup! You can now install packages. 가 보임

- sudo apt-get install gitlab

- $ sudo apt-get install gitlab-ce
> 화면에 노란 별로 그린 마크와 빨간색으로 쓴 GitLab 글자 밑에
> 메세지로 sudo gitlab-ctl reconfigure 를 실행하라고 알려줌

- $ sudo gitlab-ctl reconfigure
> 화면에 글자들이 올라가면서 잠시 시간 걸림
> 설정파일 수정 후에는 항상 이 명령을 해주어야 함

- sudo vi /etc/gitlab/gitlab.rb
- external-url 'http://localhost:8080'

- sudo gitlab-ctl restart
whoops, gitlab is taking too much time to respond
- sudo vi /etc/gitlab/gitlab.rb
- /unicorn 검색해서 주석 두줄 해제하고 포트를 8081로 변경

### 개발PC에서 설치
> 패키지 다운받아서 직접 설치하기

- sudo apt-get install libnss3-1d libxss1
- apt-get install ruby //ruby1.9 버전 이상이 gitlab 요구사항
- sudo dpkg -i 패키지.deb
- sudo vi /etc/gitlab/gitlab.rb

### 만난 오류들

1.
W: bzip2:/var/lib/apt/lists/partial/kr.archive.ubuntu.com_ubuntu_dists_precise_main_source_Sources 파일을 받는데 실패했습니다  해시 합이 맞지 않습니다

$ rm -rf /var/lib/apt/lists/*
$ apt-get update

2.
samba (2:3.6.3-2ubuntu2.10) 설정하는 중입니다 ...
  /var/lib/dpkg/info/samba.postinst: 95: /var/lib/dpkg/info/samba.postinst: update-inetd: not found
  dpkg: samba을(를) 처리하는데 오류가 발생했습니다 (--configure):
  설치한 post-installation 스크립트 하위 프로세스가 오류 127번을 리턴했습니다
  처리하는데 오류가 발생했습니다:
  samba

root@roy-VPCCB17FK:test# rm -f /var/lib/dpkg/info/samba.*
root@roy-VPCCB17FK:test# apt-get upgrade

3.
젬이 없다면
$ sudo gem install bundler --no-ri --no-rdoc
