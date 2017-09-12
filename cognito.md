<<<<<<< HEAD

=======
>>>>>>> c62a59ce5b4d215785f550098475f8d72b9779f4
$ apt-get install nodejs

$ git clone https://github.com/aws/amazon-cognito-identity-js/

config.js 수정 > 풀아이디, 클라이언트아이디 입력

vi package.json
{ "private":true} add

> npm install --save-dev webpack json-loader
> npm install --save amazon-cognito-identity-js

vi webpack.config.babel.js
module.exports = {
  // Example setup for your project:
  // The entry module that requires or imports the rest of your project.
  // Must start with `./`!
  entry: './src/entry',
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

추가하고 entry 부분 수정하기

vi package.json
{
  "scripts": {
    "build": "webpack"
  }
} 추가

vi test.js

  1 var AmazonCognitoIdentity = require('amazon-cognito-identity-js');              
  2 var CognitoUserPool = AmazonCognitoIdentity.CognitoUserPool;                    
  3 var CognitoUserAttribute = AmazonCognitoIdentity.CognitoUserAttribute;          
  4                                                                                 
  5 var poolData = {                                                                
  6         UserPoolId : 'ap-northe', // Your user pool id here      
  7         ClientId : '4tof5n2vsi' // Your client id here          
  8     };                                                                          
  9     var userPool = new CognitoUserPool(poolData);                               
 10                                                                                 
 11     var attributeList = [];                                                     
 12                                                                                 
 13     var dataEmail = {                                                           
 14         Name : 'email',                                                         
 15         Value : '.com'                                            
 16     };                                                                          
 17                                                                                 
 18     var dataGender = {                                                          
 19         Name : 'gender',                                                        
 20         Value : 'female'                                                        
 21     };                                                                          
 22     var attributeEmail = new CognitoUserAttribute(dataEmail);                   
 23     var attributeGender = new CognitoUserAttribute(dataGender);                 
 24                                                                                 
 25     attributeList.push(attributeEmail);                                         
 26     attributeList.push(attributeGender);                                        
 27                                                                                 
 28     userPool.signUp('hoonhoon', 'Hoonhoon1', attributeList, null, function(err, result){
 29         if (err) {                                                              
 30             alert(err);                                                         
 31             return;                                                             
 32         }                                                                       
 33         cognitoUser = result.user;                                              
 34         console.log('user name is ' + cognitoUser.getUsername());               
 35     });                                                                         
<<<<<<< HEAD
=======


$ npm install aws-sdk

$ export 모른다 > npm install
>>>>>>> c62a59ce5b4d215785f550098475f8d72b9779f4
