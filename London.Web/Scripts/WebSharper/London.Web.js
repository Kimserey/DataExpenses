(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,London,Web,ExpensesPerMonth,Remoting,AjaxRemotingProvider,UI,Next,View1,View,Seq,Collections,MapModule,Date,Doc,List,Arrays;
 Runtime.Define(Global,{
  London:{
   Web:{
    Client:{
     page:function()
     {
      return ExpensesPerMonth.page();
     }
    },
    ExpensesPerMonth:{
     page:function()
     {
      var arg00,arg10,x,_arg00_1;
      arg00=function()
      {
       return AjaxRemotingProvider.Async("London.Web:0",[]);
      };
      arg10=View1.Const(null);
      x=View.MapAsync(arg00,arg10);
      _arg00_1=function(expenses)
      {
       var x1,mapping,x2;
       x1=Seq.toList(MapModule.ToSeq(expenses));
       mapping=function(tupledArg)
       {
        var _arg1,v,y,name,m,title,mapping1,body,attrs1,attrs2,attrs3;
        _arg1=tupledArg[0];
        v=tupledArg[1];
        y=_arg1[1].$0;
        name=_arg1[0].$0[0];
        m=_arg1[0].$0[1];
        title=name+" "+Global.String(y);
        mapping1=function(expense)
        {
         var date,label,amount,category,attrs;
         date=(new Date(expense.Date)).toLocaleDateString();
         label=expense.Label;
         amount=Global.String(expense.Amount);
         category=expense.Category;
         attrs=[];
         return Doc.Concat([Doc.Element("tr",attrs,[Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode(date)]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode(label)]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode(amount)]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode(category)]),Doc.TextNode("\n        ")])]);
        };
        body=List.map(mapping1,v);
        attrs1=[];
        attrs2=[];
        attrs3=[];
        return Doc.Concat([Doc.Element("div",[],[Doc.TextNode(title)]),Doc.TextNode("\n"),Doc.Element("table",attrs1,[Doc.TextNode("\n    "),Doc.Element("thead",attrs2,[Doc.TextNode("\n        "),Doc.Element("tr",attrs3,[Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode("Date")]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode("Title")]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode("Amount")]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode("Label")]),Doc.TextNode("\n            "),Doc.Element("td",[],[Doc.TextNode("Category")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")]),Doc.TextNode("\n    "),Doc.Element("tbody",[],Arrays.ofSeq(body)),Doc.TextNode("\n")]),Doc.TextNode("\n")]);
       };
       x2=List.map(mapping,x1);
       return Doc.Concat(x2);
      };
      return Doc.BindView(_arg00_1,x);
     }
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  London=Runtime.Safe(Global.London);
  Web=Runtime.Safe(London.Web);
  ExpensesPerMonth=Runtime.Safe(Web.ExpensesPerMonth);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  View1=Runtime.Safe(Next.View1);
  View=Runtime.Safe(Next.View);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  Date=Runtime.Safe(Global.Date);
  Doc=Runtime.Safe(Next.Doc);
  List=Runtime.Safe(Global.WebSharper.List);
  return Arrays=Runtime.Safe(Global.WebSharper.Arrays);
 });
 Runtime.OnLoad(function()
 {
  return;
 });
}());
