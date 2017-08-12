# gitlab 설치하기
> http://html5around.com/wordpress/tutorials/ubuntu-gitlab-install-use-1/

### 설치 과정
- $ sudo apt-get install curl openssh-server ca-certificates postfix
> postfix는 일단 그냥 No Configuration으로 선택합니다.
> sudo dpkg-reconfigure postfix 명령으로 언제든지 설정 가능합니다.

- $ curl -sS https://packages.gitlab.com/install/repositories/gitlab/gitlab-ce/script.deb.sh | sudo bash
> Running apt-get update... 메세지 ...약간 시간 걸림
>  메세지 The repository is setup! You can now install packages. 가 보임

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
