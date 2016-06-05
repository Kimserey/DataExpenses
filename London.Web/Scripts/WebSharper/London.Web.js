(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,Concurrency,Remoting,AjaxRemotingProvider,alert,UI,Next,Doc,List,T;
 Runtime.Define(Global,{
  London:{
   Web:{
    Client:{
     page:function()
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
      return Doc.Button("Say hello ",Runtime.New(T,{
       $:0
      }),arg20);
     }
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
  return T=Runtime.Safe(List.T);
 });
 Runtime.OnLoad(function()
 {
  return;
 });
}());
