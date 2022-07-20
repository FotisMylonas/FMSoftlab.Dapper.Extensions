"# FMSoftlab.Dapper.Extensions" 

A small Dapper extension for supporting more easily Sql Server table value parameters with Dapper.
It gives you the chance to use a POCO class and pass it as a parameter to the query

Example:
```cs
public class ListItem
{
  [MaxLength(5)]
  public string Id { get; set; }
}

IEnumerable<ListItem> idlist = new List<ListItem>() { new ListItem { Id = "1" }, new ListItem { Id = "2" } };
DynamicParameters dyn = new DynamicParameters();
dyn.AddTableValuedParam<ListItem>("idlist", "dbo.idlist", idlist);
var results = con.Query<string>(@"select id from @idlist idlist", dyn).AsList<string>();
```
