### C# 4.0 필수 매개변수와 선택적 매개변수
> 4.0에 새로 추가된 기능이라고 한다. 기존에는 매개변수가 1개 일 경우, 2개일 경우 어떤 매개변수를 이용하느냐에 따라 오버로딩 하면서 사용했다.

~~~ cs
class Program
{
  class Account
  {
    public string Name {get; set;}
    public string PhoneNum {get; set;}
    public string Address {get; set;}
    public Account(string name, string phoneNum, string address)
    {
      this.Name = Name;
      this.PhoneNum = PhoneNum;
      this.Address = address;
    }
  }

  class AccountBook
  {
    public List<Account> Accounts {get; set;}
    public AccountBook()
    {
      Accounts = new List<Account>();
    }
    public void Create(string name, string phonenum, string address)
    {
      Account account = new Account(name, phonenum, address);
      this.Accounts.Add(account);
    }
    public void Create(string name, string phonenum)
    {
      this.Create(name, phonenum, "None");
    }
    public void Create(string name)
    {
      this.Create(name, "None", "None");
      Console.WriteLine("The information is short");
    }
    public void Display()
     {
       Console.WriteLine("This is AccountBook");
       Console.WriteLine("-----------------------------------------------");
       foreach (var res in Accounts)
        {
          Console.WriteLine("Name: {0} | PhoneNum: {1} | Address: {2}", res.Name, res.PhoneNum, res.Address);
          Console.WriteLine("-----------------------------------------------");
         }
      }
  }

  static void Main(string[] args)
  {
    AccountBook book = new AccountBook();
    book.Create("YHJ","1004","dearjiya");
    book.Create("JHR","0214");
    book.Create("YHW");
    book.Display();
    Console.WriteLine("Perfect!");
    Console.ReadLine();
  }
}
~~~

- 이러한 방법으로 선언해서 필요한 것을 불러 사용해야 했지만, 4.0에서 새로 추가된 이 기능을 사용한다면 불필요한 오버로딩 코드는 필요가 없다.

~~~ cs
public void Create(string name, string phonenum="", string address="")
{
  Account account = new Account(name, phonenum, address);
  this.Accounts.Add(account);
}
~~~

- 오버로딩 된 부분을 지우고 하나의 함수를 선언한 후 필수로 하고 싶은 매개변수는 그대로 두고, 선택하고 싶은 매개변수는 어떤 값을 지정해서 선언하면 된다. 다만 무조건 필수 매개변수를 가져야 하는 것은 아니지만, 필수 매개변수와 선택 매개변수를 모두 선언하고 싶다면 무조건 필수 뒤에 선택을 선언해주어야 한다.

- 선택 매개변수를 순서대로 지정할 필요는 없다.
> 이름과 주소만 지정하여 넘기고 싶을 경우 or 순서를 바꾸어 넘기고 싶을 경우

~~~ cs
public void Create(string name, string phonenum="", string address="")
{
  Account account = new Account(name, phonenum, address);
  this.Accounts.Add(account);
}
static void Main(string[] args)
{
  AccountBook book = new AccountBook();
  book.Create("YHW", address:"Busan");
  book.Create("YHJ",address:"1004", phonenum: "dearjiya");
  book.Display();
  Console.WriteLine("Perfect!");
  Console.ReadLine();
}
~~~

#### 참고자료
> http://ehdrn.tistory.com/235
