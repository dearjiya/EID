
### Amazon Cognito 인증 서비스 이용하기

#### 환경 셋팅
~~~ config
$ apt-get install nodejs
$ git clone https://github.com/aws/amazon-cognito-identity-js/
$ npm install --save-dev webpack json-loader
$ npm install --save amazon-cognito-identity-js
$ npm install aws-sdk
~~~

#### add
~~~ config
$ vi package.json
~~~
~~~ json
{ "private":true }
~~~

#### config.js 수정 > 풀아이디, 클라이언트아이디 입력
~~~ config
$ vi config.js
~~~

#### entry 수정
~~~ config
$ vi webpack.config.babel.js
~~~
~~~js
module.exports = {
  // Example setup for your project:
  // The entry module that requires or imports the rest of your project.
  // Must start with `./`!
  entry: './src/join.js',
  // Place output files in `./dist/my-app.js`
  output: {
    path: 'dist',
    filename: 'my-app.js'
  },
  module: {
    loaders: [
      {
        test: /\.json$/,
        loader: 'json'
      }
    ]
  }
};
~~~

#### join.js 생성

~~~ config
$ vi join.js
~~~
~~~ js
var AmazonCognitoIdentity = require('amazon-cognito-identity-js');
var CognitoUserPool = AmazonCognitoIdentity.CognitoUserPool;
var CognitoUserAttribute = AmazonCognitoIdentity.CogitoUserAttribute;

var poolData = {
       UserPoolId : 'ap-northeast-2_HpblGmj44', // Your user pool id here
       ClientId : '4tof5md7tpm09h9t2g4idn2vsi' // Your client id here
   };

var userPool = new CognitoUserPool(poolData);
var attributeList = [];
var dataEmail = {
        Name : 'email',
        Value : 'dearjiya@nate.com'
    };

var dataGender = {
   Name : 'gender',
        Value : 'female'
    };

var attributeEmail = new CognitoUserAttribute(dataEmail);

var attributeGender = new CognitoUserAttribute(dataGender);

attributeList.push(attributeEmail);
attributeList.push(attributeGender);

userPool.signUp('username', 'Username1', attributeList, null, function(err, result){
   if (err) {
          //alert(err);
      console.log(err.message);
               return;
        }
        cognitoUser = result.user;
        console.log('user name is ' + cognitoUser.getUsername());
});

~~~

* export 에러
~~~config
$ npm install
~~~
