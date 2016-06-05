(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,Concurrency,Remoting,AjaxRemotingProvider,alert,UI,Next,Doc,List,T,London,Web,Client;
 Runtime.Define(Global,{
  London:{
   Web:{
    Client:{
     page:Runtime.Field(function()
     {
      var arg20;
      arg20=function()
      {
       return Concurrency.Start(Concurrency.Delay(function()
       {
        return Concurrency.Bind(AjaxRemotingProvider.Async("London.Web:0",[]),function(_arg1)
        {
         alert(_arg1);
         return Concurrency.Return(null);
        });
       }),{
        $:0
       });
      };
      return Doc.Button("Say hello",Runtime.New(T,{
       $:0
      }),arg20);
     })
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  alert=Runtime.Safe(Global.alert);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
  List=Runtime.Safe(Global.WebSharper.List);
  T=Runtime.Safe(List.T);
  London=Runtime.Safe(Global.London);
  Web=Runtime.Safe(London.Web);
  return Client=Runtime.Safe(Web.Client);
 });
 Runtime.OnLoad(function()
 {
  Client.page();
  return;
 });
}());
