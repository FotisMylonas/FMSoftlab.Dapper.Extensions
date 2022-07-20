"# FMSoftlab.Dapper.Extensions" 

A small Dapper extension for supporting more easily Sql Server table value parameters with Dapper

```cs
IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
dyn.AddTableValuedParam<ListItem>("idlist", "dbo.idlist", idlist);
var results = con.Query<string>(@"select id from @idlist idlist", dyn).AsList<string>();
```
