(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,UI,Next,Doc,List,AttrProxy,Arrays,London,Web,App;
 Runtime.Define(Global,{
  London:{
   Web:{
    App:{
     main:Runtime.Field(function()
     {
      var Items,Content,Body,attrs,attrs1,attrs2,attrs3,attrs4,attrs5,attrs6,attrs7,attrs8,Content1,Body1,attrs9,attrsa,attrsb,attrsc,attrsd,attrse,attrsf,attrs10,attrs11,Content2,Body2,attrs12,attrs13,attrs14,attrs15,attrs16,attrs17,attrs18,attrs19,attrs1a,Items1,Content3,Body3,attrs1b,attrs1c,attrs1d,attrs1e,attrs1f,attrs20,attrs21,attrs22,attrs23,Items2,Content4,Body4,attrs24,attrs25,attrs26,attrs27,attrs28,attrs29,attrs2a,attrs2b,attrs2c;
      attrs=[];
      attrs1=[];
      attrs2=[];
      attrs3=[];
      Body=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs1,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs2,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs3,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")])]);
      attrs4=[AttrProxy.Create("class","table-card")];
      attrs5=[];
      attrs6=[];
      attrs7=[];
      Content=List.ofArray([Doc.Concat([Doc.Element("div",attrs4,[Doc.TextNode("\n    "),Doc.Element("table",attrs5,[Doc.TextNode("\n        "),Doc.Element("thead",attrs6,[Doc.TextNode("\n            "),Doc.Element("tr",attrs7,[Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Date")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Label")]),Doc.TextNode("\n                "),Doc.Element("th",[AttrProxy.Create("class","right")],[Doc.TextNode("Amount")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Category")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n        "),Doc.Element("tbody",[],Arrays.ofSeq(Body)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")])])]);
      attrs8=[AttrProxy.Create("class","card-list-item"),AttrProxy.Create("data-content","content-31")];
      attrs9=[];
      attrsa=[];
      attrsb=[];
      attrsc=[];
      Body1=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs9,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrsa,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrsb,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrsc,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")])]);
      attrsd=[AttrProxy.Create("class","table-card")];
      attrse=[];
      attrsf=[];
      attrs10=[];
      Content1=List.ofArray([Doc.Concat([Doc.Element("div",attrsd,[Doc.TextNode("\n    "),Doc.Element("table",attrse,[Doc.TextNode("\n        "),Doc.Element("thead",attrsf,[Doc.TextNode("\n            "),Doc.Element("tr",attrs10,[Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Date")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Label")]),Doc.TextNode("\n                "),Doc.Element("th",[AttrProxy.Create("class","right")],[Doc.TextNode("Amount")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Category")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n        "),Doc.Element("tbody",[],Arrays.ofSeq(Body1)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")])])]);
      attrs11=[AttrProxy.Create("class","card-list-item"),AttrProxy.Create("data-content","content-32")];
      attrs12=[];
      attrs13=[];
      attrs14=[];
      attrs15=[];
      Body2=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs12,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs13,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs14,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs15,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")])]);
      attrs16=[AttrProxy.Create("class","table-card")];
      attrs17=[];
      attrs18=[];
      attrs19=[];
      Content2=List.ofArray([Doc.Concat([Doc.Element("div",attrs16,[Doc.TextNode("\n    "),Doc.Element("table",attrs17,[Doc.TextNode("\n        "),Doc.Element("thead",attrs18,[Doc.TextNode("\n            "),Doc.Element("tr",attrs19,[Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Date")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Label")]),Doc.TextNode("\n                "),Doc.Element("th",[AttrProxy.Create("class","right")],[Doc.TextNode("Amount")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Category")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n        "),Doc.Element("tbody",[],Arrays.ofSeq(Body2)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")])])]);
      attrs1a=[AttrProxy.Create("class","card-list-item"),AttrProxy.Create("data-content","content-33")];
      Items=List.ofArray([Doc.Concat([Doc.TextNode("\n        "),Doc.Element("div",attrs8,[Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header")],[Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-title")],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-subtitle")],[Doc.Element("div",[AttrProxy.Create("class","amount")],[Doc.TextNode("20.00")])]),Doc.TextNode("\n            ")]),Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-content"),AttrProxy.Create("id","content-31")],Arrays.ofSeq(Content)),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")]),Doc.Concat([Doc.TextNode("\n        "),Doc.Element("div",attrs11,[Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header")],[Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-title")],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-subtitle")],[Doc.Element("div",[AttrProxy.Create("class","amount")],[Doc.TextNode("20.00")])]),Doc.TextNode("\n            ")]),Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-content"),AttrProxy.Create("id","content-32")],Arrays.ofSeq(Content1)),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")]),Doc.Concat([Doc.TextNode("\n        "),Doc.Element("div",attrs1a,[Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header")],[Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-title")],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-subtitle")],[Doc.Element("div",[AttrProxy.Create("class","amount")],[Doc.TextNode("20.00")])]),Doc.TextNode("\n            ")]),Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-content"),AttrProxy.Create("id","content-33")],Arrays.ofSeq(Content2)),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")])]);
      attrs1b=[];
      attrs1c=[];
      attrs1d=[];
      attrs1e=[];
      Body3=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs1b,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs1c,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs1d,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs1e,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")])]);
      attrs1f=[AttrProxy.Create("class","table-card")];
      attrs20=[];
      attrs21=[];
      attrs22=[];
      Content3=List.ofArray([Doc.Concat([Doc.Element("div",attrs1f,[Doc.TextNode("\n    "),Doc.Element("table",attrs20,[Doc.TextNode("\n        "),Doc.Element("thead",attrs21,[Doc.TextNode("\n            "),Doc.Element("tr",attrs22,[Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Date")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Label")]),Doc.TextNode("\n                "),Doc.Element("th",[AttrProxy.Create("class","right")],[Doc.TextNode("Amount")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Category")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n        "),Doc.Element("tbody",[],Arrays.ofSeq(Body3)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")])])]);
      attrs23=[AttrProxy.Create("class","card-list-item"),AttrProxy.Create("data-content","content-2")];
      Items1=List.ofArray([Doc.Concat([Doc.TextNode("\n        "),Doc.Element("div",attrs23,[Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header")],[Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-title")],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-subtitle")],[Doc.Element("div",[AttrProxy.Create("class","amount")],[Doc.TextNode("20.00")])]),Doc.TextNode("\n            ")]),Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-content"),AttrProxy.Create("id","content-2")],Arrays.ofSeq(Content3)),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")])]);
      attrs24=[];
      attrs25=[];
      attrs26=[];
      attrs27=[];
      Body4=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs24,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs25,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs26,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("tr",attrs27,[Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("02/01/2015")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Waitrose")]),Doc.TextNode("\n                "),Doc.Element("td",[AttrProxy.Create("class","money")],[Doc.TextNode("-21.01")]),Doc.TextNode("\n                "),Doc.Element("td",[],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")])]);
      attrs28=[AttrProxy.Create("class","table-card")];
      attrs29=[];
      attrs2a=[];
      attrs2b=[];
      Content4=List.ofArray([Doc.Concat([Doc.Element("div",attrs28,[Doc.TextNode("\n    "),Doc.Element("table",attrs29,[Doc.TextNode("\n        "),Doc.Element("thead",attrs2a,[Doc.TextNode("\n            "),Doc.Element("tr",attrs2b,[Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Date")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Label")]),Doc.TextNode("\n                "),Doc.Element("th",[AttrProxy.Create("class","right")],[Doc.TextNode("Amount")]),Doc.TextNode("\n                "),Doc.Element("th",[],[Doc.TextNode("Category")]),Doc.TextNode("\n            ")]),Doc.TextNode("\n        ")]),Doc.TextNode("\n        "),Doc.Element("tbody",[],Arrays.ofSeq(Body4)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")])])]);
      attrs2c=[AttrProxy.Create("class","card-list-item"),AttrProxy.Create("data-content","content-1")];
      Items2=List.ofArray([Doc.Concat([Doc.TextNode("\n        "),Doc.Element("div",attrs2c,[Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header")],[Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-title")],[Doc.TextNode("Supermarket")]),Doc.TextNode("\n                "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-header-subtitle")],[Doc.Element("div",[AttrProxy.Create("class","amount")],[Doc.TextNode("20.00")])]),Doc.TextNode("\n            ")]),Doc.TextNode("\n            "),Doc.Element("div",[AttrProxy.Create("class","card-list-item-content"),AttrProxy.Create("id","content-1")],Arrays.ofSeq(Content4)),Doc.TextNode("\n        ")]),Doc.TextNode("\n    ")])]);
      return Doc.Concat(List.ofArray([Doc.Concat([Doc.Element("div",[AttrProxy.Create("class","card")],[Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-title")],[Doc.TextNode("April 2016")]),Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-content")],Arrays.ofSeq(Items)),Doc.TextNode("\n")])]),Doc.Concat([Doc.Element("div",[AttrProxy.Create("class","card")],[Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-title")],[Doc.TextNode("March 2016")]),Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-content")],Arrays.ofSeq(Items1)),Doc.TextNode("\n")])]),Doc.Concat([Doc.Element("div",[AttrProxy.Create("class","card")],[Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-title")],[Doc.TextNode("February 2016")]),Doc.TextNode("\n    "),Doc.Element("div",[AttrProxy.Create("class","card-content")],Arrays.ofSeq(Items2)),Doc.TextNode("\n")])])]));
     }),
     nav:Runtime.Field(function()
     {
      var Categories,Links,arg20,arg201,Links1,arg202,arg203,attrs;
      arg20=List.ofArray([Doc.TextNode("1")]);
      arg201=List.ofArray([Doc.TextNode("2")]);
      Links=List.ofArray([Doc.Element("a",[],arg20),Doc.Element("a",[],arg201)]);
      arg202=List.ofArray([Doc.TextNode("1")]);
      arg203=List.ofArray([Doc.TextNode("2")]);
      Links1=List.ofArray([Doc.Element("a",[],arg202),Doc.Element("a",[],arg203)]);
      Categories=List.ofArray([Doc.Concat([Doc.TextNode("\n            "),Doc.Element("dt",[],[Doc.TextNode(" "),Doc.TextNode("Expenses")]),Doc.TextNode("\n            "),Doc.Element("dd",[],Arrays.ofSeq(Links)),Doc.TextNode("\n        ")]),Doc.Concat([Doc.TextNode("\n            "),Doc.Element("dt",[],[Doc.TextNode(" "),Doc.TextNode("Dates")]),Doc.TextNode("\n            "),Doc.Element("dd",[],Arrays.ofSeq(Links1)),Doc.TextNode("\n        ")])]);
      attrs=[];
      return Doc.Concat([Doc.Element("header",[],[Doc.TextNode(" "),Doc.TextNode("Expenses")]),Doc.TextNode("\n"),Doc.Element("nav",attrs,[Doc.TextNode("\n    "),Doc.Element("button",[AttrProxy.Create("id","side-menu-button"),AttrProxy.Create("class","nav-menu")],[Doc.TextNode("\n        "),Doc.Element("i",[AttrProxy.Create("class","fa fa-bars")],[]),Doc.TextNode("\n    ")]),Doc.TextNode("\n\n    "),Doc.Element("div",[AttrProxy.Create("id","side-menu")],[Doc.TextNode("\n        "),Doc.Element("div",[AttrProxy.Create("class","brand")],[Doc.TextNode("List design")]),Doc.TextNode("\n        "),Doc.Element("dl",[],Arrays.ofSeq(Categories)),Doc.TextNode("\n    ")]),Doc.TextNode("\n")]),Doc.TextNode("\n\n"),Doc.Element("div",[AttrProxy.Create("id","mask")],[])]);
     })
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
  List=Runtime.Safe(Global.WebSharper.List);
  AttrProxy=Runtime.Safe(Next.AttrProxy);
  Arrays=Runtime.Safe(Global.WebSharper.Arrays);
  London=Runtime.Safe(Global.London);
  Web=Runtime.Safe(London.Web);
  return App=Runtime.Safe(Web.App);
 });
 Runtime.OnLoad(function()
 {
  App.nav();
  App.main();
  return;
 });
}());
