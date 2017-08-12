# gitlab 설치하기
> http://html5around.com/wordpress/tutorials/ubuntu-gitlab-install-use-1/

### 설치 과정
- $ sudo apt-get install curl openssh-server ca-certificates postfix
> postfix는 일단 그냥 No Configuration으로 선택합니다.
## sudo dpkg-reconfigure postfix 명령으로 언제든지 설정 가능합니다.  

- $ curl -sS https://packages.gitlab.com/install/repositories/gitlab/gitlab-ce/script.deb.sh | sudo bash
## Running apt-get update... 메세지 ...약간 시간 걸림
## 메세지 The repository is setup! You can now install packages. 가 보임
$ sudo apt-get install gitlab-ce
## 화면에 노란 별로 그린 마크와 빨간색으로 쓴 GitLab 글자 밑에
## 메세지로 sudo gitlab-ctl reconfigure 를 실행하라고 알려줌
$ sudo gitlab-ctl reconfigure
## 화면에 글자들이 올라가면서 잠시 시간 걸림
## 설정파일 수정 후에는 항상 이 명령을 해주어야 함
$ gitlab-ctl -help
## gitlab-ctl 명령뒤에 사용할 수 있는 옵션(start..stop..upgrade 등등) 확인
$ sudo gitlab-ctl upgrade
## 그냥 한번 해 보았습니다. 이상하게 동작하면 restart하라는 메세지가 보입니다.


- sudo vi /etc/gitlab/gitlab.rb
- external-url 'http://localhost:8080'
